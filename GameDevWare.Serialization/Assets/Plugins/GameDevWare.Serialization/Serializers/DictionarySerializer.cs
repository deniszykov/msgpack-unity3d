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
using Serialization.Json.Exceptions;

namespace Serialization.Json.Serializers
{
	public sealed class DictionarySerializer : TypeSerializer
	{
		private readonly Type dictionaryType;
		private readonly Type keyType;
		private readonly Type valueType;
		private readonly bool isStringKeyType;

		public override Type SerializedType
		{
			get { return dictionaryType; }
		}

		public DictionarySerializer(Type dictionaryType)
		{
			if (dictionaryType == null)
				throw new ArgumentNullException("dictionaryType");

			this.dictionaryType = dictionaryType;
			this.keyType = typeof (object);
			this.valueType = typeof (object);

			if (dictionaryType.HasMultipleInstantiations(typeof (IDictionary<,>)))
				throw new TypeContractViolation(this.GetType(), "have only one generic IDictionary interface");

			if (dictionaryType.IsInstantiationOf(typeof (IDictionary<,>)))
			{
				var genArgs = dictionaryType.GetInstantiationArguments(typeof (IDictionary<,>));
				keyType = genArgs[0];
				valueType = genArgs[1];
			}
			else if (typeof (IDictionary).IsAssignableFrom(dictionaryType))
				return; // do nothink
			else
				throw new TypeContractViolation(this.GetType(), "be dictionary");

			this.isStringKeyType = IsStringKeyType(keyType);
		}

		public override object Deserialize(IJsonReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			if (reader.Token == JsonToken.Null)
				return null;

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
							throw new SerializationException(
								$"Failed to read '{keyType.Name}' key of dictionary: {e.Message}\r\nMore detailed information in inner exception.", e);
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
							throw new SerializationException(
								$"Failed to read '{valueType.Name}' value for key '{key}' in dictionary: {e.Message}\r\nMore detailed information in inner exception.", e);
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
							throw new SerializationException(
								$"Failed to read '{keyType.Name}' key of dictionary: {e.Message}\r\nMore detailed information in inner exception.", e);
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
							throw new SerializationException(
								$"Failed to read '{valueType.Name}' value for key '{key}' in dictionary: {e.Message}\r\nMore detailed information in inner exception.", e);
						}


						container.Add(new DictionaryEntry(key, value));
					}
				}
				else
				{
					throw new UnexpectedToken(reader, JsonToken.BeginObject, JsonToken.BeginArray);
				}

				var dictionary = (IDictionary) Activator.CreateInstance(this.dictionaryType);
				foreach (var kv in container)
				{
					var key = kv.Key;
					var value = kv.Value;

					if (this.keyType.IsEnum)
						key = Enum.Parse(this.keyType, Convert.ToString(key));
					else if (this.keyType != typeof (string) && this.keyType != typeof (object))
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

		public override void Serialize(IJsonWriter writer, object dictionary)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			if (dictionary == null)
			{
				writer.WriteNull();
				return;
			}

			// serialize generic dictionary
			if (!(dictionary is IDictionary))
				throw new TypeContractViolation(this.GetType(), "be dictionary");

			writer.Context.Hierarchy.Push(dictionary);
			// object
			if (isStringKeyType)
			{
				writer.WriteObjectBegin((dictionary as IDictionary).Count);
				foreach (DictionaryEntry pair in (dictionary as IDictionary))
				{
					var keyStr = default(string);
					if (pair.Key is Single)
						keyStr = ((Single) pair.Key).ToString("R", writer.Context.Format);
					else if (pair.Key is Double)
						keyStr = ((Double) pair.Key).ToString("R", writer.Context.Format);
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
				writer.WriteArrayBegin((dictionary as IDictionary).Count);
				foreach (DictionaryEntry pair in (dictionary as IDictionary))
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

			return keyType == typeof (String);
			//keyType == typeof(Byte) ||
			//keyType == typeof(SByte) ||
			//keyType == typeof(Int16) ||
			//keyType == typeof(UInt16) ||
			//keyType == typeof(Int32) ||
			//keyType == typeof(UInt32) ||
			//keyType == typeof(Int64) ||
			//keyType == typeof(UInt64) ||
			//keyType == typeof(Single) ||
			//keyType == typeof(Double) ||
			//keyType == typeof(Decimal) ||
			//keyType == typeof(String) ||
			//keyType == typeof(Char) ||
			//keyType.IsEnum;
		}

		public override string ToString()
		{
			return string.Format("dictionary of {1}:{2}, {0}", dictionaryType, keyType, valueType);
		}
	}
}