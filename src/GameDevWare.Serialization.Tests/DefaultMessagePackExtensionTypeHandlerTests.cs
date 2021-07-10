using System;
using GameDevWare.Serialization.MessagePack;
using NUnit.Framework;

namespace GameDevWare.Serialization.Tests
{
	[TestFixture, Category("DefaultMessagePackExtensionTypeHandlerTests")]
	public class DefaultMessagePackExtensionTypeHandlerTests
	{
		[Test]
		public void TryReadWriteDateTimeLocal()
		{
			var expectedValue = DateTime.Now;
			var writeBuffer = new ArraySegment<byte>(new byte[1024]);
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var actualValue);

			Assert.IsInstanceOf<DateTime>(actualValue);
			Assert.AreEqual(expectedValue, (DateTime)actualValue);
			Assert.AreEqual(expectedValue.Kind, ((DateTime)actualValue).Kind);
		}

		[Test]
		public void TryReadWriteDateTimeUtc()
		{
			var expectedValue = DateTime.UtcNow;
			var writeBuffer = new ArraySegment<byte>(new byte[1024]);
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var actualValue);

			Assert.IsInstanceOf<DateTime>(actualValue);
			Assert.AreEqual(expectedValue, (DateTime)actualValue);
			Assert.AreEqual(expectedValue.Kind, ((DateTime)actualValue).Kind);
		}

		[Test]
		public void TryReadWriteDateTimeUnspecified()
		{
			var expectedValue = new DateTime(DateTime.UtcNow.Ticks, DateTimeKind.Unspecified);
			var writeBuffer = new ArraySegment<byte>(new byte[1024]);
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var actualValue);

			Assert.AreEqual(expectedValue, (DateTime)actualValue);
			Assert.AreEqual(expectedValue.Kind, ((DateTime)actualValue).Kind);
		}

		[Test]
		public void TryReadWriteDateTimeMax()
		{
			var expectedValue = DateTime.MaxValue;
			var writeBuffer = new ArraySegment<byte>(new byte[1024]);
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var actualValue);

			Assert.IsInstanceOf<DateTime>(actualValue);
			Assert.AreEqual(expectedValue, (DateTime)actualValue);
			Assert.AreEqual(expectedValue.Kind, ((DateTime)actualValue).Kind);
		}


		[Test]
		public void TryReadWriteDateTimeMin()
		{
			var expectedValue = DateTime.MinValue;
			var writeBuffer = new ArraySegment<byte>(new byte[1024]);
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var actualValue);

			Assert.IsInstanceOf<DateTime>(actualValue);
			Assert.AreEqual(expectedValue, (DateTime)actualValue);
			Assert.AreEqual(expectedValue.Kind, ((DateTime)actualValue).Kind);
		}

		[Test]
		public void TryReadWriteDateTimeExact()
		{
			var expectedValue = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Local);
			var writeBuffer = new ArraySegment<byte>();
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var now);

			Assert.IsInstanceOf<DateTime>(now);
			Assert.AreEqual(expectedValue, (DateTime)now);
		}

		[Test]
		public void TryReadWriteDateTimeOffset()
		{
			var expectedValue = DateTimeOffset.Now;
			var writeBuffer = new ArraySegment<byte>(new byte[1024]);
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var actualValue);

			Assert.IsInstanceOf<DateTimeOffset>(actualValue);
			Assert.AreEqual(expectedValue, (DateTimeOffset)actualValue);
		}

		[Test]
		public void TryReadWriteDecimalMax()
		{
			var expectedValue = decimal.MaxValue;
			var writeBuffer = new ArraySegment<byte>(new byte[1024]);
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var actualValue);

			Assert.IsInstanceOf<decimal>(actualValue);
			Assert.AreEqual(expectedValue, (decimal)actualValue);
		}

		[Test]
		public void TryReadWriteDecimalMin()
		{
			var expectedValue = decimal.MinValue;
			var writeBuffer = new ArraySegment<byte>(new byte[1024]);
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var actualValue);

			Assert.IsInstanceOf<decimal>(actualValue);
			Assert.AreEqual(expectedValue, (decimal)actualValue);
		}

		[Test]
		public void TryReadWriteGuid()
		{
			var expectedValue = Guid.NewGuid();
			var writeBuffer = new ArraySegment<byte>(new byte[1024]);
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var actualValue);

			Assert.IsInstanceOf<Guid>(actualValue);
			Assert.AreEqual(expectedValue, (Guid)actualValue);
		}

		[Test]
		public void TryReadWriteMessagePackTimestamp()
		{
			var expectedValue = new MessagePackTimestamp(1500, 1500);
			var writeBuffer = new ArraySegment<byte>(new byte[1024]);
			DefaultMessagePackExtensionTypeHandler.Instance.TryWrite(expectedValue, out var type, ref writeBuffer);
			DefaultMessagePackExtensionTypeHandler.Instance.TryRead(type, writeBuffer, out var actualValue);

			Assert.IsInstanceOf<MessagePackTimestamp>(actualValue);
			Assert.AreEqual(expectedValue, (MessagePackTimestamp)actualValue);
		}
	}
}
