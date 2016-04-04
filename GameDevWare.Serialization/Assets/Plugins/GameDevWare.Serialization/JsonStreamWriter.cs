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

namespace Serialization.Json
{
	public sealed class JsonStreamWriter : JsonWriterBase
	{
		private StreamWriter writer;

		public Stream Stream
		{
			get { return writer.BaseStream; }
		}

		private StreamWriter Writer
		{
			get { return writer; }
		}

		public JsonStreamWriter(Stream stream, ISerializationContext context)
			: base(context)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (!stream.CanWrite)
				throw new UnwriteableStream("stream");


			writer = new StreamWriter(stream, context.Encoding);
		}

		public override void Flush()
		{
			writer.Flush();
		}

		public override void WriteJson(string jsonString)
		{
			if (jsonString == null)
				throw new ArgumentNullException("jsonString");


			writer.Write(jsonString);
			this.CharactersWritten += jsonString.Length;
		}

		public override void WriteJson(char[] jsonString, int index, int charactersToWrite)
		{
			if (jsonString == null)
				throw new ArgumentNullException("jsonString");
			if (index < 0 || index >= jsonString.Length)
				throw new ArgumentOutOfRangeException("index");
			if (charactersToWrite < 0 || index + charactersToWrite > jsonString.Length)
				throw new ArgumentOutOfRangeException("charactersToWrite");


			if (charactersToWrite == 0)
				return;
			writer.Write(jsonString, index, charactersToWrite);
			this.CharactersWritten += charactersToWrite;
		}
	}
}