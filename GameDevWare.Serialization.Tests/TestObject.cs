using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace GameDevWare.Serialization.Tests
{
	public class TestObject
	{
		private sealed class Comparer : IEqualityComparer<object>
		{
			public static readonly IEqualityComparer<object> Default = new Comparer();

			bool IEqualityComparer<object>.Equals(object a, object b)
			{
				var areEquals = false;
				if (ReferenceEquals(a, b))
					areEquals = true;
				else if (a == null || b == null)
					areEquals = false;
				else if (a is string && b is string)
					areEquals = a.Equals(b);
				else if (a.GetType() != b.GetType() && TryChangeType(b.GetType(), ref a))
					areEquals = a.Equals(b);
				else if (a.GetType() != b.GetType() && TryChangeType(a.GetType(), ref b))
					areEquals = a.Equals(b);
				else if (a is IEnumerable && b is IEnumerable)
					areEquals = ((IEnumerable)a).Cast<object>().SequenceEqual(((IEnumerable)b).Cast<object>(), Default);
				else if (a is DateTime && b is DateTime)
				{
					var aTicks = ((DateTime)a).ToUniversalTime().Ticks;
					var bTicks = ((DateTime)b).ToUniversalTime().Ticks;
					areEquals = aTicks - (aTicks % TimeSpan.TicksPerSecond) == bTicks - (bTicks % TimeSpan.TicksPerSecond);
				}
				else if (a is DateTimeOffset && b is DateTimeOffset)
				{
					var aTicks = ((DateTimeOffset)a).Ticks;
					var bTicks = ((DateTimeOffset)b).Ticks;
					areEquals = aTicks - (aTicks % TimeSpan.TicksPerSecond) == bTicks - (bTicks % TimeSpan.TicksPerSecond);
				}
				else if (a.GetType() == typeof(object) && b.GetType() == typeof(object))
				    return true;
				else
					areEquals = a.Equals(b);

				if (areEquals == false && Debugger.IsAttached)
					Debugger.Break();

				return areEquals;
			}
			int IEqualityComparer<object>.GetHashCode(object obj)
			{
				if (obj is IEnumerable)
					return ((IEnumerable)obj).Cast<object>().Aggregate(0, (s, v) => unchecked(s + Default.GetHashCode(v)));
				return obj == null ? 0 : obj.GetHashCode();
			}
		}

		public int IntField;
		public int IntProperty { get; set; }
		public short ShortField;
		public long LongField;
		public double DoubleField;
		public float SingleField;
		public decimal DecimalField;
		public int[] IntArrayProperty { get; set; }
		public TestObject ObjectProperty { get; set; }
		public TestObject[] ObjectArrayProperty { get; set; }
		public object[] MixedArrayProperty { get; set; }
		public string[] StringArrayProperty { get; set; }
		public string StringProperty { get; set; }
		public bool BoolProperty { get; set; }
		public DateTime DateProperty { get; set; }
		public DateTimeOffset DateOffsetProperty { get; set; }
		public long? NullableProperty { get; set; }
		public object AnyProperty { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as TestObject;
			if (other == null)
				return false;

			return this.IntField == other.IntField &&
				   this.IntProperty == other.IntProperty &&
				   this.LongField == other.LongField &&
				   Math.Abs(this.DoubleField - other.DoubleField) < double.Epsilon &&
				   Math.Abs(this.SingleField - other.SingleField) < float.Epsilon &&
				   this.ShortField == other.ShortField &&
				   this.DecimalField == other.DecimalField &&
				   this.StringProperty == other.StringProperty &&
				   Comparer.Default.Equals(this.DateProperty, other.DateProperty) &&
				   Comparer.Default.Equals(this.DateOffsetProperty, other.DateOffsetProperty) &&
				   this.BoolProperty == other.BoolProperty &&
				   this.NullableProperty == other.NullableProperty &&
				   Comparer.Default.Equals(this.AnyProperty, other.AnyProperty) &&
				   Comparer.Default.Equals(this.IntArrayProperty, other.IntArrayProperty) &&
				   Comparer.Default.Equals(this.MixedArrayProperty, other.MixedArrayProperty) &&
				   Comparer.Default.Equals(this.ObjectProperty, other.ObjectProperty) &&
				   Comparer.Default.Equals(this.ObjectArrayProperty, other.ObjectArrayProperty) &&
				   Comparer.Default.Equals(this.StringArrayProperty, other.StringArrayProperty);
		}
		public override int GetHashCode()
		{
			return unchecked(this.IntField + this.IntField +
				ShortField + LongField.GetHashCode() +
				DoubleField.GetHashCode() +
				SingleField.GetHashCode() +
				DecimalField.GetHashCode() +
				(this.StringProperty ?? "").GetHashCode() +
				DateProperty.GetHashCode() +
				DateOffsetProperty.GetHashCode() +
				(AnyProperty ?? string.Empty).GetHashCode()
			);
		}

		private static bool TryChangeType(Type toType, ref object value)
		{
			try
			{
				if (toType.IsEnum)
					value = Enum.ToObject(toType, Convert.ChangeType(value, Enum.GetUnderlyingType(toType)));
				else if (toType == typeof(DateTime) && value is string)
					value = DateTime.Parse((string)value, CultureInfo.InvariantCulture);
				else if (toType == typeof(DateTimeOffset) && value is string)
					value = DateTimeOffset.Parse((string)value, CultureInfo.InvariantCulture);
				else
					value = Convert.ChangeType(value, toType);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
