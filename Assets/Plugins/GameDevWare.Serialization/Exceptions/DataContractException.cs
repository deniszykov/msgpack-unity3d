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

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization.Exceptions
{
	[Serializable]
	public class DataContractException : SerializationException
	{
		public int Code { get; set; }

		internal DataContractException(string message, ErrorCode errorCode) : base(message)
		{
		}

		internal DataContractException(string message, ErrorCode errorCode, Exception innerException)
			: base(message, innerException)
		{
		}

		internal DataContractException(string message, Exception innerException) : base(message, innerException)
		{
			Code = (int) ErrorCode.GenericDataContractException;
		}

		internal DataContractException(string message) : base(message)
		{
			Code = (int) ErrorCode.GenericDataContractException;
		}

		protected DataContractException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.Code = info.GetInt32("Code");
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Code", this.Code);
			base.GetObjectData(info, context);
		}
	}
}
