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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using GameDevWare.Serialization.Exceptions;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization.MessagePack
{
	public class MsgPackReader : IJsonReader
	{
		internal class MsgPackValueInfo : IValueInfo
		{
			private readonly MsgPackReader reader;
			private object value;

			internal MsgPackValueInfo(MsgPackReader reader)
			{
				if (reader == null) throw new ArgumentNullException("reader");

				this.reader = reader;
			}

			public JsonToken Token { get; private set; }
			public bool HasValue { get; private set; }
			public object Raw
			{
				get { return this.value; }
				set
				{
					this.value = value;
					this.HasValue = true;
				}
			}
			public Type ProbableType
			{
				get
				{
					if (this.HasValue && value != null)
						return value.GetType();
					else
					{
						switch (this.Token)
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
								return typeof(bool);
						}
						return typeof(object);
					}
				}
			}
			public int LineNumber { get { return 0; } }
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
					if (this.Raw is DateTime)
						return (DateTime)this.Raw;
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

			public void CopyJsonTo(StringBuilder stringBuilder)
			{
				if (this.HasValue && this.value is JsonString)
					stringBuilder.Append((this.value as JsonString).ToString());
				else
					throw new InvalidOperationException("This operation available only for JsonString data type.");
			}
			public void CopyJsonTo(TextWriter writer)
			{
				if (this.HasValue && this.value is JsonString)
					writer.Write((this.value as JsonString).ToString());
				else
					throw new InvalidOperationException("This operation available only for JsonString data type.");
			}
			public void CopyJsonTo(Stream stream)
			{
				using (var textWriter = new StreamWriter(stream, reader.Context.Encoding))
					this.CopyJsonTo(textWriter);
			}
			public void CopyJsonTo(IJsonWriter writer)
			{
				if (this.HasValue && this.value is JsonString)
					writer.WriteJson((this.value as JsonString).ToString());
				else
					throw new InvalidOperationException("This operation available only for JsonString data type.");
			}

			public void Reset()
			{
				this.value = null;
				this.HasValue = false;
				this.Token = JsonToken.None;
				this.ColumnNumber = 0;
			}
			public void SetValue(object rawValue, JsonToken token, int position)
			{
				this.HasValue = true;
				this.value = rawValue;
				this.Token = token;
				this.ColumnNumber = position;
			}

			public override string ToString()
			{
				if (this.HasValue)
					return Convert.ToString(this.value);
				else
					return "<no value>";
			}
		}
		internal struct ClosingToken
		{
			public JsonToken Token;
			public long Counter;
		}

		private readonly Stream inputStream;
		private readonly byte[] buffer;
		private readonly EndianBitConverter bitConverter;
		private readonly Stack<ClosingToken> closingTokens;
		private bool isEndOfStream;
		private int bytesReaded;

		public ISerializationContext Context { get; private set; }
		JsonToken IJsonReader.Token
		{
			get
			{
				if (this.Value.Token == JsonToken.None)
					this.NextToken();
				if (this.isEndOfStream)
					return JsonToken.EndOfStream;

				return this.Value.Token;
			}
		}
		object IJsonReader.RawValue
		{
			get
			{
				if (this.Value.Token == JsonToken.None)
					this.NextToken();

				return this.Value.Raw;
			}
		}
		IValueInfo IJsonReader.Value
		{
			get
			{
				if (this.Value.Token == JsonToken.None)
					this.NextToken();

				return this.Value;
			}
		}
		long IJsonReader.CharactersReaded
		{
			get { return this.bytesReaded; }
		}
		internal MsgPackValueInfo Value { get; private set; }

		public MsgPackReader(Stream stream, ISerializationContext context)
		{
			if (stream == null) throw new ArgumentNullException("stream");
			if (context == null) throw new ArgumentNullException("context");
			if (!stream.CanRead) throw new UnreadableStream("stream");

			this.Context = context;
			this.inputStream = stream;
			this.buffer = new byte[16];
			this.bitConverter = EndianBitConverter.Little;
			this.closingTokens = new Stack<ClosingToken>();

			this.Value = new MsgPackValueInfo(this);
		}

		public bool NextToken()
		{
			this.Value.Reset();

			if (this.closingTokens.Count > 0 && this.closingTokens.Peek().Counter == 0)
			{
				var closingToken = this.closingTokens.Pop();
				this.Value.SetValue(null, closingToken.Token, this.bytesReaded);

				this.DecrementClosingTokenCounter();

				return true;
			}

			if (!this.ReadToBuffer(1, throwOnEos: false))
			{
				this.isEndOfStream = true;
				return false;
			}

			var pos = this.bytesReaded;
			var formatValue = buffer[0];
			if (formatValue >= (byte)MsgPackType.FixArrayStart && formatValue <= (byte)MsgPackType.FixArrayEnd)
			{
				var arrayCount = formatValue - (byte)MsgPackType.FixArrayStart;

				this.closingTokens.Push(new ClosingToken { Token = JsonToken.EndOfArray, Counter = arrayCount + 1 });
				this.Value.SetValue(null, JsonToken.BeginArray, pos);
			}
			else if (formatValue >= (byte)MsgPackType.FixMapStart && formatValue <= (byte)MsgPackType.FixMapEnd)
			{
				var mapCount = formatValue - (byte)MsgPackType.FixMapStart;
				this.closingTokens.Push(new ClosingToken { Token = JsonToken.EndOfObject, Counter = mapCount * 2 + 1 });
				this.Value.SetValue(null, JsonToken.BeginObject, pos);
			}
			else if (formatValue >= (byte)MsgPackType.NegativeFixIntStart && formatValue <= (byte)MsgPackType.NegativeFixIntEnd)
			{
				var value = -(formatValue - (byte)MsgPackType.NegativeFixIntStart);
				this.Value.SetValue(value, JsonToken.Number, pos);
			}
			else if (formatValue >= (byte)MsgPackType.PositiveFixIntStart && formatValue <= (byte)MsgPackType.PositiveFixIntEnd)
			{
				var value = formatValue - (byte)MsgPackType.PositiveFixIntStart;
				this.Value.SetValue(value, JsonToken.Number, pos);
			}
			else
			{
				switch ((MsgPackType)formatValue)
				{
					case MsgPackType.Nil:
						this.Value.SetValue(null, JsonToken.Null, pos);
						break;
					case MsgPackType.Array16:
					case MsgPackType.Array32:
						var arrayCount = 0L;
						if (formatValue == (int)MsgPackType.Array16)
						{
							this.ReadToBuffer(2, throwOnEos: true);
							arrayCount = bitConverter.ToUInt16(this.buffer, 0);
						}
						else if (formatValue == (int)MsgPackType.Array32)
						{
							this.ReadToBuffer(4, throwOnEos: true);
							arrayCount = bitConverter.ToUInt32(this.buffer, 0);
						}
						this.closingTokens.Push(new ClosingToken { Token = JsonToken.EndOfArray, Counter = arrayCount + 1 });
						this.Value.SetValue(null, JsonToken.BeginArray, pos);
						break;
					case MsgPackType.Map16:
					case MsgPackType.Map32:
						var mapCount = 0L;
						if (formatValue == (int)MsgPackType.Map16)
						{
							this.ReadToBuffer(2, throwOnEos: true);
							mapCount = bitConverter.ToUInt16(this.buffer, 0);
						}
						else if (formatValue == (int)MsgPackType.Map32)
						{
							this.ReadToBuffer(4, throwOnEos: true);
							mapCount = bitConverter.ToUInt32(this.buffer, 0);
						}
						this.closingTokens.Push(new ClosingToken { Token = JsonToken.EndOfObject, Counter = mapCount * 2 + 1 });
						this.Value.SetValue(null, JsonToken.BeginObject, pos);
						break;
					case MsgPackType.Str16:
					case MsgPackType.Str32:
					case MsgPackType.Str8:
						var strBytesCount = 0L;
						if (formatValue == (int)MsgPackType.Str8)
						{
							this.ReadToBuffer(1, throwOnEos: true);
							strBytesCount = this.buffer[0];
						}
						else if (formatValue == (int)MsgPackType.Str16)
						{
							this.ReadToBuffer(2, throwOnEos: true);
							strBytesCount = bitConverter.ToUInt16(this.buffer, 0);
						}
						else if (formatValue == (int)MsgPackType.Str32)
						{
							this.ReadToBuffer(4, throwOnEos: true);
							strBytesCount = bitConverter.ToUInt32(this.buffer, 0);
						}

						var strTokenType = JsonToken.StringLiteral;
						if (this.closingTokens.Count > 0)
						{
							var closingToken = this.closingTokens.Peek();
							if (closingToken.Token == JsonToken.EndOfObject && closingToken.Counter > 0 && closingToken.Counter % 2 == 0)
								strTokenType = JsonToken.Member;
						}

						var strBytes = this.ReadBytes(strBytesCount);
						var strValue = this.Context.Encoding.GetString(strBytes);
						this.Value.SetValue(strValue, strTokenType, pos);
						break;
					case MsgPackType.Bin32:
					case MsgPackType.Bin16:
					case MsgPackType.Bin8:
						var bytesCount = 0L;
						if (formatValue == (int)MsgPackType.Bin8)
						{
							this.ReadToBuffer(1, throwOnEos: true);
							bytesCount = this.buffer[0];
						}
						else if (formatValue == (int)MsgPackType.Bin16)
						{
							this.ReadToBuffer(2, throwOnEos: true);
							bytesCount = bitConverter.ToUInt16(this.buffer, 0);
						}
						else if (formatValue == (int)MsgPackType.Bin32)
						{
							this.ReadToBuffer(4, throwOnEos: true);
							bytesCount = bitConverter.ToUInt32(this.buffer, 0);
						}

						var bytes = this.ReadBytes(bytesCount);
						var base64Bytes = Convert.ToBase64String(bytes);
						this.Value.SetValue(base64Bytes, JsonToken.StringLiteral, pos);
						break;
					case MsgPackType.FixExt1:
					case MsgPackType.FixExt16:
					case MsgPackType.FixExt2:
					case MsgPackType.FixExt4:
					case MsgPackType.FixExt8:
					case MsgPackType.Ext32:
					case MsgPackType.Ext16:
					case MsgPackType.Ext8:
						var dataCount = 0L;
						if (formatValue == (int)MsgPackType.FixExt1)
							dataCount = 1;
						else if (formatValue == (int)MsgPackType.FixExt2)
							dataCount = 2;
						else if (formatValue == (int)MsgPackType.FixExt4)
							dataCount = 4;
						else if (formatValue == (int)MsgPackType.FixExt8)
							dataCount = 8;
						else if (formatValue == (int)MsgPackType.FixExt16)
							dataCount = 16;
						if (formatValue == (int)MsgPackType.Ext8)
						{
							this.ReadToBuffer(1, throwOnEos: true);
							dataCount = this.buffer[0];
						}
						else if (formatValue == (int)MsgPackType.Ext16)
						{
							this.ReadToBuffer(2, throwOnEos: true);
							dataCount = bitConverter.ToUInt16(this.buffer, 0);
						}
						else if (formatValue == (int)MsgPackType.Ext32)
						{
							this.ReadToBuffer(4, throwOnEos: true);
							dataCount = bitConverter.ToUInt32(this.buffer, 0);
						}

						this.ReadToBuffer(1, throwOnEos: true);
						var extType = buffer[0];

						var data = this.ReadBytes(dataCount);
						this.ReadExtType(extType, data, pos);
						break;
					case MsgPackType.False:
						this.Value.SetValue(false, JsonToken.Boolean, pos);
						break;
					case MsgPackType.True:
						this.Value.SetValue(true, JsonToken.Boolean, pos);
						break;
					case MsgPackType.Float32:
						this.ReadToBuffer(4, throwOnEos: true);
						this.Value.SetValue(bitConverter.ToSingle(buffer, 0), JsonToken.Number, pos);
						break;
					case MsgPackType.Float64:
						this.ReadToBuffer(8, throwOnEos: true);
						this.Value.SetValue(bitConverter.ToDouble(buffer, 0), JsonToken.Number, pos);
						break;
					case MsgPackType.Int16:
						this.ReadToBuffer(2, throwOnEos: true);
						this.Value.SetValue(bitConverter.ToInt16(buffer, 0), JsonToken.Number, pos);
						break;
					case MsgPackType.Int32:
						this.ReadToBuffer(4, throwOnEos: true);
						this.Value.SetValue(bitConverter.ToInt32(buffer, 0), JsonToken.Number, pos);
						break;
					case MsgPackType.Int64:
						this.ReadToBuffer(8, throwOnEos: true);
						this.Value.SetValue(bitConverter.ToInt64(buffer, 0), JsonToken.Number, pos);
						break;
					case MsgPackType.Int8:
						this.ReadToBuffer(1, throwOnEos: true);
						this.Value.SetValue((sbyte)buffer[0], JsonToken.Number, pos);
						break;
					case MsgPackType.UInt16:
						this.ReadToBuffer(2, throwOnEos: true);
						this.Value.SetValue((ushort)bitConverter.ToInt16(buffer, 0), JsonToken.Number, pos);
						break;
					case MsgPackType.UInt32:
						this.ReadToBuffer(4, throwOnEos: true);
						this.Value.SetValue((uint)bitConverter.ToInt32(buffer, 0), JsonToken.Number, pos);
						break;
					case MsgPackType.UInt64:
						this.ReadToBuffer(8, throwOnEos: true);
						this.Value.SetValue((ulong)bitConverter.ToInt64(buffer, 0), JsonToken.Number, pos);
						break;
					case MsgPackType.UInt8:
						this.ReadToBuffer(1, throwOnEos: true);
						this.Value.SetValue(buffer[0], JsonToken.Number, pos);
						break;
					default:
						throw new UnknownMsgPackFormatException(formatValue);
				}
			}

			this.DecrementClosingTokenCounter();

			return true;
		}
		public void Reset()
		{
			Array.Clear(this.buffer, 0, this.buffer.Length);
			this.bytesReaded = 0;
			this.Value.Reset();
		}
		public bool IsEndOfStream()
		{
			return isEndOfStream;
		}

		private bool ReadToBuffer(int bytesCount, bool throwOnEos)
		{
			if (this.inputStream.Read(buffer, 0, bytesCount) != bytesCount)
			{
				if (throwOnEos)
					throw new UnexpectedEndOfStream(this);

				return false;
			}
			this.bytesReaded += 1;
			return true;
		}
		private byte[] ReadBytes(long byteCount)
		{
			var bytes = new byte[byteCount];
			if (this.inputStream.Read(bytes, 0, bytes.Length) != bytes.Length)
				throw new UnexpectedEndOfStream(this);

			this.bytesReaded += (int)byteCount;

			return bytes;
		}
		private void ReadExtType(byte extType, byte[] data, int pos)
		{
			switch ((MsgPackExtType)extType)
			{
				case MsgPackExtType.DateTime:
					this.Value.SetValue(new DateTime(bitConverter.ToInt64(data, 0), DateTimeKind.Utc), JsonToken.DateTime, pos);
					break;
				case MsgPackExtType.Decimal:
					this.Value.SetValue(bitConverter.ToDecimal(data, 0), JsonToken.Number, pos);
					break;
				case MsgPackExtType.JsonString:
					this.Value.SetValue(new JsonString(this.Context.Encoding.GetString(data)), JsonToken.StringLiteral, pos);
					break;
				default:
					throw new UnknownMsgPackExtentionTypeException(extType);
			}
		}

		private void DecrementClosingTokenCounter()
		{
			if (this.closingTokens.Count > 0)
			{
				var closingToken = this.closingTokens.Pop();
				closingToken.Counter--;
				this.closingTokens.Push(closingToken);
			}
		}
	}
}
