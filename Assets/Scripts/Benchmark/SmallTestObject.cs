using System;

namespace Assets.Scripts.Benchmark
{
	public class SmallTestObject : ITestObject
	{
		public int X { get; set; }
		public int Y { get; set; }
		public float Rotation { get; set; }

		public void Fill()
		{
			this.X = 100;
			this.Y = 500;
			this.Rotation = 1.45f;
		}

		public override bool Equals(object obj)
		{
			var other = obj as SmallTestObject;
			if (other == null) return false;

			return this.X == other.X && this.Y == other.Y && Math.Abs(this.Rotation - other.Rotation) < float.Epsilon;
		}

		protected bool Equals(SmallTestObject other)
		{
			return X == other.X && Y == other.Y && Rotation.Equals(other.Rotation);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = X;
				hashCode = (hashCode*397) ^ Y;
				hashCode = (hashCode*397) ^ Rotation.GetHashCode();
				return hashCode;
			}
		}
	}
}
