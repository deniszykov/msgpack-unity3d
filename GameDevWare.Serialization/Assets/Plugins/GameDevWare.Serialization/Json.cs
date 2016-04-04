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
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using Serialization.Json.Exceptions;
using Serialization.Json.Serializers;

namespace Serialization.Json
{
	public static class Json
	{
		private static IFormatProvider _DefaultFormat = CultureInfo.InvariantCulture;
		private static Encoding _DefaultEncoding = new UTF8Encoding(false, true);

		private static string[] _DefaultDateTimeFormats = new string[]
		{
			"yyyy-MM-ddTHH:mm:ss.fffzzz", // ISO 8601, with timezone
			"yyyy-MM-ddTHH:mm:ss.ffzzz", // ISO 8601, with timezone
			"yyyy-MM-ddTHH:mm:ss.fzzz", // ISO 8601, with timezone
			"yyyy-MM-ddTHH:mm:ssZ", // also ISO 8601, without timezone and without microseconds
			"yyyy-MM-ddTHH:mm:ss.fZ", // also ISO 8601, without timezone
			"yyyy-MM-ddTHH:mm:ss.ffZ", // also ISO 8601, without timezone
			"yyyy-MM-ddTHH:mm:ss.fffZ", // also ISO 8601, without timezone
			"yyyy-MM-ddTHH:mm:ss.ffffZ", // also ISO 8601, without timezone
			"yyyy-MM-ddTHH:mm:ss.fffffZ", // also ISO 8601, without timezone
			"yyyy-MM-ddTHH:mm:ss.ffffffZ", // also ISO 8601, without timezone
			"yyyy-MM-ddTHH:mm:ss.fffffffZ"  // also ISO 8601, without timezone
		};

		public static string[] DefaultDateTimeFormats
		{
			get { return _DefaultDateTimeFormats; }
			set
			{
				if (value == null) throw new ArgumentNullException("value");
				if (value.Length == 0) throw new ArgumentException();

				_DefaultDateTimeFormats = value;
			}
		}

		public static IFormatProvider DefaultFormat
		{
			get { return _DefaultFormat; }
			set
			{
				if (value == null) throw new ArgumentNullException("value");

				_DefaultFormat = value;
			}
		}

		public static Encoding DefaultEncoding
		{
			get { return _DefaultEncoding; }
			set
			{
				if (value == null) throw new ArgumentNullException("value");

				_DefaultEncoding = value;
			}
		}

		public static ReadOnlyCollection<TypeSerializer> DefaultSerializers { get; private set; }

		static Json()
		{
			DefaultSerializers = new ReadOnlyCollection<TypeSerializer>(new TypeSerializer[]
			{
				new Base64Serializer(),
				new DateTimeOffsetSerializer(),
				new DateTimeSerializer(),
				new GuidSerializer(),
				new JsonStringSerializer(),
				new StreamSerializer(),
				new UriSerializer(),
				new VersionSerializer(),
				new TimeSpanSerializer(),
				new PrimitiveSerializer(typeof (Boolean)),
				new PrimitiveSerializer(typeof (Byte)),
				new PrimitiveSerializer(typeof (Decimal)),
				new PrimitiveSerializer(typeof (Double)),
				new PrimitiveSerializer(typeof (Int16)),
				new PrimitiveSerializer(typeof (Int32)),
				new PrimitiveSerializer(typeof (Int64)),
				new PrimitiveSerializer(typeof (sbyte)),
				new PrimitiveSerializer(typeof (Single)),
				new PrimitiveSerializer(typeof (UInt16)),
				new PrimitiveSerializer(typeof (UInt32)),
				new PrimitiveSerializer(typeof (UInt64)),
				new PrimitiveSerializer(typeof (String)),
			});
		}

		public static void Serialize<T>(T objectToSerialize, Stream jsonOutput)
		{
			Serialize(objectToSerialize, jsonOutput, CreateDefaultContext(SerializationOptions.None));
		}
		public static void Serialize<T>(T objectToSerialize, Stream jsonOutput, SerializationOptions options)
		{
			Serialize(objectToSerialize, jsonOutput, CreateDefaultContext(options));
		}
		public static void Serialize<T>(T objectToSerialize, Stream jsonOutput, ISerializationContext context)
		{
			if (jsonOutput == null) throw new ArgumentNullException("jsonOutput");
			if (!jsonOutput.CanWrite) throw new UnwriteableStream("jsonOutput");
			if (context == null) throw new ArgumentNullException("context");

			if (objectToSerialize == null)
			{
				var bytes = DefaultEncoding.GetBytes("null");
				jsonOutput.Write(bytes, 0, bytes.Length);
				return;
			}

			var writer = new JsonStreamWriter(jsonOutput, context);
			writer.WriteValue(objectToSerialize, typeof(T));
			writer.Flush();
		}

		public static void Serialize<T>(T objectToSerialize, TextWriter textWriter)
		{
			Serialize(objectToSerialize, textWriter, CreateDefaultContext(SerializationOptions.None));
		}
		public static void Serialize<T>(T objectToSerialize, TextWriter textWriter, SerializationOptions options)
		{
			Serialize(objectToSerialize, textWriter, CreateDefaultContext(options));
		}
		public static void Serialize<T>(T objectToSerialize, TextWriter textWriter, ISerializationContext context)
		{
			if (textWriter == null) throw new ArgumentNullException("textWriter");
			if (context == null) throw new ArgumentNullException("context");

			if (objectToSerialize == null)
			{
				textWriter.Write("null");
				textWriter.Flush();
				return;
			}


			var writer = new JsonTextWriter(textWriter, context);
			writer.WriteValue(objectToSerialize, typeof(T));
			writer.Flush();
		}

		public static void Serialize<T>(T objectToSerialize, IJsonWriter writer, ISerializationContext context)
		{
			if (writer == null) throw new ArgumentNullException("writer");
			if (context == null) throw new ArgumentNullException("context");

			if (objectToSerialize == null)
			{
				writer.WriteNull();
				writer.Flush();
				return;
			}

			writer.WriteValue(objectToSerialize, typeof(T));
			writer.Flush();
		}

		public static string SerializeToString<T>(T objectToSerialize)
		{
			return SerializeToString(objectToSerialize, CreateDefaultContext(SerializationOptions.None));
		}
		public static string SerializeToString<T>(T objectToSerialize, SerializationOptions options)
		{
			return SerializeToString(objectToSerialize, CreateDefaultContext(options));
		}
		public static string SerializeToString<T>(T objectToSerialize, ISerializationContext context)
		{
			if (context == null) throw new ArgumentNullException("context");

			if (objectToSerialize == null)
				return "null";

			var writer = new JsonStringBuilderWriter(new StringBuilder(), context);
			writer.WriteValue(objectToSerialize, typeof(T));
			writer.Flush();

			return writer.ToString();
		}

		public static object Deserialize(Type objectType, Stream jsonStream)
		{
			return Deserialize(objectType, jsonStream, CreateDefaultContext(SerializationOptions.None));
		}
		public static object Deserialize(Type objectType, Stream jsonStream, SerializationOptions options)
		{
			return Deserialize(objectType, jsonStream, CreateDefaultContext(options));
		}
		public static object Deserialize(Type objectType, Stream jsonStream, ISerializationContext context)
		{
			if (objectType == null) throw new ArgumentNullException("objectType");
			if (jsonStream == null) throw new ArgumentNullException("jsonStream");
			if (!jsonStream.CanRead) throw new UnreadableStream("jsonStream");
			if (context == null) throw new ArgumentNullException("context");


			var reader = new JsonStreamReader(jsonStream, context);
			return reader.ReadValue(objectType, false);
		}

		public static object Deserialize(Type objectType, TextReader textReader)
		{
			return Deserialize(objectType, textReader, CreateDefaultContext(SerializationOptions.None));
		}
		public static object Deserialize(Type objectType, TextReader textReader, SerializationOptions options)
		{
			return Deserialize(objectType, textReader, CreateDefaultContext(options));
		}
		public static object Deserialize(Type objectType, TextReader textReader, ISerializationContext context)
		{
			if (objectType == null) throw new ArgumentNullException("objectType");
			if (textReader == null) throw new ArgumentNullException("textReader");
			if (context == null) throw new ArgumentNullException("context");

			var reader = new JsonTextReader(textReader, context);
			return reader.ReadValue(objectType, false);
		}

		public static object Deserialize(Type objectType, string jsonString)
		{
			return Deserialize(objectType, jsonString, CreateDefaultContext(SerializationOptions.None));
		}
		public static object Deserialize(Type objectType, string jsonString, SerializationOptions options)
		{
			return Deserialize(objectType, jsonString, CreateDefaultContext(options));
		}
		public static object Deserialize(Type objectType, string jsonString, ISerializationContext context)
		{
			if (objectType == null) throw new ArgumentNullException("objectType");
			if (jsonString == null) throw new ArgumentNullException("jsonString");
			if (context == null) throw new ArgumentNullException("context");


			var reader = new JsonStringReader(jsonString, context);
			return reader.ReadValue(objectType, false);
		}

		public static object Deserialize(Type objectType, JsonString jsonString)
		{
			return Deserialize(objectType, jsonString, CreateDefaultContext(SerializationOptions.None));
		}
		public static object Deserialize(Type objectType, JsonString jsonString, SerializationOptions options)
		{
			return Deserialize(objectType, jsonString, CreateDefaultContext(options));
		}
		public static object Deserialize(Type objectType, JsonString jsonString, ISerializationContext context)
		{
			if (objectType == null) throw new ArgumentNullException("objectType");
			if (jsonString == null) throw new ArgumentNullException("jsonString");
			if (context == null) throw new ArgumentNullException("context");

			var reader = jsonString.ToJsonReader(context);
			return reader.ReadValue(objectType, false);
		}

		public static object Deserialize(Type objectType, IJsonReader reader)
		{
			if (objectType == null) throw new ArgumentNullException("objectType");
			if (reader == null) throw new ArgumentNullException("reader");

			return reader.ReadValue(objectType, false);
		}

		public static T Deserialize<T>(Stream jsonStream)
		{
			return (T)Deserialize(typeof(T), jsonStream, CreateDefaultContext(SerializationOptions.None));
		}
		public static T Deserialize<T>(Stream jsonStream, SerializationOptions options)
		{
			return (T)Deserialize(typeof(T), jsonStream, CreateDefaultContext(options));
		}
		public static T Deserialize<T>(Stream jsonStream, ISerializationContext context)
		{
			return (T)Deserialize(typeof(T), jsonStream, context);

		}

		public static T Deserialize<T>(TextReader textReader)
		{
			return (T)Deserialize(typeof(T), textReader, CreateDefaultContext(SerializationOptions.None));
		}
		public static T Deserialize<T>(TextReader textReader, SerializationOptions options)
		{
			return (T)Deserialize(typeof(T), textReader, CreateDefaultContext(options));
		}
		public static T Deserialize<T>(TextReader textReader, ISerializationContext context)
		{
			return (T)Deserialize(typeof(T), textReader, context);
		}

		public static T Deserialize<T>(string jsonString)
		{
			return (T)Deserialize(typeof(T), jsonString, CreateDefaultContext(SerializationOptions.None));
		}
		public static T Deserialize<T>(string jsonString, SerializationOptions options)
		{
			return (T)Deserialize(typeof(T), jsonString, CreateDefaultContext(options));
		}
		public static T Deserialize<T>(string jsonString, ISerializationContext context)
		{
			return (T)Deserialize(typeof(T), jsonString, context);
		}

		public static T Deserialize<T>(JsonString jsonString)
		{
			return Deserialize<T>(jsonString, CreateDefaultContext(SerializationOptions.None));
		}
		public static T Deserialize<T>(JsonString jsonString, SerializationOptions options)
		{
			return Deserialize<T>(jsonString, CreateDefaultContext(options));
		}
		public static T Deserialize<T>(JsonString jsonString, ISerializationContext context)
		{
			return (T)Deserialize(typeof(T), jsonString, context);
		}

		public static T Deserialize<T>(IJsonReader reader)
		{
			if (reader == null) throw new ArgumentNullException("reader");

			var serializer = reader.Context.GetSerializerForType(typeof(T));
			return (T)serializer.Deserialize(reader);
		}


		private static ISerializationContext CreateDefaultContext(SerializationOptions options)
		{
			return new DefaultSerializationContext
			{
				Options = options
			};
		}
	}
}