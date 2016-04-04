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
using Serialization.Json.Exceptions;

namespace Serialization.Json
{
	public static class JsonReaderExtentions
	{
		public static void ReadArrayBegin(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			if (reader.Token != JsonToken.BeginArray)
				throw new UnexpectedToken(reader, JsonToken.BeginArray);
			if (reader.IsEndOfStream())
				throw new UnexpectedToken(reader, JsonToken.EndOfArray);

			if (advance)
				reader.NextToken();
		}

		public static void ReadArrayEnd(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			if (reader.Token != JsonToken.EndOfArray)
				throw new UnexpectedToken(reader, JsonToken.EndOfArray);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();
		}

		public static void ReadObjectBegin(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			if (reader.Token != JsonToken.BeginObject)
				throw new UnexpectedToken(reader, JsonToken.BeginObject);
			if (reader.IsEndOfStream())
				throw new UnexpectedToken(reader, JsonToken.EndOfObject);

			if (advance)
				reader.NextToken();
		}

		public static void ReadObjectEnd(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");
			if (reader.Token != JsonToken.EndOfObject)
				throw new UnexpectedToken(reader, JsonToken.EndOfObject);


			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();
		}

		public static string ReadMember(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			if (reader.Token != JsonToken.Member)
				throw new UnexpectedToken(reader, JsonToken.Member);

			var memberName = (string) reader.RawValue;

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return memberName;
		}

		public static Byte ReadByte(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(Byte);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsByte;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static Byte? ReadByteOrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(Byte?);
			if (reader.Token == JsonToken.Null)
			{
				value = null;
				goto next;
			}
			else if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsByte;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			next:
			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static SByte ReadSByte(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(SByte);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsSByte;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static SByte? ReadSByteOrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(SByte?);
			if (reader.Token == JsonToken.Null)
			{
				value = null;
				goto next;
			}
			else if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsSByte;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			next:
			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static Int16 ReadInt16(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(Int16);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsInt16;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static Int16? ReadInt16OrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(Int16?);
			if (reader.Token == JsonToken.Null)
			{
				value = null;
				goto next;
			}
			else if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsInt16;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			next:
			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static Int32 ReadInt32(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(Int32);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsInt32;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static Int32? ReadInt32OrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(Int32?);
			if (reader.Token == JsonToken.Null)
			{
				value = null;
				goto next;
			}
			else if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsInt32;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			next:
			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static Int64 ReadInt64(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(Int64);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsInt64;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static Int64? ReadInt64OrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(Int64?);
			if (reader.Token == JsonToken.Null)
			{
				value = null;
				goto next;
			}
			else if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsInt64;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			next:
			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static UInt16 ReadUInt16(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(UInt16);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsUInt16;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static UInt16? ReadUInt16OrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(UInt16?);
			if (reader.Token == JsonToken.Null)
			{
				value = null;
				goto next;
			}
			else if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsUInt16;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			next:
			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static UInt32 ReadUInt32(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(UInt32);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsUInt32;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static UInt32? ReadUInt32OrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(UInt32?);
			if (reader.Token == JsonToken.Null)
			{
				value = null;
				goto next;
			}
			else if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsUInt32;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			next:
			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static UInt64 ReadUInt64(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(UInt64);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsUInt64;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static UInt64? ReadUInt64OrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(UInt64?);
			if (reader.Token == JsonToken.Null)
			{
				value = null;
				goto next;
			}
			else if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsUInt64;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			next:
			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static Single ReadSingle(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(Single);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsSingle;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static Single? ReadSingleOrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(Single?);
			if (reader.Token == JsonToken.Null)
			{
				value = null;
				goto next;
			}
			else if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsSingle;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			next:
			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static Double ReadDouble(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(Double);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsDouble;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static Double? ReadDoubleOrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(Double?);
			if (reader.Token == JsonToken.Null)
			{
				value = null;
				goto next;
			}
			else if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsDouble;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			next:
			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static Decimal ReadDecimal(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(Decimal);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsDecimal;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static Decimal? ReadDecimalOrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(Decimal?);
			if (reader.Token == JsonToken.Null)
			{
				value = null;
				goto next;
			}
			else if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsDecimal;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			next:
			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static Boolean ReadBoolean(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(Boolean);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Boolean)
				value = reader.Value.AsBoolean;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Boolean);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static Boolean? ReadBooleanOrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(Boolean?);
			if (reader.Token == JsonToken.Null)
			{
				value = null;
				goto next;
			}
			else if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Boolean)
				value = reader.Value.AsBoolean;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Boolean);
			next:
			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static DateTime ReadDateTime(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(DateTime);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number || reader.Token == JsonToken.DateTime)
				value = reader.Value.AsDateTime;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number, JsonToken.DateTime);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static DateTime? ReadDateTimeOrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(DateTime?);
			if (reader.Token == JsonToken.Null)
			{
				value = null;
				goto next;
			}
			else if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number ||
			         reader.Token == JsonToken.DateTime)
				value = reader.Value.AsDateTime;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number, JsonToken.DateTime);
			next:
			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static string ReadString(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var stringValue = default(string);

			if (reader.Token == JsonToken.Null)
				goto next;

			if (reader.Token != JsonToken.StringLiteral)
				throw new UnexpectedToken(reader, JsonToken.StringLiteral);

			stringValue = Convert.ToString(reader.RawValue, reader.Context.Format);

			next:
			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return stringValue;
		}

		public static void ReadNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			if (reader.Token != JsonToken.Null)
				throw new UnexpectedToken(reader, JsonToken.Null);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();
		}

		public static object ReadValue(this IJsonReader reader, Type valueType, bool advance = true)
		{
			if (reader == null) throw new ArgumentNullException("reader");

			// try guess type
			if (valueType == typeof (object) && reader.Token != JsonToken.BeginObject)
				valueType = reader.Value.ProbableType;

			if (valueType != null && valueType.IsInstantiationOf(typeof (Nullable<>)))
			{
				if (reader.Token == JsonToken.Null)
					return null;
				
				valueType = valueType.GetGenericArguments()[0];
			}

			var serializer = reader.Context.GetSerializerForType(valueType);
			var value = serializer.Deserialize(reader);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static void ThrowUnexpectedToken(this IJsonReader reader, params JsonToken[] expectedTokens)
		{
			throw new UnexpectedToken(reader, expectedTokens);
		}

		public static void ThrowIfEndOfStream(this IJsonReader reader, JsonToken expectedToken)
		{
			if (reader.IsEndOfStream())
				throw new UnexpectedToken(reader, expectedToken);
		}

		public static void ThrowStackImbalance(IJsonReader reader)
		{
			throw new Exception("stack imbalance");
		}

		public static void ThrowIfMemberNameIsEmpty(IJsonReader reader, string memberName)
		{
			if (string.IsNullOrEmpty(memberName))
				throw new EmptyMemberName(reader);
		}
	}
}