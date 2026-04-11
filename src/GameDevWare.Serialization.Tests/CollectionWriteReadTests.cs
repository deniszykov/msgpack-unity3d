using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace GameDevWare.Serialization.Tests
{
	public class CollectionWriteReadTests : TestBase
	{
		[Fact]
		public void WriteReadDictionaryMsgPack()
		{
			var expectedValue = new Dictionary<string, int>
			{
				{ "a", 1 },
				{ "b", 2 },
				{ "c", 3 },
			};
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.NotNull(actualValue);
			Assert.Equal(expectedValue.Count, actualValue.Count);
		}

		[Fact]
		public void WriteReadIDictionary2MsgPack()
		{
			var expectedValue = new Dictionary<string, int>
			{
				{ "a", 1 },
				{ "b", 2 },
				{ "c", 3 },
			};
			var actualValue = this.WriteReadMessagePack<IDictionary<string, int>>(expectedValue);

			Assert.NotNull(actualValue);
			Assert.Equal(expectedValue.Count, actualValue.Count);
		}

		[Fact]
		public void WriteReadIndexedDictionary2MsgPack()
		{
			var expectedValue = new IndexedDictionary<string, int>
			{
				{ "a", 1 },
				{ "b", 2 },
				{ "c", 3 },
			};
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.NotNull(actualValue);
			Assert.Equal(expectedValue.Count, actualValue.Count);
		}

		[Fact]
		public void WriteReadHashtableMsgPack()
		{
			var expectedValue = new Hashtable
			{
				{ "a", 1 },
				{ "b", 2 },
				{ "c", 3 },
			};
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.NotNull(actualValue);
			Assert.Equal(expectedValue.Count, actualValue.Count);
		}

		[Fact]
		public void WriteReadListMsgPack()
		{
			var expectedValue = new List<int> { 1, 2, 3 };
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.NotNull(actualValue);
			Assert.Equal(expectedValue.Count, actualValue.Count);
		}

		[Fact]
		public void WriteReadHashSetMsgPack()
		{
			var expectedValue = new HashSet<int> { 1, 2, 3 };
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.NotNull(actualValue);
			Assert.Equal(expectedValue.Count, actualValue.Count);
		}

		[Fact]
		public void WriteReadArrayListMsgPack()
		{
			var expectedValue = new ArrayList { 1, 2, 3 };
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.NotNull(actualValue);
			Assert.Equal(expectedValue.Count, actualValue.Count);
		}

		[Fact]
		public void WriteReadDictionaryJson()
		{
			var expectedValue = new Dictionary<string, int>
			{
				{ "a", 1 },
				{ "b", 2 },
				{ "c", 3 },
			};
			var actualValue = this.WriteReadJson(expectedValue);

			Assert.NotNull(actualValue);
			Assert.Equal(expectedValue.Count, actualValue.Count);
		}

		[Fact]
		public void WriteReadEmptyDictionaryJson()
		{
			var expectedValue = new Dictionary<string, int>();
			var actualValue = this.WriteReadJson(expectedValue);

			Assert.NotNull(actualValue);
			Assert.Equal(expectedValue.Count, actualValue.Count);
			Assert.Equal(expectedValue, actualValue);
		}

		[Fact]
		public void WriteReadNonStringKeyDictionaryJson()
		{
			var expectedValue = new Dictionary<Version, int>
			{
				{ new Version(1, 0, 1), 1 },
				{ new Version(1, 0, 2), 1 },
			};
			var actualValue = this.WriteReadJson(expectedValue);

			Assert.NotNull(actualValue);
			Assert.Equal(expectedValue.Count, actualValue.Count);
			Assert.Equal(expectedValue, actualValue);
		}

		[Fact]
		public void WriteReadListJson()
		{
			var expectedValue = new List<int> { 1, 2, 3 };
			var actualValue = this.WriteReadJson(expectedValue);

			Assert.NotNull(actualValue);
			Assert.Equal(expectedValue.Count, actualValue.Count);
		}
	}
}
