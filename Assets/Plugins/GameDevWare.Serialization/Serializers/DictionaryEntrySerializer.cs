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

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization.Serializers
{
	public sealed class DictionaryEntrySerializer : TypeSerializer
	{
		public override Type SerializedType { get { return typeof(DictionaryEntry); } }

		public override object Deserialize(IJsonReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			if (reader.Token == JsonToken.Null)
				return null;

			var entry = new DictionaryEntry();
			reader.ReadObjectBegin();
			while (reader.Token != JsonToken.EndOfObject)
			{
				var memberName = reader.ReadMember();
				if (memberName == "Key")
					entry.Key = reader.ReadValue(typeof(object));
				else if (memberName == "Value")
					entry.Key = reader.ReadValue(typeof(object));
				else
					reader.ReadValue(typeof(object));
			}
			reader.ReadObjectEnd(advance: false);
			return entry;
		}

		public override void Serialize(IJsonWriter writer, object value)
		{
			if (writer == null) throw new ArgumentNullException("writer");

			if (value == null)
			{
				writer.WriteNull();
				return;
			}

			var entry = (DictionaryEntry)value;
			writer.WriteObjectBegin(2);
			writer.WriteMember("Key");
			writer.WriteValue(entry.Key, typeof(object));
			writer.WriteMember("Value");
			writer.WriteValue(entry.Value, typeof(object));
			writer.WriteObjectEnd();
		}
	}
}
