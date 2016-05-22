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
using GameDevWare.Serialization.Exceptions;

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

			if (stream == null)
				throw new TypeContractViolation(this.GetType(), "be a Stream");

			if (!stream.CanRead)
				throw new UnreadableStream("value");

			var bufferSize = 3 * 1024 * 1024;
			var buffer = new byte[bufferSize]; // 3kb buffer

			// if it's a small seakable stream
			if (stream.CanSeek && stream.Length < bufferSize)
			{
				// read it to buffer
				stream.Read(buffer, 0, buffer.Length);
				// convert to base64
				var base64Str = Convert.ToBase64String(buffer, 0, (int)stream.Length);
				// and write it
				writer.WriteString(base64Str);
				return;
			}
			// else white with chunks

			var readed = 0;
			writer.WriteJson("\"");

			// read chunks
			while ((readed = stream.Read(buffer, 0, buffer.Length)) > 0)
			{
				var base64Str = Convert.ToBase64String(buffer, 0, readed);
				writer.WriteJson(base64Str);
			}

			writer.WriteJson("\"");
		}

		public override string ToString()
		{
			return string.Format("stream");
		}
	}
}
