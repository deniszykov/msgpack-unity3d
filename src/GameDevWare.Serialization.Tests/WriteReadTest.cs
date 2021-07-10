using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using GameDevWare.Serialization.MessagePack;
using NUnit.Framework;

namespace GameDevWare.Serialization.Tests
{
	[TestFixture, Category("WriteReadTests")]
	public class WriteReadTest
	{
		[Test]
		public void WriteReadDictionaryMsgPack()
		{
			var expectedValue = new Dictionary<string, int>
			{
				{ "a", 1 },
				{ "b", 2 },
				{ "c", 3 },
			};
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadIDictionary2MsgPack()
		{
			var expectedValue = new Dictionary<string, int>
			{
				{ "a", 1 },
				{ "b", 2 },
				{ "c", 3 },
			};
			var actualValue = this.WriteReadMessagePack<IDictionary<string, int>>(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadIndexedDictionary2MsgPack()
		{
			var expectedValue = new IndexedDictionary<string, int>
			{
				{ "a", 1 },
				{ "b", 2 },
				{ "c", 3 },
			};
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadHashtableMsgPack()
		{
			var expectedValue = new Hashtable
			{
				{ "a", 1 },
				{ "b", 2 },
				{ "c", 3 },
			};
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteIDictionaryMsgPack()
		{
			var expectedValue = new Hashtable
			{
				{ "a", 1 },
				{ "b", 2 },
				{ "c", 3 },
			};
			var actualValue = this.WriteReadMessagePack<IDictionary>(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadListMsgPack()
		{
			var expectedValue = new List<int> { 1, 2, 3 };
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadHashSetMsgPack()
		{
			var expectedValue = new HashSet<int> { 1, 2, 3 };
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadArrayListMsgPack()
		{
			var expectedValue = new ArrayList { 1, 2, 3 };
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadTestObjectMsgPack()
		{
			var expectedValue = new TestObject
			{
				IntField = int.MaxValue,
				LongField = long.MaxValue,
				ShortField = short.MaxValue,
				SingleField = float.MaxValue,
				DoubleField = double.MaxValue,
				DecimalField = decimal.MaxValue,
				IntProperty = 2,
				IntArrayProperty = new[] { 1, 2, 3, int.MaxValue, int.MinValue, byte.MaxValue, byte.MinValue, short.MaxValue, short.MinValue, sbyte.MinValue, sbyte.MaxValue, ushort.MinValue, ushort.MaxValue },
				MixedArrayProperty = new object[] { 1, false, "a text", new TestObject(), "a another text", 2, new object[] { 1, 2, 3 }, null, 4, null },
				ObjectProperty = new TestObject
				{
					IntField = 2,
					BoolProperty = false,
					LongField = long.MinValue,
					ShortField = short.MinValue,
					SingleField = float.MinValue,
					DoubleField = double.MinValue,
					DecimalField = decimal.MinValue,
					ObjectProperty = new TestObject(),
					IntArrayProperty = new int[0],
					AnyProperty = new object()
				},
				StringArrayProperty = new[] { "a\ttext\nasaaasdasdasd \n\r\n \a \r asdasd", null, null, null },
				StringProperty = new string(Array.ConvertAll(Enumerable.Range(1, 256).ToArray(), input => (char)input)),
				BoolProperty = true,
				DateProperty = DateTime.Today,
				DateOffsetProperty = DateTimeOffset.UtcNow,
				NullableProperty = long.MinValue,
				ObjectArrayProperty = new[] { new TestObject(), new TestObject() },
				AnyProperty = "",
			};
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue, actualValue);
		}

		[Test]
		public void WriteReadIListMsgPack()
		{
			var expectedValue = new List<int> { 1, 2, 3 };
			var actualValue = this.WriteReadMessagePack<IList<int>>(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadUtcDateMsgPack()
		{
			var expectedValue = DateTime.UtcNow;
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.AreEqual(expectedValue, actualValue);
		}

		[Test]
		public void WriteReadLocalDateMsgPack()
		{
			var expectedValue = DateTime.Now;
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.AreEqual(expectedValue, actualValue);
		}

		[Test]
		public void WriteReadUnspecifiedDateMsgPack()
		{
			var expectedValue = new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified);
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.AreEqual(expectedValue, actualValue);
		}

		[Test]
		public void WriteReadDateTimeOffsetLocalMsgPack()
		{
			var expectedValue = DateTimeOffset.Now;
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.AreEqual(expectedValue, actualValue);
		}

		[Test]
		public void WriteReadDateTimeOffsetUtcMsgPack()
		{
			var expectedValue = DateTimeOffset.UtcNow;
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.AreEqual(expectedValue, actualValue);
		}

		[Test]
		public void WriteReadDecimalMaxMsgPack()
		{
			var expectedValue = decimal.MaxValue;
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.AreEqual(expectedValue, actualValue);
		}

		[Test]
		public void WriteReadDecimalMinMsgPack()
		{
			var expectedValue = decimal.MinValue;
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.AreEqual(expectedValue, actualValue);
		}

		[Test]
		public void WriteReadDecimalOneMsgPack()
		{
			var expectedValue = decimal.One;
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.AreEqual(expectedValue, actualValue);
		}

		[Test]
		public void WriteReadDictionaryJson()
		{
			var expectedValue = new Dictionary<string, int>
			{
				{ "a", 1 },
				{ "b", 2 },
				{ "c", 3 },
			};
			var actualValue = this.WriteReadJson(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadIDictionary2Json()
		{
			var expectedValue = new Dictionary<string, int>
			{
				{ "a", 1 },
				{ "b", 2 },
				{ "c", 3 },
			};
			var actualValue = this.WriteReadJson<IDictionary<string, int>>(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadIndexedDictionary2Json()
		{
			var expectedValue = new IndexedDictionary<string, int>
			{
				{ "a", 1 },
				{ "b", 2 },
				{ "c", 3 },
			};
			var actualValue = this.WriteReadJson(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadHashtableJson()
		{
			var expectedValue = new Hashtable
			{
				{ "a", 1 },
				{ "b", 2 },
				{ "c", 3 },
			};
			var actualValue = this.WriteReadJson(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteIDictionaryJson()
		{
			var expectedValue = new Hashtable
			{
				{ "a", 1 },
				{ "b", 2 },
				{ "c", 3 },
			};
			var actualValue = this.WriteReadJson<IDictionary>(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadEmptyDictionaryJson()
		{
			var expectedValue = new Dictionary<string, int>();
			var actualValue = this.WriteReadJson(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
			CollectionAssert.AreEqual(expectedValue, actualValue);
		}

		[Test]
		public void WriteReadNonStringKeyDictionaryJson()
		{
			var expectedValue = new Dictionary<Version, int>
			{
				{ new Version(1, 0, 1), 1 },
				{ new Version(1, 0, 2), 1 },
			};
			var actualValue = this.WriteReadJson(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
			CollectionAssert.AreEqual(expectedValue, actualValue);
		}

		[Test]
		public void WriteReadEmptyNonStringKeyDictionaryJson()
		{
			var expectedValue = new Dictionary<Version, int>();
			var actualValue = this.WriteReadJson(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
			CollectionAssert.AreEqual(expectedValue, actualValue);
		}

		[Test]
		public void WriteReadListJson()
		{
			var expectedValue = new List<int> { 1, 2, 3 };
			var actualValue = this.WriteReadJson(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadIListJson()
		{
			var expectedValue = new List<int> { 1, 2, 3 };
			var actualValue = this.WriteReadJson<IList<int>>(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadHashSetJson()
		{
			var expectedValue = new HashSet<int> { 1, 2, 3 };
			var actualValue = this.WriteReadJson(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadArrayListJson()
		{
			var expectedValue = new ArrayList { 1, 2, 3 };
			var actualValue = this.WriteReadJson(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadTestObjectJson()
		{
			var expectedValue = new TestObject
			{
				IntField = int.MaxValue,
				LongField = long.MaxValue,
				ShortField = short.MaxValue,
				SingleField = float.MaxValue,
				DoubleField = double.MaxValue,
				DecimalField = decimal.MaxValue,
				IntProperty = 2,
				IntArrayProperty = new[] { 1, 2, 3, int.MaxValue, int.MinValue, byte.MaxValue, byte.MinValue, short.MaxValue, short.MinValue, sbyte.MinValue, sbyte.MaxValue, ushort.MinValue, ushort.MaxValue },
				MixedArrayProperty = new object[] { 1, false, "a text", new TestObject(), "a another text", 2, new object[] { 1, 2, 3 }, null, 4, null },
				ObjectProperty = new TestObject
				{
					IntField = 2,
					BoolProperty = false,
					LongField = long.MinValue,
					ShortField = short.MinValue,
					SingleField = float.MinValue,
					DoubleField = double.MinValue,
					DecimalField = decimal.MinValue,
					ObjectProperty = new TestObject(),
					IntArrayProperty = new int[0],
					AnyProperty = new object()
				},
				StringArrayProperty = new[] { "a\ttext\nasaaasdasdasd \n\r\n \a \r asdasd", null, null, null },
				StringProperty = new string(Array.ConvertAll(Enumerable.Range(1, 256).ToArray(), input => (char)input)),
				BoolProperty = true,
				DateProperty = DateTime.Today,
				DateOffsetProperty = DateTimeOffset.UtcNow,
				NullableProperty = long.MinValue,
				ObjectArrayProperty = new[] { new TestObject(), new TestObject() },
				AnyProperty = "",
			};
			var actualValue = this.WriteReadJson(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue, actualValue);
		}

		[Test]
		public void ReadDictionaryAsJsonArrayOfPairs()
		{
			var json = Json.SerializeToString(new[] { new DictionaryEntry("a", 1), new DictionaryEntry("b", 2), new DictionaryEntry("c", 3) });
			var expected = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } };
			var actual = Json.Deserialize<Dictionary<string, int>>(json);

			Trace.WriteLine(json);

			Assert.IsNotNull(actual, "actual != null");
			CollectionAssert.AreEqual(actual, expected);
		}

		[Test]
		public void ReadDictionaryAsJsonArrayOfArrays()
		{
			var json = Json.SerializeToString(new[] { new object[] { "a", 1 }, new object[] { "b", 2 }, new object[] { "c", 3 } });
			var expected = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } };
			var actual = Json.Deserialize<Dictionary<string, int>>(json);

			Trace.WriteLine(json);

			Assert.IsNotNull(actual, "actual != null");
			CollectionAssert.AreEqual(actual, expected);
		}


		private T WriteReadMessagePack<T>(T value)
		{
			var stream = new MemoryStream();
			MsgPack.Serialize(value, stream);

			stream.Position = 0;
			Debug.WriteLine(new MsgPackReader(stream, new SerializationContext()).DebugPrintTokens());

			stream.Position = 0;			
			var readValue = (T)MsgPack.Deserialize(typeof(T), stream);
			return readValue;
		}
		private T WriteReadJson<T>(T value)
		{
			var stream = new MemoryStream();
			Json.Serialize(value, stream);
			stream.Position = 0;

			var json = Json.DefaultEncoding.GetString(stream.ToArray());
			Trace.WriteLine(json);
			Debug.WriteLine(new JsonStreamReader(stream, new SerializationContext()).DebugPrintTokens());

			stream.Position = 0;
			var readValue = (T)Json.Deserialize(typeof(T), stream);
			return readValue;
		}
	}
}
