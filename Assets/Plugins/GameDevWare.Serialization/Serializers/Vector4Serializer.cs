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
#if UNITY_5 || UNITY_4 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization.Serializers
{
	public sealed class Vector4Serializer : TypeSerializer
	{
		public override Type SerializedType { get { return typeof(Vector4); } }

		public override object Deserialize(IJsonReader reader)
		{
			if (reader == null) throw new ArgumentNullException("reader");

			if (reader.Token == JsonToken.Null)
				return null;

			var value = new Vector4();
			reader.ReadObjectBegin();
			while (reader.Token != JsonToken.EndOfObject)
			{
				var memberName = reader.ReadMember();
				switch (memberName)
				{
					case "x": value.x = reader.ReadSingle(); break;
					case "y": value.y = reader.ReadSingle(); break;
					case "z": value.z = reader.ReadSingle(); break;
					case "w": value.w = reader.ReadSingle(); break;
					default: reader.ReadValue(typeof(object)); break;
				}
			}
			reader.ReadObjectEnd(advance: false);
			return value;
		}
		public override void Serialize(IJsonWriter writer, object value)
		{
			if (writer == null) throw new ArgumentNullException("writer");
			if (value == null) throw new ArgumentNullException("value");

			var vector4 = (Vector4)value;
			writer.WriteObjectBegin(4);
			writer.WriteMember("x");
			writer.Write(vector4.x);
			writer.WriteMember("y");
			writer.Write(vector4.y);
			writer.WriteMember("z");
			writer.Write(vector4.z);
			writer.WriteMember("w");
			writer.Write(vector4.w);
			writer.WriteObjectEnd();
		}
	}
}
#endif
