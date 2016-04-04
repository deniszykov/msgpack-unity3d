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
namespace GameDevWare.Serialization.Serializers
{
	public sealed class PrimitiveSerializer : TypeSerializer
	{
		private readonly Type primitiveType;
		private readonly TypeCode primitiveTypeCode;

		public override Type SerializedType { get { return this.primitiveType; } }

		public PrimitiveSerializer(Type primitiveType)
		{
			if (primitiveType == null) throw new ArgumentNullException("primitiveType");

			if (primitiveType.IsGenericType && primitiveType.GetGenericTypeDefinition() == typeof(Nullable<>))
				throw new TypeContractViolation(typeof(PrimitiveSerializer), "can't be nullable type");

			this.primitiveType = primitiveType;
			this.primitiveTypeCode = Type.GetTypeCode(primitiveType);

			if (this.primitiveTypeCode == TypeCode.Object || this.primitiveTypeCode == TypeCode.Empty ||
				this.primitiveTypeCode == TypeCode.DBNull)
				throw new TypeContractViolation(this.GetType(), "be a primitive type");
		}

		public override object Deserialize(IJsonReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			if (reader.Token == JsonToken.Null)
			{
				if (this.primitiveTypeCode == TypeCode.String)
					return null;

				throw new UnexpectedToken(reader,
					JsonToken.Boolean | JsonToken.DateTime | JsonToken.Null | JsonToken.Number | JsonToken.StringLiteral);
			}

			var value = default(object);
			switch (primitiveTypeCode)
			{
				case TypeCode.Boolean:
					value = reader.ReadBoolean(false);
					break;
				case TypeCode.Byte:
					value = reader.ReadByte(false);
					break;
				case TypeCode.DateTime:
					value = reader.ReadDateTime(false);
					break;
				case TypeCode.Decimal:
					value = reader.ReadDecimal(false);
					break;
				case TypeCode.Double:
					value = reader.ReadDouble(false);
					break;
				case TypeCode.Int16:
					value = reader.ReadInt16(false);
					break;
				case TypeCode.Int32:
					value = reader.ReadInt32(false);
					break;
				case TypeCode.Int64:
					value = reader.ReadInt64(false);
					break;
				case TypeCode.SByte:
					value = reader.ReadSByte(false);
					break;
				case TypeCode.Single:
					value = reader.ReadSingle(false);
					break;
				case TypeCode.UInt16:
					value = reader.ReadUInt16(false);
					break;
				case TypeCode.UInt32:
					value = reader.ReadUInt32(false);
					break;
				case TypeCode.UInt64:
					value = reader.ReadUInt64(false);
					break;
				default:
					var valueStr = reader.ReadString(false);
					value = Convert.ChangeType(valueStr, this.primitiveType, reader.Context.Format);
					break;
			}
			return value;
		}

		public override void Serialize(IJsonWriter writer, object value)
		{
			if (writer == null) throw new ArgumentNullException("writer");
			if (value == null) throw new ArgumentNullException("value");

			if (value == null && primitiveTypeCode == TypeCode.String)
			{
				writer.WriteNull();
				return;
			}

			switch (primitiveTypeCode)
			{
				case TypeCode.Boolean:
					writer.WriteBoolean((bool)value);
					break;
				case TypeCode.Byte:
					writer.WriteNumber((byte)value);
					break;
				case TypeCode.DateTime:
					writer.WriteDateTime((DateTime)value);
					break;
				case TypeCode.Decimal:
					writer.WriteNumber((decimal)value);
					break;
				case TypeCode.Double:
					writer.WriteNumber((double)value);
					break;
				case TypeCode.Int16:
					writer.WriteNumber((short)value);
					break;
				case TypeCode.Int32:
					writer.WriteNumber((int)value);
					break;
				case TypeCode.Int64:
					writer.WriteNumber((long)value);
					break;
				case TypeCode.SByte:
					writer.WriteNumber((sbyte)value);
					break;
				case TypeCode.Single:
					writer.WriteNumber((float)value);
					break;
				case TypeCode.UInt16:
					writer.WriteNumber((ushort)value);
					break;
				case TypeCode.UInt32:
					writer.WriteNumber((uint)value);
					break;
				case TypeCode.UInt64:
					writer.WriteNumber((ulong)value);
					break;
				default:
					var valueStr = default(string);

					if (value is IFormattable)
						valueStr = (string)Convert.ChangeType(value, typeof(string), writer.Context.Format);
					else
						valueStr = value.ToString();

					writer.WriteString(valueStr);
					break;
			}
		}
	}
}
