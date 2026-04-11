using System;
using Xunit;

namespace GameDevWare.Serialization.Tests
{
	public class DateTimeWriteReadTests : TestBase
	{
		[Theory]
		[InlineData(DateTimeKind.Utc)]
		[InlineData(DateTimeKind.Local)]
		[InlineData(DateTimeKind.Unspecified)]
		public void DateTimeRoundtrip(DateTimeKind kind)
		{
			var value = kind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now;
			if (kind == DateTimeKind.Unspecified) value = DateTime.SpecifyKind(value, DateTimeKind.Unspecified);

			// MsgPack
			var msgPackActual = WriteReadMessagePack(value);
			Assert.Equal(value, msgPackActual);
			Assert.Equal(value.Kind, msgPackActual.Kind);

			// JSON
			var jsonActual = WriteReadJson(value);
			Assert.Equal(value.ToUniversalTime(), jsonActual.ToUniversalTime());
		}

		[Fact]
		public void DateTimeOffsetRoundtrip()
		{
			var value = DateTimeOffset.Now;
			Assert.Equal(value, WriteReadMessagePack(value));
			Assert.Equal(value, WriteReadJson(value));
		}

		[Theory]
		[InlineData("\"2023-01-01T12:00:00Z\"", 2023, 1, 1, 12, 0, 0, DateTimeKind.Utc)]
		[InlineData("\"2023-01-01 12:00:00\"", 2023, 1, 1, 12, 0, 0, DateTimeKind.Utc)]
		public void JsonDateTimeParsing(string json, int year, int month, int day, int hour, int min, int sec, DateTimeKind kind)
		{
			var expected = new DateTime(year, month, day, hour, min, sec, kind);
			var actual = Json.Deserialize<DateTime>(json);
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void JsonLegacyDateParsing()
		{
			var date = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc);
			var ticks = (date.Ticks - JsonUtils.UnixEpochTicks) / 10000;
			var json = string.Format("\"/Date({0})/\"", ticks);
			var actual = Json.Deserialize<DateTime>(json);
			Assert.Equal(date, actual);
		}
	}
}
