using UnityEngine;

namespace Assets.Scripts.Benchmark
{
	public class MediumTestObject : ITestObject
	{
		public Vector3 Position { get; set; }
		public Quaternion Rotation { get; set; }

		public void Fill()
		{
			this.Position = new Vector3(10, 100, 1000);
			this.Rotation = new Quaternion(1, 100, 1000, 10000);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as MediumTestObject);
		}

		protected bool Equals(MediumTestObject other)
		{
			if (other == null) return false;

			return Position.Equals(other.Position) && Rotation.Equals(other.Rotation);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Position.GetHashCode() * 397) ^ Rotation.GetHashCode();
			}
		}
	}
}
