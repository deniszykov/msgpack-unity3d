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
