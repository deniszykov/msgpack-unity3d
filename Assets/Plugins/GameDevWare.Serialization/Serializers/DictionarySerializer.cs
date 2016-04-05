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
using System.Runtime.Serialization;
using GameDevWare.Serialization.Exceptions;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization.Serializers
{
	public sealed class DictionarySerializer : TypeSerializer
	{
		private readonly Type dictionaryType;
		private readonly Type keyType;
		private readonly Type valueType;
		private readonly bool isStringKeyType;

		public override Type SerializedType { get { return dictionaryType; } }

		public DictionarySerializer(Type dictionaryType)
		{
			if (dictionaryType == null)
				throw new ArgumentNullException("dictionaryType");

			this.dictionaryType = dictionaryType;
			this.keyType = typeof(object);
			this.valueType = typeof(object);

			if (dictionaryType.HasMultipleInstantiations(typeof(IDictionary<,>)))
				throw new TypeContractViolation(this.GetType(), "have only one generic IDictionary interface");

			if (dictionaryType.IsInstantiationOf(typeof(IDictionary<,>)))
			{
				var genArgs = dictionaryType.GetInstantiationArguments(typeof(IDictionary<,>));
				keyType = genArgs[0];
				valueType = genArgs[1];
			}
			else if (typeof(IDictionary).IsAssignableFrom(dictionaryType))
				return; // do nothink
			else
				throw new TypeContractViolation(this.GetType(), "be dictionary");

			this.isStringKeyType = IsStringKeyType(keyType);
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
							throw new UnexpectedToken(reader, JsonToken.BeginArray);

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
							throw new UnexpectedToken(reader, JsonToken.Boolean, JsonToken.DateTime, JsonToken.Number,
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
							throw new UnexpectedToken(reader, JsonToken.EndOfArray);
					}
				}
				else if (reader.Token == JsonToken.BeginObject)
				{
					while (reader.NextToken() && reader.Token != JsonToken.EndOfObject)
					{
						if (reader.Token != JsonToken.Member)
							throw new UnexpectedToken(reader, JsonToken.Member);

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
							throw new UnexpectedToken(reader, JsonToken.Boolean, JsonToken.DateTime, JsonToken.Number,
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
					throw new UnexpectedToken(reader, JsonToken.BeginObject, JsonToken.BeginArray);
				}

				var dictionary = (IDictionary)Activator.CreateInstance(this.dictionaryType);
				foreach (var kv in container)
				{
					var key = kv.Key;
					var value = kv.Value;

					if (this.keyType.IsEnum)
						key = Enum.Parse(this.keyType, Convert.ToString(key));
					else if (this.keyType != typeof(string) && this.keyType != typeof(object))
						key = Convert.ChangeType(key, this.keyType);

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

			// serialize generic dictionary
			if (!(value is IDictionary))
				throw new TypeContractViolation(this.GetType(), "be dictionary");

			writer.Context.Hierarchy.Push(value);
			// object
			if (isStringKeyType)
			{
				writer.WriteObjectBegin((value as IDictionary).Count);
				foreach (DictionaryEntry pair in (value as IDictionary))
				{
					var keyStr = default(string);
					if (pair.Key is float)
						keyStr = ((float)pair.Key).ToString("R", writer.Context.Format);
					else if (pair.Key is double)
						keyStr = ((double)pair.Key).ToString("R", writer.Context.Format);
					else
						keyStr = Convert.ToString(pair.Key, writer.Context.Format);

					// key
					writer.WriteMember(keyStr);
					// value
					writer.WriteValue(pair.Value, valueType);
				}
				writer.WriteObjectEnd();
			}
			else
			{
				writer.WriteArrayBegin((value as IDictionary).Count);
				foreach (DictionaryEntry pair in (value as IDictionary))
				{
					writer.WriteArrayBegin(2);
					writer.WriteValue(pair.Key, keyType);
					writer.WriteValue(pair.Value, valueType);
					writer.WriteArrayEnd();
				}
				writer.WriteArrayEnd();
			}

			writer.Context.Hierarchy.Pop();
		}

		public static bool IsStringKeyType(Type keyType)
		{
			if (keyType == null) throw new ArgumentNullException("keyType");

			return keyType == typeof(string);
		}

		public override string ToString()
		{
			return string.Format("dictionary of {1}:{2}, {0}", dictionaryType, keyType, valueType);
		}
	}
}
