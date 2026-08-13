using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Xunit;

namespace GameDevWare.Serialization.Tests
{
	public class ResilienceTests : TestBase
	{
		/// <summary>
		/// Type overriding <see cref="object.Equals(object)"/> over a subset of its state, as is common
		/// for entity-style types compared by id. Two instances can be equal while being different objects.
		/// </summary>
		public class EqualByIdNode
		{
			public string Id;
			public EqualByIdNode Child;

			public override bool Equals(object obj)
			{
				return obj is EqualByIdNode && ((EqualByIdNode)obj).Id == this.Id;
			}
			public override int GetHashCode()
			{
				return this.Id == null ? 0 : this.Id.GetHashCode();
			}
		}

		[Fact]
		public void EqualButDistinctNestedObjectsAreNotCircularMsgPack()
		{
			var obj = new EqualByIdNode { Id = "1", Child = new EqualByIdNode { Id = "1" } };

			var readValue = this.WriteReadMessagePack(obj);

			Assert.NotNull(readValue.Child);
			Assert.Null(readValue.Child.Child);
		}

		[Fact]
		public void EqualButDistinctNestedObjectsAreNotCircularJson()
		{
			var obj = new EqualByIdNode { Id = "1", Child = new EqualByIdNode { Id = "1" } };

			var readValue = this.WriteReadJson(obj);

			Assert.NotNull(readValue.Child);
			Assert.Null(readValue.Child.Child);
		}

		[Fact]
		public void SameInstanceTwiceIsNotCircularMsgPack()
		{
			var shared = new EqualByIdNode { Id = "1" };
			var list = new List<EqualByIdNode> { shared, shared };

			var readValue = this.WriteReadMessagePack(list);

			Assert.Equal(2, readValue.Count);
		}

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
