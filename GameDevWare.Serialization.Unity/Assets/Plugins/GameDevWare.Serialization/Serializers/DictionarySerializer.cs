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
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization.Serializers
{
	public sealed class DictionarySerializer : TypeSerializer
	{
		private readonly Type dictionaryType;
		private readonly Type instantiatedDictionaryType;
		private readonly Type keyType;
		private readonly Type valueType;
		private readonly bool isStringKeyType;

		public override Type SerializedType { get { return dictionaryType; } }

		public DictionarySerializer(Type dictionaryType)
		{
			if (dictionaryType == null)
				throw new ArgumentNullException("dictionaryType");

			this.dictionaryType =
			this.instantiatedDictionaryType = dictionaryType;
			this.keyType = typeof(object);
			this.valueType = typeof(object);

			if (dictionaryType.HasMultipleInstantiations(typeof(IDictionary<,>)))
				throw JsonSerializationException.TypeIsNotValid(this.GetType(), "have only one generic IDictionary interface");

			if (dictionaryType.IsInstantiationOf(typeof(IDictionary<,>)))
			{
				var genArgs = dictionaryType.GetInstantiationArguments(typeof(IDictionary<,>));
				keyType = genArgs[0];
				valueType = genArgs[1];

				if (dictionaryType.IsInterface && dictionaryType.IsGenericType && dictionaryType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
					this.instantiatedDictionaryType = typeof(Dictionary<,>).MakeGenericType(genArgs);
			}
			else if (typeof(IDictionary).IsAssignableFrom(dictionaryType))
			{
				if (dictionaryType == typeof(IDictionary))
					this.instantiatedDictionaryType = typeof(Hashtable);
			}
			else
			{
				throw JsonSerializationException.TypeIsNotValid(this.GetType(), "be dictionary");
			}

			this.isStringKeyType = keyType == typeof(string);
		}

		public override object Deserialize(IJsonReader reader)
		{
			if (reader == null) throw new ArgumentNullException("reader");

			var container = new List<DictionaryEntry>();
			reader.Context.Hierarchy.Push(container);
			try
			{
				if (reader.Token == JsonToken.BeginArray)
				{
					while (reader.NextToken() && reader.Token != JsonToken.EndOfArray)
					{
						if (reader.Token != JsonToken.BeginArray || !reader.NextToken())
							throw JsonSerializationException.UnexpectedToken(reader, JsonToken.BeginArray);

						object key = null;
						object value = null;

						try
						{
							key = reader.ReadValue(keyType, false);
						}
						catch (Exception e)
						{
							throw new SerializationException(string.Format("Failed to read '{0}' key of dictionary: {1}\r\nMore detailed information in inner exception.", keyType.Name, e.Message), e);
						}

						if (!reader.NextToken())
							throw JsonSerializationException.UnexpectedToken(reader, JsonToken.Boolean, JsonToken.DateTime, JsonToken.Number,
								JsonToken.StringLiteral);

						try
						{
							value = reader.ReadValue(valueType, false);
						}
						catch (Exception e)
						{
							throw new SerializationException(string.Format("Failed to read '{0}' value for key '{1}' in dictionary: {2}\r\nMore detailed information in inner exception.", valueType.Name, key, e.Message), e);
						}


						container.Add(new DictionaryEntry(key, value));

						if (!reader.NextToken() || reader.Token != JsonToken.EndOfArray)
							throw JsonSerializationException.UnexpectedToken(reader, JsonToken.EndOfArray);
					}
				}
				else if (reader.Token == JsonToken.BeginObject)
				{
					while (reader.NextToken() && reader.Token != JsonToken.EndOfObject)
					{
						if (reader.Token != JsonToken.Member)
							throw JsonSerializationException.UnexpectedToken(reader, JsonToken.Member);

						string key = null;
						object value = null;

						try
						{
							key = reader.Value.AsString;
						}
						catch (Exception e)
						{
							throw new SerializationException(string.Format("Failed to read '{0}' key of dictionary: {1}\r\nMore detailed information in inner exception.", keyType.Name, e.Message), e);
						}

						if (!reader.NextToken())
							throw JsonSerializationException.UnexpectedToken(reader, JsonToken.Boolean, JsonToken.DateTime, JsonToken.Number,
								JsonToken.StringLiteral);
						try
						{
							value = reader.ReadValue(valueType, false);
						}
						catch (Exception e)
						{
							throw new SerializationException(string.Format("Failed to read '{0}' value for key '{1}' in dictionary: {2}\r\nMore detailed information in inner exception.", valueType.Name, key, e.Message), e);
						}


						container.Add(new DictionaryEntry(key, value));
					}
				}
				else
				{
					throw JsonSerializationException.UnexpectedToken(reader, JsonToken.BeginObject, JsonToken.BeginArray);
				}

				var dictionary = (IDictionary)Activator.CreateInstance(this.instantiatedDictionaryType);
				foreach (var kv in container)
				{
					var key = kv.Key;
					var value = kv.Value;

					if (key.GetType() != this.keyType && this.keyType != typeof(object))
					{
						if (this.keyType.IsEnum)
							key = Enum.Parse(this.keyType, Convert.ToString(key));
						else
							key = Convert.ChangeType(key, this.keyType);
					}

					if (dictionary.Contains(key))
						dictionary.Remove(key);

					dictionary.Add(key, value);
				}

				return dictionary;
			}
			finally
			{
				reader.Context.Hierarchy.Pop();
			}
		}
		public override void Serialize(IJsonWriter writer, object value)
		{
			if (writer == null) throw new ArgumentNullException("writer");
			if (value == null) throw new ArgumentNullException("value");

			var dictionary = (IDictionary)value;
			// ReSharper disable PossibleMultipleEnumeration
			writer.Context.Hierarchy.Push(value);
			// object
			if (isStringKeyType)
			{
				writer.WriteObjectBegin(dictionary.Count);
				foreach (DictionaryEntry pair in dictionary)
				{
					var keyStr = Convert.ToString(pair.Key, writer.Context.Format);

					// key
					writer.WriteMember(keyStr);
					// value
					writer.WriteValue(pair.Value, valueType);
				}
				writer.WriteObjectEnd();
			}
			else
			{
				writer.WriteArrayBegin(dictionary.Count);
				foreach (DictionaryEntry pair in dictionary)
				{
					writer.WriteArrayBegin(2);
					writer.WriteValue(pair.Key, keyType);
					writer.WriteValue(pair.Value, valueType);
					writer.WriteArrayEnd();
				}
				writer.WriteArrayEnd();
			}
			// ReSharper restore PossibleMultipleEnumeration

			writer.Context.Hierarchy.Pop();
		}

		public override string ToString()
		{
			return string.Format("dictionary of {1}:{2}, {0}", dictionaryType, keyType, valueType);
		}
	}
}
