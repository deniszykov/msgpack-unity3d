/* 
	Copyright (c) 2016 Denis Zykov, GameDevWare.com

	This a part of "Json & MessagePack Serialization" Unity Asset - https://www.assetstore.unity3d.com/#!/content/59918

	THIS SOFTWARE IS DISTRIBUTED "AS-IS" WITHOUT ANY WARRANTIES, CONDITIONS AND 
	REPRESENTATIONS WHETHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION THE 
	IMPLIED WARRANTIES AND CONDITIONS OF MERCHANTABILITY, MERCHANTABLE QUALITY, 
	FITNESS FOR A PARTICULAR PURPOSE, DURABILITY, NON-INFRINGEMENT, PERFORMANCE 
	AND THOSE ARISING BY STATUTE OR FROM CUSTOM OR USAGE OF TRADE OR COURSE OF DEALING.
	
	This source code is distributed via Unity Asset Store, 
	to use it in your project you should accept Terms of Service and EULA 
	https://unity3d.com/ru/legal/as_terms
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization.Serializers
{
	public sealed class ArraySerializer : TypeSerializer
	{
		private readonly Type arrayType;
		private readonly Type elementType;

		public override Type SerializedType { get { return this.arrayType; } }

		public ArraySerializer(Type enumerableType)
		{
			if (enumerableType == null) throw new ArgumentNullException("enumerableType");

			arrayType = enumerableType;
			elementType = GetElementType(arrayType);

			if (elementType == null) JsonSerializationException.TypeIsNotValid(this.GetType(), "be enumerable");
		}

		public override object Deserialize(IJsonReader reader)
		{
			if (reader == null) throw new ArgumentNullException("reader");

			if (reader.Token == JsonToken.Null)
				return null;

			var container = new ArrayList();
			if (reader.Token != JsonToken.BeginArray)
				throw JsonSerializationException.UnexpectedToken(reader, JsonToken.BeginArray);

			reader.Context.Hierarchy.Push(container);
			while (reader.NextToken() && reader.Token != JsonToken.EndOfArray)
			{
				var value = reader.ReadValue(elementType, false);
				container.Add(value);
			}
			reader.Context.Hierarchy.Pop();

			if (reader.IsEndOfStream())
				throw JsonSerializationException.UnexpectedToken(reader, JsonToken.EndOfArray);

			var array = container.ToArray(elementType);
			if (arrayType.IsArray)
				return array;
			else
				return Activator.CreateInstance(arrayType, array);
		}

		public override void Serialize(IJsonWriter writer, object value)
		{
			if (writer == null) throw new ArgumentNullException("writer");
			if (value == null) throw new ArgumentNullException("value");

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
			if (arrayType == null) throw new ArgumentNullException("arrayType");


			var elementType = (Type)null;
			if (arrayType.IsArray)
			{
				elementType = arrayType.GetElementType();
				return elementType;
			}

			if (arrayType.IsInstantiationOf(typeof(IEnumerable<>)))
			{
				if (arrayType.HasMultipleInstantiations(typeof(IEnumerable<>)))
					throw JsonSerializationException.TypeIsNotValid(this.GetType(), "have only one generic IEnumerable interface");

				elementType = arrayType.GetInstantiationArguments(typeof(IEnumerable<>))[0];
			}

			if (elementType == null && typeof(IEnumerable).IsAssignableFrom(arrayType))
				elementType = typeof(object);
			else if (elementType == null)
				throw JsonSerializationException.TypeIsNotValid(this.GetType(), "be enumerable");

			return elementType;
		}

		public override string ToString()
		{
			return string.Format("array of {1}, {0}", arrayType, elementType);
		}
	}
}
