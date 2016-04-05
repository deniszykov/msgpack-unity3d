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
using System.Text;
using GameDevWare.Serialization.Exceptions;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization.Serializers
{
	public sealed class JsonStringSerializer : TypeSerializer
	{
		public override Type SerializedType { get { return typeof(JsonString); } }

		public override object Deserialize(IJsonReader reader)
		{
			if (reader == null) throw new ArgumentNullException("reader");

			if (reader.Value.Raw is JsonString)
				return reader.Value.Raw as JsonString;

			var jsonStringBuilder = new StringBuilder();
			CopyValue(reader, jsonStringBuilder);
			var jsonString = new JsonString(jsonStringBuilder);
			return jsonString;
		}

		public override void Serialize(IJsonWriter writer, object value)
		{
			if (writer == null) throw new ArgumentNullException("writer");
			if (value == null) throw new ArgumentNullException("value");

			var jsonString = (JsonString)value;
			writer.WriteJson(jsonString.ToString());
		}

		private static void CopyValue(IJsonReader reader, StringBuilder buffer)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			switch (reader.Token)
			{
				case JsonToken.None:
				case JsonToken.EndOfStream:
					throw new UnexpectedEndOfStream(reader);
				case JsonToken.DateTime:
				case JsonToken.EndOfArray:
				case JsonToken.EndOfObject:
				case JsonToken.Number:
				case JsonToken.Member:
				case JsonToken.Boolean:
				case JsonToken.StringLiteral:
				case JsonToken.Null:
					if (buffer != null) reader.Value.CopyJsonTo(buffer);
					break;
				case JsonToken.BeginArray:

					// copy BeginArray token
					if (buffer != null) reader.Value.CopyJsonTo(buffer);

					reader.NextToken(); // advance

					// iterate values
					while (reader.Token != JsonToken.EndOfArray)
					{
						// copy Value
						CopyValue(reader, buffer);
						reader.NextToken();

						if (reader.Token != JsonToken.EndOfArray)
							buffer.Append(',');
					}

					// copy EndOfArray token
					if (buffer != null) reader.Value.CopyJsonTo(buffer);
					break;
				case JsonToken.BeginObject:
					// copy BeginObject token
					if (buffer != null) reader.Value.CopyJsonTo(buffer);

					reader.NextToken(); // advance

					// iterate values
					while (reader.Token != JsonToken.EndOfObject)
					{
						// copy MemberName token
						if (buffer != null) reader.Value.CopyJsonTo(buffer);
						reader.NextToken();
						if (buffer != null) buffer.Append(':');
						// copy Value
						CopyValue(reader, buffer);
						reader.NextToken();

						if (reader.Token != JsonToken.EndOfObject)
							buffer.Append(',');
					}

					// copy EndOfObject token
					if (buffer != null) reader.Value.CopyJsonTo(buffer);
					break;
			}
		}
	}
}
