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
using System.Runtime.Serialization;
using System.Security;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization.Exceptions
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
