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
using GameDevWare.Serialization.MessagePack;
using GameDevWare.Serialization.Serializers;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization
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

			var reader = new MsgPackReader(msgPackInput, context);
			return reader.ReadValue(objectType, false);
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
