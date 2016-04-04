/* 
Copyright (c) 2016 Denis Zykov, GameDevWare.com

https://www.assetstore.unity3d.com/#!/content/56706

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Serialization.Json.Exceptions;

namespace Serialization.Json
{
	public abstract class JsonReaderBase : IJsonReader
	{
		private static readonly long UNIX_EPOCH_TICKS = new DateTime(0x7b2, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
		protected const int DEFAULT_BUFFER_SIZE = 1024;

		private sealed class Buffer : IList<char>
		{
			public const float SHIFT_THRESHOLD = 0.5f; // postition over 50% of buffer
			public const float GROW_THRESHOLD = 0.1f; // when less that 10% of space is free

			private readonly JsonReaderBase reader;
			private char[] buffer;
			private int offset;
			private int? lazyFixation;
			private int end;
			private int initialSize;
			private bool isLast;
			private int lineNumber;
			private long lineStartIndex;
			private long charsReaded;
			private bool dontCountLines;

			public int Capacity
			{
				get { return buffer.Length; }
				set
				{
					if (value <= 0)
						throw new ArgumentOutOfRangeException("value");

					var newBuffer = new char[value];
					BlockCopy(buffer, 0, newBuffer, 0, Math.Min(newBuffer.Length, buffer.Length));
					buffer = newBuffer;
				}
			}

			public int Offset
			{
				get { return offset; }
			}

			public bool DontCountLines
			{
				get { return this.dontCountLines; }
				set { this.dontCountLines = value; }
			}

			public long CharactersReaded
			{
				get { return charsReaded + offset; }
			}

			public int LineNumber
			{
				get { return lineNumber + 1; }
			}

			public int ColumnNumber
			{
				get { return (int)(this.CharactersReaded - lineStartIndex + 1); }
			}

			public char this[int index]
			{
				get
				{
					if (index < 0)
						throw new ArgumentOutOfRangeException("index");

					if (this.IsBeyondOfStream(index))
						return (char)0;

					return buffer[offset + index];
				}
			}

			public Buffer(int size, JsonReaderBase reader)
			{
				if (size <= 0)
					throw new ArgumentOutOfRangeException("size");
				if (reader == null)
					throw new ArgumentNullOrEmptyException("reader");


				if (size < 10)
					size = 10;

				this.reader = reader;
				this.buffer = new char[size];
				this.initialSize = size;
				this.end = 0;
				this.offset = 0;
			}

			public void FixateNow()
			{
				if (lazyFixation != null)
				{
					this.Fixate(lazyFixation.Value);
					lazyFixation = null;
				}
			}

			public void Fixate(int atIndex)
			{
				if (atIndex < 0)
					throw new ArgumentOutOfRangeException("position");


				if (!dontCountLines)
				{
					for (var i = 0; i < atIndex; i++)
					{
						if (this[i] == '\n')
						{
							lineNumber++;
							lineStartIndex = this.CharactersReaded + i;
						}
					}
				}

				// ensure that fixation point in loaded range
				IsBeyondOfStream(atIndex);

				offset += atIndex;

				// when we are at second half of buffer - we need to shift back
				if ((offset / (float)buffer.Length) > SHIFT_THRESHOLD)
					this.ShiftToZero();
			}

			public void FixateLater(int atIndex)
			{
				if (atIndex < 0)
					throw new ArgumentOutOfRangeException("position");


				if (lazyFixation != null)
					lazyFixation += atIndex;
				else
					lazyFixation = atIndex;
			}

			public bool IsBeyondOfStream(int index)
			{
				if (!isLast && offset + index >= end)
					this.ReadNextBlock();

				if (isLast && offset + index >= end)
					return true;

				return false;
			}

			public char[] GetChars()
			{
				return this.buffer;
			}

			public void Reset()
			{
				this.FixateNow();
				this.ShiftToZero();
				this.charsReaded = 0;
				this.lineNumber = 0;
				this.lineStartIndex = 0;
			}

			private void ReadNextBlock()
			{
				// when we are at second half of buffer - we need to shift back
				if ((offset / (float)buffer.Length) > SHIFT_THRESHOLD)
					this.ShiftToZero();

				// check for free space
				var free = buffer.Length - end;
				if ((free / (float)initialSize) < GROW_THRESHOLD)
					this.Capacity += initialSize;

				var newEnd = reader.FillBuffer(buffer, end);
				isLast = newEnd == end;
				end = newEnd;
			}

			private void ShiftToZero()
			{
				charsReaded += offset;

				var block = Math.Min(offset, end - offset);
				var start = offset;
				var lastBlock = 0;
				while (start < end)
				{
					BlockCopy(buffer, start, buffer, lastBlock, Math.Min(block, end - start));
					lastBlock += block;
					start += block;
				}
				end = end - offset;
				offset = 0;
#if DEBUG
				if (end < buffer.Length) // zero unused space(just for debug)
					Array.Clear(buffer, end, buffer.Length - end);
#endif
			}

			private static void BlockCopy(char[] from, int fromIdx, char[] to, int toIdx, int len)
			{
				const int CHAR_SIZE = sizeof(char);

				System.Buffer.BlockCopy(from, fromIdx * CHAR_SIZE, to, toIdx * CHAR_SIZE, len * CHAR_SIZE);
			}

			public override string ToString()
			{
				return new string(buffer, offset, end - offset);
			}

			#region IList<char> Members

			int IList<char>.IndexOf(char item)
			{
				throw new NotSupportedException();
			}

			void IList<char>.Insert(int index, char item)
			{
				throw new NotSupportedException();
			}

			void IList<char>.RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			char IList<char>.this[int index]
			{
				get { return this[index]; }
				set { throw new NotImplementedException(); }
			}

			#endregion

			#region ICollection<char> Members

			void ICollection<char>.Add(char item)
			{
				throw new NotSupportedException();
			}

			void ICollection<char>.Clear()
			{
				throw new NotSupportedException();
			}

			bool ICollection<char>.Contains(char item)
			{
				throw new NotSupportedException();
			}

			void ICollection<char>.CopyTo(char[] array, int arrayIndex)
			{
				throw new NotSupportedException();
			}

			int ICollection<char>.Count
			{
				get { return buffer.Length; }
			}

			bool ICollection<char>.IsReadOnly
			{
				get { return true; }
			}

			bool ICollection<char>.Remove(char item)
			{
				throw new NotSupportedException();
			}

			#endregion

			#region IEnumerable<char> Members

			IEnumerator<char> IEnumerable<char>.GetEnumerator()
			{
				return (this.buffer as IList<char>).GetEnumerator();
			}

			#endregion

			#region IEnumerable Members

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.buffer.GetEnumerator();
			}

			#endregion
		}

		private sealed class LazyValueInfo : IValueInfo
		{
			private enum Kind : byte
			{
				Explicit = 0,
				QuotedString,
				String
			};

			private readonly JsonReaderBase reader;
			private int jsonStart;
			private int jsonLen;
			private object value;
			private Kind valueKind;

			public bool HasValue { get; private set; }
			public object Raw
			{
				get
				{
					// eval lazy value
					if (valueKind == Kind.String)
						this.Raw = new string(reader.buffer.GetChars(), reader.buffer.Offset + jsonStart, jsonLen);
					else if (valueKind == Kind.QuotedString)
						this.Raw = JsonUtils.UnescapeBuffer(reader.buffer.GetChars(), reader.buffer.Offset + jsonStart, jsonLen);

					return this.value;
				}
				set
				{
					this.valueKind = Kind.Explicit;
					this.value = value;
					this.HasValue = true;
				}
			}
			public Type ProbableType
			{
				get
				{
					if (valueKind != Kind.Explicit)
					{
						switch (reader.token)
						{
							case JsonToken.BeginArray:
								return typeof(List<object>);
							case JsonToken.Number:
								return typeof(double);
							case JsonToken.StringLiteral:
								return typeof(string);
							case JsonToken.DateTime:
								return typeof(DateTime);
							case JsonToken.Boolean:
								return typeof(Boolean);
						}
					}

					if (value != null)
						return value.GetType();
					else
						return typeof(object);
				}
			}
			public int LineNumber { get; private set; }
			public int ColumnNumber { get; private set; }
			public bool AsBoolean { get { return Convert.ToBoolean(this.Raw, reader.Context.Format); } }
			public byte AsByte { get { return Convert.ToByte(this.Raw, reader.Context.Format); } }
			public short AsInt16 { get { return Convert.ToInt16(this.Raw, reader.Context.Format); } }
			public int AsInt32 { get { return Convert.ToInt32(this.Raw, reader.Context.Format); } }
			public long AsInt64 { get { return Convert.ToInt64(this.Raw, reader.Context.Format); } }
			public sbyte AsSByte { get { return Convert.ToSByte(this.Raw, reader.Context.Format); } }
			public ushort AsUInt16 { get { return Convert.ToUInt16(this.Raw, reader.Context.Format); } }
			public uint AsUInt32 { get { return Convert.ToUInt32(this.Raw, reader.Context.Format); } }
			public ulong AsUInt64 { get { return Convert.ToUInt64(this.Raw, reader.Context.Format); } }
			public float AsSingle { get { return Convert.ToSingle(this.Raw, reader.Context.Format); } }
			public double AsDouble { get { return Convert.ToDouble(this.Raw, reader.Context.Format); } }
			public decimal AsDecimal { get { return Convert.ToDecimal(this.Raw, reader.Context.Format); } }
			public DateTime AsDateTime
			{
				get
				{
					if (this.Raw is DateTime) return (DateTime)this.Raw;
					else
						return DateTime.ParseExact(this.AsString, reader.Context.DateTimeFormats, reader.Context.Format,
							DateTimeStyles.AssumeUniversal);
				}
			}
			public string AsString
			{
				get
				{
					var raw = this.Raw;
					if (raw is string)
						return (string)raw;
					else
						return Convert.ToString(raw, reader.Context.Format);
				}
			}

			public LazyValueInfo(JsonReaderBase reader)
			{
				if (reader == null)
					throw new ArgumentNullException("reader");

				this.reader = reader;
			}

			public void ClearValue()
			{
				this.value = null;
				this.HasValue = false;
				this.valueKind = Kind.String;
			}

			public void CopyJsonTo(StringBuilder stringBuilder)
			{
				if (stringBuilder == null)
					throw new ArgumentNullException("stringBuilder");


				var buffer = reader.buffer.GetChars();
				var start = jsonStart + reader.buffer.Offset;
				var length = jsonLen;

				if (valueKind == Kind.QuotedString)
				{
					start -= 1;
					length += 2;
				}

				stringBuilder.Append(buffer, start, length);
			}
			public void CopyJsonTo(TextWriter writer)
			{
				if (writer == null)
					throw new ArgumentNullException("writer");


				var buffer = reader.buffer.GetChars();
				var start = jsonStart + reader.buffer.Offset;
				var length = jsonLen;

				if (valueKind == Kind.QuotedString)
				{
					start -= 1;
					length += 2;
				}

				writer.Write(buffer, start, length);
			}
			public void CopyJsonTo(Stream stream)
			{
				if (stream == null)
					throw new ArgumentNullException("stream");


				var buffer = reader.buffer.GetChars();
				var start = jsonStart + reader.buffer.Offset;
				var length = jsonLen;
				var writer = new BinaryWriter(stream, reader.Context.Encoding);

				if (valueKind == Kind.QuotedString)
				{
					start -= 1;
					length += 2;
				}

				writer.Write(buffer, start, length);
				writer.Flush();
			}
			public void CopyJsonTo(IJsonWriter writer)
			{
				if (writer == null)
					throw new ArgumentNullException("writer");


				var buffer = reader.buffer.GetChars();
				var start = jsonStart + reader.buffer.Offset;
				var length = jsonLen;

				if (valueKind == Kind.QuotedString)
				{
					start -= 1;
					length += 2;
				}

				writer.WriteJson(buffer, start, length);
			}

			public void SetBufferBounds(int start, int len)
			{
				if (start < 0)
					throw new ArgumentOutOfRangeException("start");
				if (len < 0)
					throw new ArgumentOutOfRangeException("len");


				LineNumber = reader.buffer.LineNumber;
				ColumnNumber = reader.buffer.ColumnNumber + start;
				jsonStart = start;
				jsonLen = len;
			}
			public void SetAsLazyString(bool quoted)
			{
				valueKind = quoted ? Kind.QuotedString : Kind.String;
				HasValue = true;
			}
		}

		private const char INSIGNIFICANT_TAB = '\t';
		private const char INSIGNIFICANT_SPACE = ' ';
		private const char INSIGNIFICANT_NEWLINE = '\n';
		private const char INSIGNIFICANT_RETURN = '\r';
		private const char INSIGNIFICANT_NAME_SEPARATOR = ':';
		private const char INSIGNIFICANT_VALUE_SEPARATOR = ',';
		private const char SIGNIFICANT_BEGIN_ARRAY = '[';
		private const char SIGNIFICANT_END_ARRAY = ']';
		private const char SIGNIFICANT_BEGIN_OBJECT = '{';
		private const char SIGNIFICANT_END_OBJECT = '}';
		private const char SIGNIFICANT_QUOTE = '\"';
		private const char SIGNIFICANT_QUOTE_ALT = '\'';

		private LazyValueInfo lazyValue;
		private JsonToken token;
		private readonly Buffer buffer;

		protected JsonReaderBase(ISerializationContext context, int bufferSize = DEFAULT_BUFFER_SIZE)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			if (bufferSize <= 0)
				throw new IndexOutOfRangeException("bufferSize");


			this.Context = context;
			lazyValue = new LazyValueInfo(this);
			buffer = new Buffer(bufferSize, this);
		}

		#region IJsonReader Members

		public ISerializationContext Context { get; private set; }

		public IValueInfo Value
		{
			get
			{
				if (this.Token == JsonToken.None)
					this.NextToken();
				return lazyValue;
			}
		}

		public JsonToken Token
		{
			get
			{
				if (token == JsonToken.None)
					this.NextToken();

				return token;
			}
		}

		public object RawValue
		{
			get
			{
				if (token == JsonToken.None)
					this.NextToken();

				return this.Value.Raw;
			}
		}

		public long CharactersReaded
		{
			get { return this.buffer.CharactersReaded; }
		}

		public bool NextToken()
		{
			var start = 0;
			var len = 0;
			var quoted = false;
			var isMember = false;

			if (!this.NextLexeme(ref start, ref len, ref quoted, ref isMember)) // end of stream
			{
				token = JsonToken.EndOfStream;
				lazyValue.Raw = null;
				return false;
			}

			lazyValue.ClearValue();
			lazyValue.SetBufferBounds(start, len);
			if (len == 1 && !quoted && buffer[start] == SIGNIFICANT_BEGIN_ARRAY)
			{
				token = JsonToken.BeginArray;
			}
			else if (len == 1 && !quoted && buffer[start] == SIGNIFICANT_BEGIN_OBJECT)
			{
				token = JsonToken.BeginObject;
			}
			else if (len == 1 && !quoted && buffer[start] == SIGNIFICANT_END_ARRAY)
			{
				token = JsonToken.EndOfArray;
			}
			else if (len == 1 && !quoted && buffer[start] == SIGNIFICANT_END_OBJECT)
			{
				token = JsonToken.EndOfObject;
			}
			else if (len == 4 && !quoted && this.LookupAt(buffer, start, len, "null"))
			{
				token = JsonToken.Null;
			}
			else if (len == 4 && !quoted && this.LookupAt(buffer, start, len, "true"))
			{
				token = JsonToken.Boolean;
				lazyValue.Raw = true;
			}
			else if (len == 5 && !quoted && this.LookupAt(buffer, start, len, "false"))
			{
				token = JsonToken.Boolean;
				lazyValue.Raw = false;
			}
			else if (quoted && LookupAt(buffer, start, 6, "/Date(") && LookupAt(buffer, start + len - 2, 2, ")/"))
			{
				var ticks = JsonUtils.StringToInt64(buffer.GetChars(), buffer.Offset + start + 6, len - 8);

				token = JsonToken.DateTime;
				var dateTime = new DateTime(ticks * 0x2710L + UNIX_EPOCH_TICKS, DateTimeKind.Utc);
				lazyValue.Raw = dateTime;
			}
			else if (!quoted && IsNumber(buffer, start, len))
			{
				token = JsonToken.Number;
				lazyValue.SetAsLazyString(false);
			}
			else
			{
				token = isMember ? JsonToken.Member : JsonToken.StringLiteral;
				lazyValue.SetAsLazyString(quoted);
			}

			buffer.FixateLater(start + len + (quoted ? 1 : 0));

			return true;
		}

		public bool IsEndOfStream()
		{
			return token == JsonToken.EndOfStream;
		}

		public void Reset()
		{
			buffer.Reset();
			if (token != JsonToken.EndOfStream)
			{
				lazyValue = new LazyValueInfo(this);
				token = JsonToken.None;
			}
		}

		#endregion

		private static bool IsNumber(Buffer buffer, int start, int len)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer");
			if (start < 0)
				throw new ArgumentOutOfRangeException("start");
			if (len < 0)
				throw new ArgumentOutOfRangeException("len");


			const int INT_PART = 0;
			const int FRAC_PART = 1;
			const int EXP_PART = 2;
			const char POINT = '.';
			const char EXP = 'E';
			const char PLUS = '+';
			const char MINUS = '-';

			len = start + len;

			var part = INT_PART;

			for (var i = start; i < len; i++)
			{
				var ch = buffer[i];

				switch (part)
				{
					case INT_PART:
						if (ch == MINUS)
						{
							if (i != start)
								return false;
						}
#if !STRICT
						else if (ch == PLUS)
						{
							if (i != start)
								return false;
						}
#endif
						else if (ch == POINT)
						{
							if (i == start)
								return false; // decimal point as first character
							else
								part = FRAC_PART;
						}
						else if (Char.ToUpper(ch) == EXP)
						{
							if (i == start)
								return false; // exp at first character
							else
								part = EXP_PART;
						}
						else if (!Char.IsDigit(ch))
							return false; // non digit character in int part
						break;
					case FRAC_PART:
						if (Char.ToUpper(ch) == EXP)
						{
							if (i == start)
								return false; // exp at first character
							else
								part = EXP_PART;
						}
						else if (!Char.IsDigit(ch))
							return false; // non digit character in frac part
						break;
					case EXP_PART:
						if ((ch == PLUS || ch == MINUS))
						{
							if (Char.ToUpper(buffer[i - 1]) != EXP)
								return false; // sign not at start of exp part
						}
						else if (!Char.IsDigit(ch))
							return false; // non digit character in exp part
						break;
				}
			}
			return true;
		}
		private static bool IsInsignificantWhitespace(char symbol)
		{
			return symbol == INSIGNIFICANT_NEWLINE || symbol == INSIGNIFICANT_RETURN || symbol == INSIGNIFICANT_SPACE ||
				   symbol == INSIGNIFICANT_TAB;
		}
		private static bool IsInsignificant(char symbol)
		{
			return symbol == INSIGNIFICANT_NEWLINE || symbol == INSIGNIFICANT_RETURN || symbol == INSIGNIFICANT_SPACE ||
				   symbol == INSIGNIFICANT_TAB || symbol == INSIGNIFICANT_NAME_SEPARATOR || symbol == INSIGNIFICANT_VALUE_SEPARATOR;
		}
		private static bool IsLiteralTerminator(char ch, bool quoted, char quoteCh, bool escaped, bool eos, IJsonReader reader)
		{
			if (!escaped && quoted && ch == quoteCh)
				return true;
			else if (quoted && (ch == INSIGNIFICANT_NEWLINE || ch == INSIGNIFICANT_RETURN))
				throw new UnterminatedStringLiteral(reader);
			else if (eos)
			{
				if (quoted)
					throw new UnexpectedEndOfStream(reader);
				else
					return true;
			}
			else if (!quoted &&
					 (ch == SIGNIFICANT_BEGIN_ARRAY || ch == SIGNIFICANT_BEGIN_OBJECT || ch == SIGNIFICANT_END_ARRAY ||
					  ch == SIGNIFICANT_END_OBJECT || ch == INSIGNIFICANT_VALUE_SEPARATOR || ch == INSIGNIFICANT_NAME_SEPARATOR))
				return true;
			else if (!quoted && IsInsignificantWhitespace(ch))
				return true;

			return false;
		}
		private bool LookupAt(Buffer buffer, int start, int len, string matchString)
		{
			for (var i = 0; i < len; i++)
			{
				if (buffer[start + i] != matchString[i])
					return false;
			}
			return true;
		}
		private bool LookupAtSkipWhitespace(Buffer buffer, int start, int len, string matchString)
		{
			while (IsInsignificantWhitespace(buffer[start])) start++;

			for (var i = 0; i < len; i++)
			{
				if (buffer[start + i] != matchString[i])
					return false;
			}
			return true;
		}

		/// <summary>
		///     Get next lexeme from current buffer
		/// </summary>
		/// <param name="start">return position of returned lexeme in buffer</param>
		/// <param name="len">return size of returned lexeme</param>
		/// <param name="quoted">return true when string literal was quoted</param>
		/// <returns>Null in case of "end of stream", or character buffer with result</returns>
		private bool NextLexeme(ref int start, ref int len, ref bool quoted, ref bool isMember)
		{
			// apply 'lazy' fixation
			buffer.FixateNow();

			if (buffer.IsBeyondOfStream(0))
				return false;

			var position = 0;
			var ch = buffer[position];

			// skip insignificant characters
			while (!buffer.IsBeyondOfStream(position) && IsInsignificant(buffer[position])) position++;

			// we reached end of stream
			if (buffer.IsBeyondOfStream(position))
				return false;


			// tell buffer that significant characters starts here
			// this prevents buffer overgrow
			buffer.Fixate(position);
			position = 0;
			var literalStart = position;
			//

			ch = buffer[position];
			//
			// check for quote character
			var quoteCh = '\0';
			if (ch == SIGNIFICANT_QUOTE)
			{
				quoteCh = ch;
				quoted = true;
				position++;
				literalStart++;
			}
#if !STRICT
			else if (ch == SIGNIFICANT_QUOTE_ALT)
			{
				quoteCh = ch;
				quoted = true;
				position++;
				literalStart++;
			}
#endif
			var escaped = false; // is character is escaped
			var eos = false; // is end of stream
			do
			{
				eos = buffer.IsBeyondOfStream(position);
				escaped = ch == '\\';
				ch = buffer[position];
				position++;
			} while (!IsLiteralTerminator(ch, quoted, quoteCh, escaped, eos, this));

			var literalEnd = position - 1; // minus terminator character

			start = literalStart;
			len = literalEnd - literalStart;

			// special case - self terminated lexeme
			if (literalStart == literalEnd && !quoted)
				len = 1;

			isMember = this.LookupAtSkipWhitespace(buffer, literalEnd + (quoted ? 1 : 0), 1, ":");

			return true;
		}

		/// <summary>
		///     Fills buffer with new characters, staring from <paramref name="index" />
		/// </summary>
		/// <param name="buffer">Character buffer to fill</param>
		/// <param name="index">index from which to start</param>
		/// <returns>new buffer size</returns>
		protected abstract int FillBuffer(char[] buffer, int index);
	}
}