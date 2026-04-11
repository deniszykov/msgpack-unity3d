using System;
using Xunit;

namespace GameDevWare.Serialization.Tests
{
	public class NumberWriteReadTests : TestBase
	{
		[Theory]
		[InlineData(0)]
		[InlineData(int.MaxValue)]
		[InlineData(int.MinValue)]
		[InlineData(123456)]
		public void Int32Roundtrip(int value)
		{
			Assert.Equal(value, WriteReadMessagePack(value));
			Assert.Equal(value, WriteReadJson(value));
		}

		[Theory]
		[InlineData(0L)]
		[InlineData(long.MaxValue)]
		[InlineData(long.MinValue)]
		public void Int64Roundtrip(long value)
		{
			Assert.Equal(value, WriteReadMessagePack(value));
			Assert.Equal(value, WriteReadJson(value));
		}

		[Theory]
		[InlineData(float.MaxValue)]
		[InlineData(float.MinValue)]
		[InlineData(0.0f)]
		[InlineData(1.2345f)]
		public void SingleRoundtrip(float value)
		{
			Assert.Equal(value, WriteReadMessagePack(value));
			Assert.Equal(value, WriteReadJson(value));
		}

		[Theory]
		[InlineData(double.MaxValue)]
		[InlineData(double.MinValue)]
		[InlineData(0.0d)]
		[InlineData(Math.PI)]
		public void DoubleRoundtrip(double value)
		{
			Assert.Equal(value, WriteReadMessagePack(value));
			Assert.Equal(value, WriteReadJson(value));
		}

		[Theory]
		[InlineData("79228162514264337593543950335")] // decimal.MaxValue
		[InlineData("-79228162514264337593543950335")] // decimal.MinValue
		[InlineData("0.0000000000000000000000000001")]
		public void DecimalRoundtrip(string valueStr)
		{
			var value = decimal.Parse(valueStr);
			Assert.Equal(value, WriteReadMessagePack(value));
			Assert.Equal(value, WriteReadJson(value));
		}

		[Theory]
		[InlineData("123", 123)]
		[InlineData(" 123 ", 123)]
		[InlineData("123.0", 123.0)]
		[InlineData("-123.456", -123.456)]
		[InlineData("1.23E2", 123.0)]
		public void JsonNumberParsing(string jsonValue, double expected)
		{
			var actual = Json.Deserialize<double>(jsonValue);
			Assert.Equal(expected, actual);
		}
	}
}
