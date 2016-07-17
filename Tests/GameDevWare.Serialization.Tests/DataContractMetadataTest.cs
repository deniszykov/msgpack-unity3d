using System.IO;
using System.Runtime.Serialization;
using Xunit;

namespace GameDevWare.Serialization.Tests
{
	public class DataContractMetadataTest
	{
		[DataContract]
		public class ClassWithPrivateMembers
		{
			[DataMember]
			private int field1 { get; set; }
			[DataMember]
			public int field2;
			private int field3;
			public int field4 { get; set; }

			private ClassWithPrivateMembers()
			{

			}

			public static ClassWithPrivateMembers Create()
			{
				return new ClassWithPrivateMembers();
			}

			public void Init()
			{
				this.field1 = 1;
				this.field2 = 2;
				this.field3 = 3;
				this.field4 = 4;
			}

			public bool Test(object obj)
			{
				var other = obj as ClassWithPrivateMembers;
				if (other == null) return false;
				return this.field1 == other.field1 && this.field2 == other.field2 && this.field3 == 0 && this.field4 == 0;
			}

			public override string ToString()
			{
				return "{" + this.field1 + ", " + this.field2 + ", " + this.field3 + ", " + this.field4 + "}";
			}
		}
		public class ClassWithSerializablePublicMembers
		{
			private int field1 { get; set; }
			public int field2;
			private int field3;
			public virtual int field4 { get; set; }

			public virtual void Init()
			{
				this.field1 = 1;
				this.field2 = 2;
				this.field3 = 3;
				this.field4 = 4;
			}

			public virtual bool Test(object obj)
			{
				var other = obj as ClassWithSerializablePublicMembers;
				if (other == null) return false;
				return this.field1 == 0 && this.field2 == other.field2 && this.field3 == 0 && this.field4 == other.field4;
			}

			public override string ToString()
			{
				return "{" + this.field1 + ", " + this.field2 + ", " + this.field3 + ", " + this.field4 + "}";
			}
		}
		[DataContract]
		public class ClassWithDerivedMixedContract : ClassWithSerializablePublicMembers
		{
			[DataMember]
			private int field5;
			public int field6;

			public override void Init()
			{
				base.Init();

				this.field5 = 5;
				this.field6 = 6;
			}

			public override bool Test(object obj)
			{
				var other = obj as ClassWithDerivedMixedContract;
				if (other == null) return false;
				return base.Test(obj) && this.field5 == other.field5 && this.field6 == 0;
			}

			public override string ToString()
			{
				return base.ToString() + " {" + this.field5 + ", " + this.field6 + "}";
			}
		}
		[DataContract]
		public class ClassWithDerivedContractAndOverriddenProperty : ClassWithSerializablePublicMembers
		{
			[DataMember]
			public int field5;
			[DataMember]
			public override int field4 { get; set; }

			public override void Init()
			{
				base.Init();
				this.field5 = 5;
				this.field4 = 6;
			}

			public override bool Test(object obj)
			{
				var other = obj as ClassWithDerivedContractAndOverriddenProperty;
				if (other == null) return false;
				return base.Test(obj) && this.field5 == other.field5;
			}

			public override string ToString()
			{
				return base.ToString() + " {" + this.field5 + "}";
			}
		}

		[Fact]
		public void PrivateMembersSerializationTest()
		{
			var expected = ClassWithPrivateMembers.Create();
			expected.Init();
			var stream = new MemoryStream();

			MsgPack.Serialize(expected, stream);
			stream.Position = 0;

			var actual = MsgPack.Deserialize<ClassWithPrivateMembers>(stream);
			Assert.True(actual.Test(expected), "Actual object is not valid: " + actual + ", this one expected: " + expected);
		}

		[Fact]
		public void PublicMembersSerializationTest()
		{
			var expected = new ClassWithSerializablePublicMembers();
			expected.Init();
			var stream = new MemoryStream();

			MsgPack.Serialize(expected, stream);
			stream.Position = 0;

			var actual = MsgPack.Deserialize<ClassWithSerializablePublicMembers>(stream);
			Assert.True(actual.Test(expected), "Actual object is not valid: " + actual + ", this one expected: " + expected);
		}

		[Fact]
		public void MixedContractSerializationTest()
		{
			var expected = new ClassWithDerivedMixedContract();
			expected.Init();
			var stream = new MemoryStream();

			MsgPack.Serialize(expected, stream);
			stream.Position = 0;

			var actual = MsgPack.Deserialize<ClassWithDerivedMixedContract>(stream);
			Assert.True(actual.Test(expected), "Actual object is not valid: " + actual + ", this one expected: " + expected);
		}

		[Fact]
		public void OverriddenPropertySerializationTest()
		{
			var expected = new ClassWithDerivedContractAndOverriddenProperty();
			expected.Init();
			var stream = new MemoryStream();

			MsgPack.Serialize(expected, stream);
			stream.Position = 0;

			var actual = MsgPack.Deserialize<ClassWithDerivedContractAndOverriddenProperty>(stream);
			Assert.True(actual.Test(expected), "Actual object is not valid: " + actual + ", this one expected: " + expected);
		}
	}
}
