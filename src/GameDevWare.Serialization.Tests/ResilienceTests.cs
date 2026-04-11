using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Xunit;

namespace GameDevWare.Serialization.Tests
{
	public class ResilienceTests : TestBase
	{
		[Fact]
		public void CircularReferenceMsgPack()
		{
			var obj = new TestObject();
			obj.ObjectProperty = obj; // circular

			Assert.Throws<SerializationException>(() =>
			{
				MsgPack.Serialize(obj, new MemoryStream());
			});
		}

		[Fact]
		public void CircularReferenceJson()
		{
			var obj = new TestObject();
			obj.ObjectProperty = obj; // circular

			Assert.Throws<SerializationException>(() =>
			{
				Json.Serialize(obj, new MemoryStream());
			});
		}

		[Fact]
		public void DeepNestingMsgPack()
		{
			var root = new List<object>();
			var current = root;
			for (int i = 0; i < 100; i++) // default is 64
			{
				var next = new List<object>();
				current.Add(next);
				current = next;
			}

			Assert.Throws<SerializationException>(() =>
			{
				MsgPack.Serialize(root, new MemoryStream());
			});
		}

		[Fact]
		public void DeepNestingJson()
		{
			var root = new List<object>();
			var current = root;
			for (int i = 0; i < 100; i++)
			{
				var next = new List<object>();
				current.Add(next);
				current = next;
			}

			Assert.Throws<SerializationException>(() =>
			{
				Json.Serialize(root, new MemoryStream());
			});
		}
	}
}
