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
		public static void SerializeToMsgPackMemoryStream()
		{
			try
			{
				var stream = new MemoryStream();
				
				// write vector to stream
				MsgPack.Serialize(new { x = 1, y = 2 }, stream);
				// reset stream's position
				stream.Position = 0;
				// read vector from stream
				var vec = MsgPack.Deserialize<Vector2>(stream);
				// 
				UnityEngine.Debug.Log("vec: " + vec);				
			}
			catch(System.Exception e)
			{
				UnityEngine.Debug.LogError(e);
			}
		}
		
		public static void SerializeToJsonMemoryStream()
		{
			try
			{
				var stream = new MemoryStream();
						
				// write object to stream
				Json.Serialize(new { pos = new Vector3(1, 1, 1), rot = new Quaternion(1, 1, 1, 1) }, stream);
				// reset stream's position
				stream.Position = 0;
				UnityEngine.Debug.Log( System.Text.Encoding.UTF8.GetString(stream.ToArray()) );
				
				// read object from stream
				var myObject = Json.Deserialize<Dictionary<string, object>>(stream);
				var pos = (IDictionary<string, object>)myObject["pos"];
				var rot = (IDictionary<string, object>)myObject["rot"];
				//
				UnityEngine.Debug.Log("pos x:y: " + pos["x"] + ":" + pos["y"]);
		
			}
			catch(System.Exception e)
			{
				UnityEngine.Debug.LogError(e);
			}
		}
	}
}
