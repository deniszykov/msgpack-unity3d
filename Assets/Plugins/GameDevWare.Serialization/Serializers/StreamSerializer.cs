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
using System.IO;
using Serialization.Json.Exceptions;

namespace Serialization.Json.Serializers
{
	public sealed class StreamSerializer : TypeSerializer
	{
		public override Type SerializedType
		{
			get { return typeof (Stream); }
		}

		public override object Deserialize(IJsonReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			if (reader.Token == JsonToken.Null)
				return null;

			var base64Str = Convert.ToString(reader.RawValue, reader.Context.Format);
			var bytes = Convert.FromBase64String(base64Str);
			return new MemoryStream(bytes);
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

			var stream = value as Stream;

			if (stream == null)
				throw new TypeContractViolation(this.GetType(), "be a Stream");

			if (!stream.CanRead)
				throw new UnreadableStream("value");

			var bufferSize = 3*1024*1024;
			var buffer = new byte[bufferSize]; // 3kb buffer

			// if it's a small seakable stream
			if (stream.CanSeek && stream.Length < bufferSize)
			{
				// read it to buffer
				stream.Read(buffer, 0, buffer.Length);
				// convert to base64
				var base64Str = Convert.ToBase64String(buffer, 0, (int) stream.Length);
				// and write it
				writer.WriteString(base64Str);
				return;
			}
			// else white with chunks

			var readed = 0;
			writer.WriteJson("\"");

			// read chunks
			while ((readed = stream.Read(buffer, 0, buffer.Length)) > 0)
			{
				var base64Str = Convert.ToBase64String(buffer, 0, readed);
				writer.WriteJson(base64Str);
			}

			writer.WriteJson("\"");
		}

		public override string ToString()
		{
			return string.Format("stream");
		}
	}
}