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
using System.Linq;
using GameDevWare.Serialization.Exceptions;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization.Serializers
{
	public sealed class ArraySerializer : TypeSerializer
	{
		private Type arrayType;
		private Type elementType;

		public override Type SerializedType { get { return this.arrayType; } }

		public ArraySerializer(Type enumerableType)
		{
			if (enumerableType == null)
				throw new ArgumentNullException("enumerableType");


			arrayType = enumerableType;
			elementType = GetElementType(arrayType);

			if (elementType == null) throw new TypeContractViolation(this.GetType(), "be enumerable");
		}

		public override object Deserialize(IJsonReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			if (reader.Token == JsonToken.Null)
				return null;

			var container = new ArrayList();
			if (reader.Token != JsonToken.BeginArray)
				throw new UnexpectedToken(reader, JsonToken.BeginArray);

			reader.Context.Hierarchy.Push(container);
			while (reader.NextToken() && reader.Token != JsonToken.EndOfArray)
			{
				var value = reader.ReadValue(elementType, false);
				container.Add(value);
			}
			reader.Context.Hierarchy.Pop();

			if (reader.IsEndOfStream())
				throw new UnexpectedToken(reader, JsonToken.EndOfArray);

			var array = container.ToArray(elementType);
			if (arrayType.IsArray)
				return array;
			else
				return Activator.CreateInstance(arrayType, array);
		}

		public override void Serialize(IJsonWriter writer, object value)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			var size = 0;
			if (value is ICollection)
				size = ((ICollection)value).Count;
			else
				size = ((IEnumerable)value).Cast<object>().Count();

			writer.WriteArrayBegin(size);
			foreach (var item in (IEnumerable)value)
				writer.WriteValue(item, elementType);
			writer.WriteArrayEnd();
		}

		private Type GetElementType(Type arrayType)
		{
			if (arrayType == null)
				throw new ArgumentNullException("arrayType");


			var elementType = (Type)null;
			if (arrayType.IsArray)
			{
				elementType = arrayType.GetElementType();
				return elementType;
			}

			if (arrayType.IsInstantiationOf(typeof(IEnumerable<>)))
			{
				if (arrayType.HasMultipleInstantiations(typeof(IEnumerable<>)))
					throw new TypeContractViolation(this.GetType(), "have only one generic IEnumerable interface");

				elementType = arrayType.GetInstantiationArguments(typeof(IEnumerable<>))[0];
			}

			if (elementType == null && typeof(IEnumerable).IsAssignableFrom(arrayType))
				elementType = typeof(object);
			else if (elementType == null)
				throw new TypeContractViolation(this.GetType(), "be enumerable");

			return elementType;
		}

		public override string ToString()
		{
			return string.Format("array of {1}, {0}", arrayType, elementType);
		}
	}
}
