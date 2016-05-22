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
