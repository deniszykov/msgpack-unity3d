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
