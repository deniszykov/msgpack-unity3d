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
using System.Text;
using GameDevWare.Serialization.Exceptions;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization.Serializers
{
	public sealed class JsonStringSerializer : TypeSerializer
	{
		public override Type SerializedType { get { return typeof(JsonString); } }

		public override object Deserialize(IJsonReader reader)
		{
			if (reader == null) throw new ArgumentNullException("reader");

			if (reader.Value.Raw is JsonString)
				return reader.Value.Raw as JsonString;

			var jsonStringBuilder = new StringBuilder();
			CopyValue(reader, jsonStringBuilder);
			var jsonString = new JsonString(jsonStringBuilder);
			return jsonString;
		}

		public override void Serialize(IJsonWriter writer, object value)
		{
			if (writer == null) throw new ArgumentNullException("writer");
			if (value == null) throw new ArgumentNullException("value");

			var jsonString = (JsonString)value;
			writer.WriteJson(jsonString.ToString());
		}

		private static void CopyValue(IJsonReader reader, StringBuilder buffer)
		{
			if (reader == null)
				throw new ArgumentNullException("reader");

			switch (reader.Token)
			{
				case JsonToken.None:
				case JsonToken.EndOfStream:
					throw new UnexpectedEndOfStream(reader);
				case JsonToken.DateTime:
				case JsonToken.EndOfArray:
				case JsonToken.EndOfObject:
				case JsonToken.Number:
				case JsonToken.Member:
				case JsonToken.Boolean:
				case JsonToken.StringLiteral:
				case JsonToken.Null:
					if (buffer != null) reader.Value.CopyJsonTo(buffer);
					break;
				case JsonToken.BeginArray:

					// copy BeginArray token
					if (buffer != null) reader.Value.CopyJsonTo(buffer);

					reader.NextToken(); // nextToken

					// iterate values
					while (reader.Token != JsonToken.EndOfArray)
					{
						// copy Value
						CopyValue(reader, buffer);
						reader.NextToken();

						if (reader.Token != JsonToken.EndOfArray)
							buffer.Append(',');
					}

					// copy EndOfArray token
					if (buffer != null) reader.Value.CopyJsonTo(buffer);
					break;
				case JsonToken.BeginObject:
					// copy BeginObject token
					if (buffer != null) reader.Value.CopyJsonTo(buffer);

					reader.NextToken(); // nextToken

					// iterate values
					while (reader.Token != JsonToken.EndOfObject)
					{
						// copy MemberName token
						if (buffer != null) reader.Value.CopyJsonTo(buffer);
						reader.NextToken();
						if (buffer != null) buffer.Append(':');
						// copy Value
						CopyValue(reader, buffer);
						reader.NextToken();

						if (reader.Token != JsonToken.EndOfObject)
							buffer.Append(',');
					}

					// copy EndOfObject token
					if (buffer != null) reader.Value.CopyJsonTo(buffer);
					break;
			}
		}
	}
}
