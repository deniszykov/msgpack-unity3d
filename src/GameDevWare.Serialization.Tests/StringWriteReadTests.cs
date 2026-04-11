using System;
using Xunit;

namespace GameDevWare.Serialization.Tests
{
	public class StringWriteReadTests : TestBase
	{
		[Theory]
		[InlineData("")]
		[InlineData("simple string")]
		[InlineData("string with \"quote\"")]
		[InlineData("string with \\backslash\\")]
		[InlineData("string with \n newline")]
		[InlineData("string with \r return")]
		[InlineData("string with \t tab")]
		[InlineData("string with \b backspace")]
		[InlineData("string with \f formfeed")]
		[InlineData("Unicode: \u1234 \u5678")]
		[InlineData("Emoji: 🌍🚀")]
		public void StringRoundtrip(string value)
		{
			Assert.Equal(value, WriteReadMessagePack(value));
			Assert.Equal(value, WriteReadJson(value));
		}

		[Fact]
		public void LargeStringRoundtrip()
		{
			var value = new string('A', 10000);
			Assert.Equal(value, WriteReadMessagePack(value));
			Assert.Equal(value, WriteReadJson(value));
		}

		[Theory]
		[InlineData("\"\\u0041\"", "A")] // Unicode escape
		[InlineData("\"\\\"\"", "\"")] // Escaped quote
		[InlineData("\"\\\\\"", "\\")] // Escaped backslash
		[InlineData("\"\\/\"", "/")] // Escaped forward slash
		[InlineData("\"\\b\"", "\b")] // Escaped backspace
		[InlineData("\"\\f\"", "\f")] // Escaped formfeed
		[InlineData("\"\\n\"", "\n")] // Escaped newline
		[InlineData("\"\\r\"", "\r")] // Escaped carriage return
		[InlineData("\"\\t\"", "\t")] // Escaped tab
		public void JsonStringUnescaping(string json, string expected)
		{
			var actual = Json.Deserialize<string>(json);
			Assert.Equal(expected, actual);
		}
	}
}
