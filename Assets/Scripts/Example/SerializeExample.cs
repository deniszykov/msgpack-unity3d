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
