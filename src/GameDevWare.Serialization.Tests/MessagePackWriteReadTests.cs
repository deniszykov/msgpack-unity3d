using System;
using System.Linq;
using Xunit;

namespace GameDevWare.Serialization.Tests
{
	public class MessagePackWriteReadTests : TestBase
	{
		[Fact]
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

			Assert.NotNull(actualValue);
			Assert.Equal(expectedValue, actualValue);
		}

		[Fact]
		public void WriteReadUtcDateMsgPack()
		{
			var expectedValue = DateTime.UtcNow;
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.Equal(expectedValue, actualValue);
		}

		[Fact]
		public void WriteReadLocalDateMsgPack()
		{
			var expectedValue = DateTime.Now;
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.Equal(expectedValue, actualValue);
		}

		[Fact]
		public void WriteReadUnspecifiedDateMsgPack()
		{
			var expectedValue = new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified);
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.Equal(expectedValue, actualValue);
		}

		[Fact]
		public void WriteReadDateTimeOffsetLocalMsgPack()
		{
			var expectedValue = DateTimeOffset.Now;
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.Equal(expectedValue, actualValue);
		}

		[Fact]
		public void WriteReadDateTimeOffsetUtcMsgPack()
		{
			var expectedValue = DateTimeOffset.UtcNow;
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.Equal(expectedValue, actualValue);
		}

		[Fact]
		public void WriteReadDecimalMaxMsgPack()
		{
			var expectedValue = decimal.MaxValue;
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.Equal(expectedValue, actualValue);
		}

		[Fact]
		public void WriteReadDecimalMinMsgPack()
		{
			var expectedValue = decimal.MinValue;
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.Equal(expectedValue, actualValue);
		}

		[Fact]
		public void WriteReadDecimalOneMsgPack()
		{
			var expectedValue = decimal.One;
			var actualValue = this.WriteReadMessagePack(expectedValue);

			Assert.Equal(expectedValue, actualValue);
		}
	}
}
