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
using System.Runtime.Serialization;
using System.Security;

namespace Serialization.Json.Exceptions
{
	[Serializable]
	public class JsonSerializationException : SerializationException
	{
		public int Code { get; set; }
		public int LineNumber { get; set; }
		public int ColumnNumber { get; set; }
		public long CharactersReaded { get; set; }
		public long CharactersWritten { get; set; }

		internal JsonSerializationException(string message, ErrorCode errorCode, IJsonReader reader)
			: base(message)
		{
			this.Code = (int) errorCode;
			if (reader != null)
				this.Update(reader);
		}

		internal JsonSerializationException(string message, ErrorCode errorCode, IJsonWriter writer)
			: base(message)
		{
			this.Code = (int) errorCode;
			if (writer != null)
				this.Update(writer);
		}

		internal JsonSerializationException(string message, ErrorCode errorCode)
			: base(message)
		{
			this.Code = (int) errorCode;
		}

		internal JsonSerializationException(string message, ErrorCode errorCode, IJsonReader reader, Exception innerException)
			: base(message, innerException)
		{
			this.Code = (int) errorCode;
			if (reader != null)
				this.Update(reader);
		}

		internal JsonSerializationException(string message, ErrorCode errorCode, IJsonWriter writer, Exception innerException)
			: base(message, innerException)
		{
			this.Code = (int) errorCode;
			if (writer != null)
				this.Update(writer);
		}

		internal JsonSerializationException(string message, ErrorCode errorCode, Exception innerException)
			: base(message, innerException)
		{
			this.Code = (int) errorCode;
		}

		internal JsonSerializationException(string message, Exception innerException)
			: base(message, innerException)
		{
			this.Code = (int) ErrorCode.GenericJsonSerializationException;
		}

		internal JsonSerializationException(string message)
			: base(message)
		{
			this.Code = (int) ErrorCode.GenericJsonSerializationException;
		}

		internal JsonSerializationException()
			: base("During serialization error has occurred.")
		{
			this.Code = (int) ErrorCode.GenericJsonSerializationException;
		}

		internal JsonSerializationException(Exception innerException)
			: base(String.Format("During serialization error has occurred."), innerException)
		{
			this.Code = (int) ErrorCode.GenericJsonSerializationException;
		}

		protected JsonSerializationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.Code = info.GetInt32("Code");
			this.LineNumber = info.GetInt32("LineNumber");
			this.ColumnNumber = info.GetInt32("ColumnNumber");
			this.CharactersReaded = info.GetInt64("CharactersReaded");
			this.CharactersWritten = info.GetInt64("CharactersWritten");
		}

		private void Update(IJsonReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			this.LineNumber = reader.Value.LineNumber;
			this.ColumnNumber = reader.Value.ColumnNumber;
			this.CharactersReaded = reader.CharactersReaded;
		}

		private void Update(IJsonWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");


			this.CharactersWritten = writer.CharactersWritten;
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Code", this.Code);
			info.AddValue("LineNumber", this.LineNumber);
			info.AddValue("ColumnNumber", this.ColumnNumber);
			info.AddValue("CharactersReaded", this.CharactersReaded);
			info.AddValue("CharactersWritten", this.CharactersWritten);

			base.GetObjectData(info, context);
		}
	}
}