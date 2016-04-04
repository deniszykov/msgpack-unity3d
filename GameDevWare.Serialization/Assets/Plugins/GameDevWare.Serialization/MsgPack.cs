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
using System.IO;
using Serialization.Json.Exceptions;
using Serialization.Json.MessagePack;
using Serialization.Json.Serializers;

namespace Serialization.Json
{
	public static class MsgPack
	{
		public static void Serialize<T>(T objectToSerialize, Stream msgPackOutput)
		{
			Serialize(objectToSerialize, msgPackOutput, CreateDefaultContext(SerializationOptions.None));
		}
		public static void Serialize<T>(T objectToSerialize, Stream msgPackOutput, SerializationOptions options)
		{
			Serialize(objectToSerialize, msgPackOutput, CreateDefaultContext(options));
		}
		public static void Serialize<T>(T objectToSerialize, Stream msgPackOutput, ISerializationContext context)
		{
			if (msgPackOutput == null) throw new ArgumentNullException("msgPackOutput");
			if (context == null) throw new ArgumentNullException("context");

			var writer = new MsgPackWriter(msgPackOutput, context);
			if (objectToSerialize == null)
			{
				writer.WriteNull();
				writer.Flush();
				return;
			}
			writer.WriteValue(objectToSerialize, typeof(T));
			writer.Flush();
		}

		public static object Deserialize(Type objectType, Stream msgPackInput)
		{
			return Deserialize(objectType, msgPackInput, CreateDefaultContext(SerializationOptions.None));
		}
		public static object Deserialize(Type objectType, Stream msgPackInput, SerializationOptions options)
		{
			return Deserialize(objectType, msgPackInput, CreateDefaultContext(options));
		}
		public static object Deserialize(Type objectType, Stream msgPackInput, ISerializationContext context)
		{
			if (objectType == null) throw new ArgumentNullException("objectType");
			if (context == null) throw new ArgumentNullException("context");
			if (msgPackInput == null) throw new ArgumentNullException("msgPackInput");
			if (!msgPackInput.CanRead) throw new UnreadableStream("msgPackInput");

			var serializer = context.GetSerializerForType(objectType);
			var reader = new MsgPackReader(msgPackInput, context);
			return serializer.Deserialize(reader);
		}

		public static T Deserialize<T>(Stream msgPackInput)
		{
			return Deserialize<T>(msgPackInput, CreateDefaultContext(SerializationOptions.None));
		}
		public static T Deserialize<T>(Stream msgPackInput, SerializationOptions options)
		{
			return Deserialize<T>(msgPackInput, CreateDefaultContext(options));
		}
		public static T Deserialize<T>(Stream msgPackInput, ISerializationContext context)
		{
			if (context == null) throw new ArgumentNullException("context");
			if (msgPackInput == null) throw new ArgumentNullException("msgPackInput");
			if (!msgPackInput.CanRead) throw new UnreadableStream("msgPackInput");

			return (T)Deserialize(typeof(T), msgPackInput, context);
		}

		private static ISerializationContext CreateDefaultContext(SerializationOptions options)
		{
			return new DefaultSerializationContext
			{
				Options = options,
				EnumSerializerFactory = (enumType) => new EnumNumberSerializer(enumType)
			};
		}
	}
}