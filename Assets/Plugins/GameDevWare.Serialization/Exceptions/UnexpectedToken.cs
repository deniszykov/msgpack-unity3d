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

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization.Exceptions
{
	[Serializable]
	internal sealed class UnexpectedToken : JsonSerializationException
	{
		public UnexpectedToken(IJsonReader reader, params JsonToken[] expectedTokens) :
			base(
			String.Format("Unexpected token '{0}'. One of these expected: '{1}'.",
				reader.IsEndOfStream() ? JsonToken.EndOfStream : reader.Token, JoinTokens(expectedTokens)),
			ErrorCode.UnexpectedToken, reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");
			if (expectedTokens == null)
				throw new ArgumentNullException("expectedTokens");
		}

		private UnexpectedToken(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private static object JoinTokens(JsonToken[] expectedTokens)
		{
			if (expectedTokens == null)
				throw new ArgumentNullException("expectedTokens");


			if (expectedTokens.Length == 0)
				return "<no tokens>";
			var tokens = Array.ConvertAll(expectedTokens, c => c.ToString());
			return String.Join(", ", tokens);
		}
	}
}
