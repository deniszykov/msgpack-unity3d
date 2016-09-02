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
using System.IO;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization.Serializers
{
	public sealed class StreamSerializer : TypeSerializer
	{
		public override Type SerializedType { get { return typeof(Stream); } }

		public override object Deserialize(IJsonReader reader)
		{
			if (reader == null) throw new ArgumentNullException("reader");
			var base64Str = Convert.ToString(reader.RawValue, reader.Context.Format);
			var bytes = Convert.FromBase64String(base64Str);
			return new MemoryStream(bytes);
		}

		public override void Serialize(IJsonWriter writer, object value)
		{
			if (writer == null) throw new ArgumentNullException("writer");
			if (value == null) throw new ArgumentNullException("value");

			var stream = value as Stream;
			if (value != null && stream == null) throw JsonSerializationException.TypeIsNotValid(this.GetType(), "be a Stream");
			if (!stream.CanRead) throw new JsonSerializationException("Stream couldn't be readed.", JsonSerializationException.ErrorCode.StreamIsNotReadable);

			var bufferSize = 1 * 1024 * 1024;
			var buffer = new byte[bufferSize];
			var read = 0;
			if (writer is JsonWriterBase)
			{
				writer.WriteJson("\"");

				while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
				{
					var base64Str = Convert.ToBase64String(buffer, 0, read);
					writer.WriteJson(base64Str);
				}

				writer.WriteJson("\"");
			}
			else
			{

				var offset = 0;
				while ((read = stream.Read(buffer, offset, buffer.Length - offset)) != 0)
				{
					offset += read;
					if (offset == buffer.Length)
						Array.Resize(ref buffer, (int)(buffer.Length * 1.5));
				}

				var base64Str = Convert.ToBase64String(buffer, 0, offset);
				writer.Write(base64Str);
			}
		}

		public override string ToString()
		{
			return "stream";
		}
	}
}
