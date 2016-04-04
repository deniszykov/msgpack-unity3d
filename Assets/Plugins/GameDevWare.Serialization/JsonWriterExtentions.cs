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
using Serialization.Json.Serializers;

namespace Serialization.Json
{
	public static class JsonWriterExtentions
	{
		public static void WriteMember(this IJsonWriter writer, string memberName)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
			if (memberName == null)
				throw new ArgumentNullException("memberName");


			writer.Write((JsonMember)memberName);
		}

		public static void WriteNumber(this IJsonWriter writer, byte number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			writer.Write(number);
		}

		public static void WriteNumber(this IJsonWriter writer, sbyte number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			writer.Write(number);
		}

		public static void WriteNumber(this IJsonWriter writer, short number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			writer.Write(number);
		}

		public static void WriteNumber(this IJsonWriter writer, ushort number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			writer.Write(number);
		}

		public static void WriteNumber(this IJsonWriter writer, int number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			writer.Write(number);
		}

		public static void WriteNumber(this IJsonWriter writer, uint number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			writer.Write(number);
		}

		public static void WriteNumber(this IJsonWriter writer, long number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			writer.Write(number);
		}

		public static void WriteNumber(this IJsonWriter writer, ulong number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			writer.Write(number);
		}

		public static void WriteNumber(this IJsonWriter writer, float number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			writer.Write(number);
		}

		public static void WriteNumber(this IJsonWriter writer, double number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			writer.Write(number);
		}

		public static void WriteNumber(this IJsonWriter writer, decimal number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			writer.Write(number);
		}

		public static void WriteDateTime(this IJsonWriter writer, DateTime date)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			writer.Write(date);
		}

		public static void WriteBoolean(this IJsonWriter writer, bool value)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			writer.Write(value);
		}

		public static void WriteNumber(this IJsonWriter writer, byte? number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			if (number == null)
				writer.WriteNull();
			else
				writer.Write(number.Value);
		}

		public static void WriteNumber(this IJsonWriter writer, sbyte? number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			if (number == null)
				writer.WriteNull();
			else
				writer.Write(number.Value);
		}

		public static void WriteNumber(this IJsonWriter writer, short? number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			if (number == null)
				writer.WriteNull();
			else
				writer.Write(number.Value);
		}

		public static void WriteNumber(this IJsonWriter writer, ushort? number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			if (number == null)
				writer.WriteNull();
			else
				writer.Write(number.Value);
		}

		public static void WriteNumber(this IJsonWriter writer, int? number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			if (number == null)
				writer.WriteNull();
			else
				writer.Write(number.Value);
		}

		public static void WriteNumber(this IJsonWriter writer, uint? number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			if (number == null)
				writer.WriteNull();
			else
				writer.Write(number.Value);
		}

		public static void WriteNumber(this IJsonWriter writer, long? number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			if (number == null)
				writer.WriteNull();
			else
				writer.Write(number.Value);
		}

		public static void WriteNumber(this IJsonWriter writer, ulong? number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			if (number == null)
				writer.WriteNull();
			else
				writer.Write(number.Value);
		}

		public static void WriteNumber(this IJsonWriter writer, float? number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			if (number == null)
				writer.WriteNull();
			else
				writer.Write(number.Value);
		}

		public static void WriteNumber(this IJsonWriter writer, double? number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			if (number == null)
				writer.WriteNull();
			else
				writer.Write(number.Value);
		}

		public static void WriteNumber(this IJsonWriter writer, decimal? number)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			if (number == null)
				writer.WriteNull();
			else
				writer.Write(number.Value);
		}

		public static void WriteDateTime(this IJsonWriter writer, DateTime? date)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			if (date == null)
				writer.WriteNull();
			else
				writer.Write(date.Value);
		}

		public static void WriteBoolean(this IJsonWriter writer, bool? value)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			if (value == null)
				writer.WriteNull();
			else
				writer.Write(value.Value);
		}

		public static void WriteString(this IJsonWriter writer, string literal)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			if (literal == null)
				writer.WriteNull();
			else
				writer.Write(literal);
		}

		public static void WriteValue(this IJsonWriter writer, object value, Type valueType)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			if (value == null)
			{
				writer.WriteNull();
				return;
			}

			var actualValueType = value.GetType();
			var serializer = writer.Context.GetSerializerForType(actualValueType);
			var objectSerializer = serializer as ObjectSerializer;

			if (objectSerializer != null && valueType == actualValueType)
				objectSerializer.SuppressTypeInformation = true; // no need to write type information on when type is obvious

			serializer.Serialize(writer, value);
		}

		public static void ThrowStackImbalance(IJsonWriter reader)
		{
			throw new Exception("stack imbalance");
		}
	}
}