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
	public sealed class EnumNumberSerializer : TypeSerializer
	{
		private readonly Type enumType;
		private readonly Type enumBaseType;

		public override Type SerializedType { get { return this.enumType; } }

		public EnumNumberSerializer(Type enumType)
		{
			if (enumType == null) throw new ArgumentNullException("enumType");
			if (!enumType.IsEnum) throw new TypeContractViolation(this.GetType(), "be a Enum");

			this.enumType = enumType;
			this.enumBaseType = Enum.GetUnderlyingType(enumType);
		}

		public override object Deserialize(IJsonReader reader)
		{
			if (reader == null) throw new ArgumentNullException("reader");

			if (reader.Token == JsonToken.StringLiteral)
				return Enum.Parse(this.enumType, reader.ReadString(false), true);
			else if (reader.Token == JsonToken.Number)
				return Enum.ToObject(this.enumType, reader.ReadValue(this.enumBaseType, false));
			else
				throw new UnexpectedToken(reader, JsonToken.Number | JsonToken.StringLiteral);
		}

		public override void Serialize(IJsonWriter writer, object value)
		{
			if (writer == null) throw new ArgumentNullException("writer");
			if (value == null) throw new ArgumentNullException("value");

			writer.WriteValue(Convert.ChangeType(value, this.enumBaseType), this.enumBaseType);
		}
	}
}
