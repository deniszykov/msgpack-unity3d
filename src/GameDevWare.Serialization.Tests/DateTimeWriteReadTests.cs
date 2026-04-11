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
			var value = kind == DateTimeKind.Unspecified ? new DateTime(DateTime.Now.Ticks, DateTimeKind.Unspecified) : kind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now;

			// MessagePack
			var msgPackActual = WriteReadMessagePack(value);
			Assert.Equal(value.ToUniversalTime(), msgPackActual.ToUniversalTime());

			// JSON
			var jsonActual = WriteReadJson(value);
			Assert.Equal(value.ToUniversalTime(), jsonActual.ToUniversalTime());
		}

		[Fact]
		public void DateTimeOffsetRoundtrip()
		{
			var value = DateTimeOffset.Now;

			// MessagePack
			var msgPackActual = WriteReadMessagePack(value);
			Assert.Equal(value, msgPackActual);

			// JSON
			var jsonActual = WriteReadJson(value);
			Assert.Equal(value, jsonActual);
		}

		[Fact]
		public void DateTimeOffsetPrecisionRoundtrip()
		{
			var value = new DateTimeOffset(2026, 4, 11, 23, 59, 59, 123, TimeSpan.FromHours(5)).AddTicks(4567);

			// MessagePack
			var msgPackActual = WriteReadMessagePack(value);
			Assert.Equal(value.Ticks, msgPackActual.Ticks);
			Assert.Equal(value.Offset, msgPackActual.Offset);

			// JSON
			var jsonActual = WriteReadJson(value);
			Assert.Equal(value.Ticks, jsonActual.Ticks);
			Assert.Equal(value.Offset, jsonActual.Offset);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(5.5)]
		[InlineData(-10)]
		public void DateTimeOffsetTimezoneRoundtrip(double offsetHours)
		{
			var offset = TimeSpan.FromHours(offsetHours);
			var value = new DateTimeOffset(2026, 4, 11, 12, 0, 0, offset);

			// MessagePack
			var msgPackActual = WriteReadMessagePack(value);
			Assert.Equal(value, msgPackActual);

			// JSON
			var jsonActual = WriteReadJson(value);
			Assert.Equal(value, jsonActual);
		}

		[Fact]
		public void LegacyDateTimeRoundtrip()
		{
			var date = new DateTime(2016, 01, 02, 03, 04, 05, DateTimeKind.Utc);
			var ticks = (date.Ticks - JsonUtils.UnixEpochTicks) / 10000;
			var json = string.Format("\"/Date({0})/\"", ticks);
			var actual = Json.Deserialize<DateTime>(json);
			Assert.Equal(date, actual);
		}
	}
}
