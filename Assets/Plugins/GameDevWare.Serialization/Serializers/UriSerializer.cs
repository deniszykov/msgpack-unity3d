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

namespace Serialization.Json.Serializers
{
	public sealed class UriSerializer : TypeSerializer
	{
		public override Type SerializedType
		{
			get { return typeof (Uri); }
		}

		public override object Deserialize(IJsonReader reader)
		{
			if (reader.Token == JsonToken.Null)
				return null;

			var uriStr = reader.ReadString(false);
			var value = new Uri(uriStr);
			return value;
		}

		public override void Serialize(IJsonWriter writer, object valueObj)
		{
			var value = (Uri) valueObj;
			if (value == null)
				writer.WriteNull();
			else
				writer.WriteString(value.ToString());
		}
	}
}