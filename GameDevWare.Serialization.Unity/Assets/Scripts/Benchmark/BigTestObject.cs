/* 
	Copyright (c) 2016 Denis Zykov, GameDevWare.com

	This a part of "Json & MessagePack Serialization" Unity Asset - https://www.assetstore.unity3d.com/#!/content/59918

	THIS SOFTWARE IS DISTRIBUTED "AS-IS" WITHOUT ANY WARRANTIES, CONDITIONS AND 
	REPRESENTATIONS WHETHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION THE 
	IMPLIED WARRANTIES AND CONDITIONS OF MERCHANTABILITY, MERCHANTABLE QUALITY, 
	FITNESS FOR A PARTICULAR PURPOSE, DURABILITY, NON-INFRINGEMENT, PERFORMANCE 
	AND THOSE ARISING BY STATUTE OR FROM CUSTOM OR USAGE OF TRADE OR COURSE OF DEALING.
	
	This source code is distributed via Unity Asset Store, 
	to use it in your project you should accept Terms of Service and EULA 
	https://unity3d.com/ru/legal/as_terms
*/
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Benchmark
{
	public class BigTestObject : IEquatable<BigTestObject>, ITestObject
	{
		public byte ByteField { get; set; }
		public sbyte SByteField { get; set; }
		public short Int16Field { get; set; }
		public ushort UInt16Field { get; set; }
		public int Int32Field { get; set; }
		public uint UInt32Field { get; set; }
		public long Int64Field { get; set; }
		public ulong UInt64Field { get; set; }
		public decimal DecimalField { get; set; }
		public float SingleFiled { get; set; }
		public double DoubleField { get; set; }
		public string StringField { get; set; }
		public ConsoleColor MyEnumField { get; set; }
		public bool BooleanField { get; set; }
		public DateTime DateTimeField { get; set; }
		public DateTimeOffset DateTimeOffsetField { get; set; }
		public Guid GuidField { get; set; }
		public TimeSpan TimeSpanField { get; set; }
		public Uri UrlField { get; set; }
		public string NullField { get; set; }
		public int[] IntArrayField { get; set; }
		public string[] StringArrayField { get; set; }
		public int[] EmptyArrayField { get; set; }
		public int[] NullArrayField { get; set; }
		public Dictionary<string, object> DictionaryArrayField { get; set; }
		public object[] ObjectArrayField { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as BigTestObject;
			return this.Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = ByteField.GetHashCode();
				hashCode = (hashCode * 397) ^ SByteField.GetHashCode();
				hashCode = (hashCode * 397) ^ Int16Field.GetHashCode();
				hashCode = (hashCode * 397) ^ UInt16Field.GetHashCode();
				hashCode = (hashCode * 397) ^ Int32Field;
				hashCode = (hashCode * 397) ^ (int)UInt32Field;
				hashCode = (hashCode * 397) ^ Int64Field.GetHashCode();
				hashCode = (hashCode * 397) ^ UInt64Field.GetHashCode();
				hashCode = (hashCode * 397) ^ DecimalField.GetHashCode();
				hashCode = (hashCode * 397) ^ SingleFiled.GetHashCode();
				hashCode = (hashCode * 397) ^ DoubleField.GetHashCode();
				hashCode = (hashCode * 397) ^ (StringField != null ? StringField.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (int)MyEnumField;
				hashCode = (hashCode * 397) ^ BooleanField.GetHashCode();
				hashCode = (hashCode * 397) ^ DateTimeField.GetHashCode();
				hashCode = (hashCode * 397) ^ DateTimeOffsetField.GetHashCode();
				hashCode = (hashCode * 397) ^ GuidField.GetHashCode();
				hashCode = (hashCode * 397) ^ TimeSpanField.GetHashCode();
				hashCode = (hashCode * 397) ^ (UrlField != null ? UrlField.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (NullField != null ? NullField.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (IntArrayField != null ? IntArrayField.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (StringArrayField != null ? StringArrayField.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (EmptyArrayField != null ? EmptyArrayField.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (NullArrayField != null ? NullArrayField.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (DictionaryArrayField != null ? DictionaryArrayField.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ObjectArrayField != null ? ObjectArrayField.GetHashCode() : 0);
				return hashCode;
			}
		}

		public void Fill()
		{
			ByteField = 1;
			SByteField = 2;
			Int16Field = 3;
			UInt16Field = 4;
			Int32Field = 5;
			UInt32Field = 6;
			Int64Field = 7;
			UInt64Field = 8;
			DecimalField = 9;
			SingleFiled = 10;
			DoubleField = 11;
			StringField = "12";
			MyEnumField = ConsoleColor.Cyan;
			BooleanField = true;
			DateTimeField = DateTime.UtcNow;
			DateTimeOffsetField = DateTime.UtcNow;
			GuidField = Guid.NewGuid();
			TimeSpanField = TimeSpan.FromHours(12.3);
			UrlField = new Uri("http://sample.com/fr.ttt?hello=world");
			NullField = null;
			IntArrayField = new int[] { 1, 2, 3 };
			StringArrayField = new string[] { "1", "2" };
			EmptyArrayField = new int[0];
			NullArrayField = null;
			DictionaryArrayField = new Dictionary<string, object> { { "a", 0 }, { "b", "c" } };
			ObjectArrayField = new object[] { new object[] { new object() } };
		}

		public bool Equals(BigTestObject other)
		{
			if (other == null)
				return false;

			return this.ByteField == other.ByteField &&
				   this.SByteField == other.SByteField &&
				   this.Int16Field == other.Int16Field &&
				   this.UInt16Field == other.UInt16Field &&
				   this.Int32Field == other.Int32Field &&
				   this.UInt32Field == other.UInt32Field &&
				   this.Int64Field == other.Int64Field &&
				   this.UInt64Field == other.UInt64Field &&
				   this.DecimalField == other.DecimalField &&
				   Math.Abs(this.SingleFiled - other.SingleFiled) < float.Epsilon &&
				   Math.Abs(this.DoubleField - other.DoubleField) < double.Epsilon &&
				   this.StringField == other.StringField &&
				   this.MyEnumField == other.MyEnumField &&
				   this.BooleanField == other.BooleanField &&
				   new TimeSpan(Math.Abs((this.DateTimeField - other.DateTimeField).Ticks)) < TimeSpan.FromSeconds(1) &&
				   new TimeSpan(Math.Abs((this.DateTimeOffsetField - other.DateTimeOffsetField).Ticks)) < TimeSpan.FromSeconds(1) &&
				   this.GuidField == other.GuidField &&
				   this.TimeSpanField == other.TimeSpanField &&
				   this.UrlField.Equals(other.UrlField) &&
				   this.NullField == other.NullField &&
				   this.IntArrayField.Length == other.IntArrayField.Length &&
				   this.StringArrayField.Length == other.StringArrayField.Length &&
				   this.EmptyArrayField.Length == other.EmptyArrayField.Length &&
				   this.NullArrayField == other.NullArrayField &&
				   this.DictionaryArrayField.Count == other.DictionaryArrayField.Count &&
				   this.ObjectArrayField.Length == other.ObjectArrayField.Length;
		}
	}
}
