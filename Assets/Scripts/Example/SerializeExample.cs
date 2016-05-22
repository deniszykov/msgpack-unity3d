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

using System.Collections.Generic;
using System.IO;
using GameDevWare.Serialization;
using UnityEngine;

namespace Assets.Scripts.Example
{
	public static class SerializeExample
	{
		public static void SerializeToMemoryStream()
		{
			var stream = new MemoryStream();

			MsgPack.Serialize(new { x = 1, y = 2 }, stream);

			MsgPack.Deserialize<Vector2>(stream);

			Json.Serialize(new { pos = new Vector3(1, 1, 1), rot = new Quaternion(1, 1, 1, 1) }, stream);

			stream.Position = 0;

			var myObject = Json.Deserialize<Dictionary<string, object>>(stream);
			var pos = (Vector3)myObject["pos"];
			var rot = (Quaternion)myObject["rot"];
		}
	}
}
