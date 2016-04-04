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
using Serialization.Json.Exceptions;

namespace Serialization.Json.Serializers
{
	public sealed class Base64Serializer : TypeSerializer
	{
		public override Type SerializedType
		{
			get { return typeof (byte[]); }
		}

		public override object Deserialize(IJsonReader reader)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");


			var value = reader.RawValue as string;
			if (value == null)
				return null;

			var buffer = Convert.FromBase64String(value);
			return buffer;
		}

		public override void Serialize(IJsonWriter writer, object value)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");

			if (value != null && value as byte[] == null)
				throw new TypeContractViolation(this.GetType(), "be array of bytes");

			if (value == null)
			{
				writer.WriteNull();
				return;
			}

			var base64String = Convert.ToBase64String(value as byte[]);
			writer.WriteString(base64String);
		}

		public override string ToString()
		{
			return string.Format("byte[] as Base64");
		}
	}
}