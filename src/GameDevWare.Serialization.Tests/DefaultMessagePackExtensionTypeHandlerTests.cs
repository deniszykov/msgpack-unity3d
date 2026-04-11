using System;
using GameDevWare.Serialization.MessagePack;
using Xunit;

namespace GameDevWare.Serialization.Tests
{
	public class DefaultMessagePackExtensionTypeHandlerTests
	{
		[Fact]
		public void TryReadWriteDateTimeLocal()
		{
			var expectedValue = DateTime.Now;
			var writeBuffer = new ArraySegment<byte>(new byte[1024]);
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var actualValue);

			Assert.IsType<DateTime>(actualValue);
			Assert.Equal(expectedValue, (DateTime)actualValue);
			Assert.Equal(expectedValue.Kind, ((DateTime)actualValue).Kind);
		}

		[Fact]
		public void TryReadWriteDateTimeUtc()
		{
			var expectedValue = DateTime.UtcNow;
			var writeBuffer = new ArraySegment<byte>(new byte[1024]);
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var actualValue);

			Assert.IsType<DateTime>(actualValue);
			Assert.Equal(expectedValue, (DateTime)actualValue);
			Assert.Equal(expectedValue.Kind, ((DateTime)actualValue).Kind);
		}

		[Fact]
		public void TryReadWriteDateTimeUnspecified()
		{
			var expectedValue = new DateTime(DateTime.UtcNow.Ticks, DateTimeKind.Unspecified);
			var writeBuffer = new ArraySegment<byte>(new byte[1024]);
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var actualValue);

			Assert.Equal(expectedValue, (DateTime)actualValue);
			Assert.Equal(expectedValue.Kind, ((DateTime)actualValue).Kind);
		}

		[Fact]
		public void TryReadWriteDateTimeMax()
		{
			var expectedValue = DateTime.MaxValue;
			var writeBuffer = new ArraySegment<byte>(new byte[1024]);
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var actualValue);

			Assert.IsType<DateTime>(actualValue);
			Assert.Equal(expectedValue, (DateTime)actualValue);
			Assert.Equal(expectedValue.Kind, ((DateTime)actualValue).Kind);
		}


		[Fact]
		public void TryReadWriteDateTimeMin()
		{
			var expectedValue = DateTime.MinValue;
			var writeBuffer = new ArraySegment<byte>(new byte[1024]);
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var actualValue);

			Assert.IsType<DateTime>(actualValue);
			Assert.Equal(expectedValue, (DateTime)actualValue);
			Assert.Equal(expectedValue.Kind, ((DateTime)actualValue).Kind);
		}

		[Fact]
		public void TryReadWriteDateTimeExact()
		{
			var expectedValue = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local);
			var writeBuffer = new ArraySegment<byte>();
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var now);

			Assert.IsType<DateTime>(now);
			Assert.Equal(expectedValue, (DateTime)now);
		}

		[Fact]
		public void TryReadWriteDateTimeOffset()
		{
			var expectedValue = DateTimeOffset.Now;
			var writeBuffer = new ArraySegment<byte>(new byte[1024]);
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var actualValue);

			Assert.IsType<DateTimeOffset>(actualValue);
			Assert.Equal(expectedValue, (DateTimeOffset)actualValue);
		}

		[Fact]
		public void TryReadWriteDecimalMax()
		{
			var expectedValue = decimal.MaxValue;
			var writeBuffer = new ArraySegment<byte>(new byte[1024]);
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var actualValue);

			Assert.IsType<decimal>(actualValue);
			Assert.Equal(expectedValue, (decimal)actualValue);
		}

		[Fact]
		public void TryReadWriteDecimalMin()
		{
			var expectedValue = decimal.MinValue;
			var writeBuffer = new ArraySegment<byte>(new byte[1024]);
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var actualValue);

			Assert.IsType<decimal>(actualValue);
			Assert.Equal(expectedValue, (decimal)actualValue);
		}

		[Fact]
		public void TryReadWriteGuid()
		{
			var expectedValue = Guid.NewGuid();
			var writeBuffer = new ArraySegment<byte>(new byte[1024]);
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var actualValue);

			Assert.IsType<Guid>(actualValue);
			Assert.Equal(expectedValue, (Guid)actualValue);
		}

		[Fact]
		public void TryReadWriteMessagePackTimestamp()
		{
			var expectedValue = new MessagePackTimestamp(1500, 1500);
			var writeBuffer = new ArraySegment<byte>(new byte[1024]);
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var actualValue);

			Assert.IsType<MessagePackTimestamp>(actualValue);
			Assert.Equal(expectedValue, (MessagePackTimestamp)actualValue);
		}
	}
}
