using System.Diagnostics;
using System.IO;
using Xunit;

namespace GameDevWare.Serialization.Tests
{
	public abstract class TestBase
	{
		protected T WriteReadMessagePack<T>(T value)
		{
			var writeContext = new SerializationContext();
			var stream = new MemoryStream();
			MsgPack.Serialize(value, stream, writeContext);

			Assert.Empty(writeContext.Path);
			Assert.Empty(writeContext.Hierarchy);

			stream.Position = 0;
			Debug.WriteLine(new MessagePack.MsgPackReader(stream, new SerializationContext()).DebugPrintTokens());

			var readContext = new SerializationContext();
			stream.Position = 0;
			var readValue = (T)MsgPack.Deserialize(typeof(T), stream, readContext);

			Assert.Empty(readContext.Path);
			Assert.Empty(readContext.Hierarchy);

			return readValue;
		}

		protected T WriteReadJson<T>(T value)
		{
			var stream = new MemoryStream();
			Json.Serialize(value, stream);
			stream.Position = 0;

			var json = Json.DefaultEncoding.GetString(stream.ToArray());
			Trace.WriteLine(json);
			Debug.WriteLine(new JsonStreamReader(stream, new SerializationContext()).DebugPrintTokens());

			stream.Position = 0;
			var readValue = (T)Json.Deserialize(typeof(T), stream);
			return readValue;
		}
	}
}
