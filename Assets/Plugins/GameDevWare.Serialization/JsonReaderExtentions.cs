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
using GameDevWare.Serialization.Exceptions;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization
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

			var memberName = (string)reader.RawValue;

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return memberName;
		}

		public static byte ReadByte(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(byte);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsByte;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}
		public static byte? ReadByteOrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(byte?);
			switch (reader.Token)
			{
				case JsonToken.Null:
					value = null;
					break;
				case JsonToken.StringLiteral:
				case JsonToken.Number:
					value = reader.Value.AsByte;
					break;
				default:
					throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			}

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static sbyte ReadSByte(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(sbyte);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsSByte;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}
		public static sbyte? ReadSByteOrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(sbyte?);
			switch (reader.Token)
			{
				case JsonToken.Null:
					value = null;
					break;
				case JsonToken.StringLiteral:
				case JsonToken.Number:
					value = reader.Value.AsSByte;
					break;
				default:
					throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			}

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static short ReadInt16(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(short);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsInt16;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}
		public static short? ReadInt16OrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(short?);
			switch (reader.Token)
			{
				case JsonToken.Null:
					value = null;
					break;
				case JsonToken.StringLiteral:
				case JsonToken.Number:
					value = reader.Value.AsInt16;
					break;
				default:
					throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			}

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static int ReadInt32(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(int);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsInt32;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}
		public static int? ReadInt32OrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(int?);
			switch (reader.Token)
			{
				case JsonToken.Null:
					value = null;
					break;
				case JsonToken.StringLiteral:
				case JsonToken.Number:
					value = reader.Value.AsInt32;
					break;
				default:
					throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			}

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static long ReadInt64(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(long);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsInt64;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}
		public static long? ReadInt64OrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(long?);
			switch (reader.Token)
			{
				case JsonToken.Null:
					value = null;
					break;
				case JsonToken.StringLiteral:
				case JsonToken.Number:
					value = reader.Value.AsInt64;
					break;
				default:
					throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			}
			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static ushort ReadUInt16(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(ushort);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsUInt16;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}
		public static ushort? ReadUInt16OrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(ushort?);
			switch (reader.Token)
			{
				case JsonToken.Null:
					value = null;
					break;
				case JsonToken.StringLiteral:
				case JsonToken.Number:
					value = reader.Value.AsUInt16;
					break;
				default:
					throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			}

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static uint ReadUInt32(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(uint);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsUInt32;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}
		public static uint? ReadUInt32OrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(uint?);
			switch (reader.Token)
			{
				case JsonToken.Null:
					value = null;
					break;
				case JsonToken.StringLiteral:
				case JsonToken.Number:
					value = reader.Value.AsUInt32;
					break;
				default:
					throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			}

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static ulong ReadUInt64(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(ulong);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsUInt64;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}
		public static ulong? ReadUInt64OrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(ulong?);
			switch (reader.Token)
			{
				case JsonToken.Null:
					value = null;
					break;
				case JsonToken.StringLiteral:
				case JsonToken.Number:
					value = reader.Value.AsUInt64;
					break;
				default:
					throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			}

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static float ReadSingle(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(float);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsSingle;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}
		public static float? ReadSingleOrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(float?);
			switch (reader.Token)
			{
				case JsonToken.Null:
					value = null;
					break;
				case JsonToken.StringLiteral:
				case JsonToken.Number:
					value = reader.Value.AsSingle;
					break;
				default:
					throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			}

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static double ReadDouble(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(double);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsDouble;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}
		public static double? ReadDoubleOrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(double?);
			switch (reader.Token)
			{
				case JsonToken.Null:
					value = null;
					break;
				case JsonToken.StringLiteral:
				case JsonToken.Number:
					value = reader.Value.AsDouble;
					break;
				default:
					throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			}

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static decimal ReadDecimal(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(decimal);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Number)
				value = reader.Value.AsDecimal;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}
		public static decimal? ReadDecimalOrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(decimal?);
			switch (reader.Token)
			{
				case JsonToken.Null:
					value = null;
					break;
				case JsonToken.StringLiteral:
				case JsonToken.Number:
					value = reader.Value.AsDecimal;
					break;
				default:
					throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number);
			}

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static bool ReadBoolean(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(bool);
			if (reader.Token == JsonToken.StringLiteral || reader.Token == JsonToken.Boolean)
				value = reader.Value.AsBoolean;
			else
				throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Boolean);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}
		public static bool? ReadBooleanOrNull(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = default(bool?);
			switch (reader.Token)
			{
				case JsonToken.Null:
					value = null;
					break;
				case JsonToken.StringLiteral:
				case JsonToken.Boolean:
					value = reader.Value.AsBoolean;
					break;
				default:
					throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Boolean);
			}

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
			switch (reader.Token)
			{
				case JsonToken.Null:
					value = null;
					break;
				case JsonToken.StringLiteral:
				case JsonToken.Number:
				case JsonToken.DateTime:
					value = reader.Value.AsDateTime;
					break;
				default:
					throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number, JsonToken.DateTime);
			}

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		public static string ReadString(this IJsonReader reader, bool advance = true)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			var stringValue = default(string);
			switch (reader.Token)
			{
				case JsonToken.Null:
					break;
				case JsonToken.StringLiteral:
				case JsonToken.Number:
				case JsonToken.DateTime:
				case JsonToken.Boolean:
					stringValue = Convert.ToString(reader.RawValue, reader.Context.Format);
					break;
				default:
					throw new UnexpectedToken(reader, JsonToken.StringLiteral, JsonToken.Number, JsonToken.DateTime, JsonToken.Boolean);
			}

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
			if (valueType == typeof(object) && reader.Token != JsonToken.BeginObject)
				valueType = reader.Value.ProbableType;

			if (valueType != null && valueType.IsInstantiationOf(typeof(Nullable<>)))
			{
				if (reader.Token == JsonToken.Null)
					return null;

				valueType = valueType.GetGenericArguments()[0];
			}

			if (valueType == null)
				valueType = typeof(object);

			if (reader.Token == JsonToken.Null)
			{
				if (valueType.IsValueType)
					reader.ThrowUnexpectedToken(JsonToken.BeginArray, JsonToken.BeginObject, JsonToken.StringLiteral, JsonToken.Number, JsonToken.Boolean, JsonToken.DateTime);
				return null;
			}

			var serializer = reader.Context.GetSerializerForType(valueType);
			var value = serializer.Deserialize(reader);

			if (!reader.IsEndOfStream() && advance)
				reader.NextToken();

			return value;
		}

		internal static void ThrowUnexpectedToken(this IJsonReader reader, params JsonToken[] expectedTokens)
		{
			throw new UnexpectedToken(reader, expectedTokens);
		}
		internal static void ThrowIfEndOfStream(this IJsonReader reader, JsonToken expectedToken)
		{
			if (reader.IsEndOfStream())
				throw new UnexpectedToken(reader, expectedToken);
		}
		internal static void ThrowIfMemberNameIsEmpty(this IJsonReader reader, string memberName)
		{
			if (string.IsNullOrEmpty(memberName))
				throw new EmptyMemberName(reader);
		}
	}
}
