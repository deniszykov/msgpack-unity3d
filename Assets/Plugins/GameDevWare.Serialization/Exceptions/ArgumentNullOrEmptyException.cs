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
namespace Serialization.Json.Exceptions
{
	[Serializable]
	internal sealed class ArgumentNullOrEmptyException : ArgumentException
	{
		// Methods
		public ArgumentNullOrEmptyException()
			: base(DefaultMessage(""))
		{
		}

		public ArgumentNullOrEmptyException(string paramName)
			: base(DefaultMessage(paramName), paramName)
		{
		}
		public ArgumentNullOrEmptyException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
		public ArgumentNullOrEmptyException(string paramName, string message)
			: base(paramName, message)
		{
		}
		public ArgumentNullOrEmptyException(string paramName, string message, Exception innerException)
			: base(paramName, message, innerException)
		{

		}
		private ArgumentNullOrEmptyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private static string DefaultMessage(string paramName)
		{
			return string.Format("Argument '{0}' is null or empty.", paramName);
		}
	}
}