using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace GameDevWare.Serialization.Tests
{
	public class JsonWriteReadTests : TestBase
	{
		[Fact]
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

			Assert.NotNull(actualValue);
			Assert.Equal(expectedValue, actualValue);
		}

		[Fact]
		public void ReadDictionaryAsJsonArrayOfPairs()
		{
			var json = Json.SerializeToString(new[] { new DictionaryEntry("a", 1), new DictionaryEntry("b", 2), new DictionaryEntry("c", 3) });
			var expected = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } };
			var actual = Json.Deserialize<Dictionary<string, int>>(json);

			Trace.WriteLine(json);

			Assert.NotNull(actual);
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void ReadDictionaryAsJsonArrayOfArrays()
		{
			var json = Json.SerializeToString(new[] { new object[] { "a", 1 }, new object[] { "b", 2 }, new object[] { "c", 3 } });
			var expected = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } };
			var actual = Json.Deserialize<Dictionary<string, int>>(json);

			Trace.WriteLine(json);

			Assert.NotNull(actual);
			Assert.Equal(expected, actual);
		}
	}
}
