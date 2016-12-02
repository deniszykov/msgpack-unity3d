using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace GameDevWare.Serialization.Tests
{
	[TestFixture, Category("WriteReadTest")]
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
			var actualValue = WriteReadMessagePack(expectedValue);

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
			var actualValue = WriteReadMessagePack<IDictionary<string, int>>(expectedValue);

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
			var actualValue = WriteReadMessagePack(expectedValue);

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
			var actualValue = WriteReadMessagePack<IDictionary>(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadListMsgPack()
		{
			var expectedValue = new List<int> { 1, 2, 3 };
			var actualValue = WriteReadMessagePack(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadHashSetMsgPack()
		{
			var expectedValue = new HashSet<int> { 1, 2, 3 };
			var actualValue = WriteReadMessagePack(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadArrayListMsgPack()
		{
			var expectedValue = new ArrayList { 1, 2, 3 };
			var actualValue = WriteReadMessagePack(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadIListMsgPack()
		{
			var expectedValue = new List<int> { 1, 2, 3 };
			var actualValue = WriteReadMessagePack<IList<int>>(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
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
			var actualValue = WriteReadJson(expectedValue);

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
			var actualValue = WriteReadJson<IDictionary<string, int>>(expectedValue);

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
			var actualValue = WriteReadJson(expectedValue);

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
			var actualValue = WriteReadJson<IDictionary>(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadListJson()
		{
			var expectedValue = new List<int> { 1, 2, 3 };
			var actualValue = WriteReadJson(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadIListJson()
		{
			var expectedValue = new List<int> { 1, 2, 3 };
			var actualValue = WriteReadJson<IList<int>>(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadHashSetJson()
		{
			var expectedValue = new HashSet<int> { 1, 2, 3 };
			var actualValue = WriteReadJson(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		[Test]
		public void WriteReadArrayListJson()
		{
			var expectedValue = new ArrayList { 1, 2, 3 };
			var actualValue = WriteReadJson(expectedValue);

			Assert.IsNotNull(actualValue, "actualValue != null");
			Assert.AreEqual(expectedValue.Count, actualValue.Count);
		}

		private T WriteReadMessagePack<T>(T value)
		{
			var stream = new MemoryStream();
			MsgPack.Serialize(value, stream);
			stream.Position = 0;
			var readedValue = (T)MsgPack.Deserialize(typeof(T), stream);
			return readedValue;
		}
		private T WriteReadJson<T>(T value)
		{
			var stream = new MemoryStream();
			Json.Serialize(value, stream);
			stream.Position = 0;
			var readedValue = (T)Json.Deserialize(typeof(T), stream);
			return readedValue;
		}
	}
}
