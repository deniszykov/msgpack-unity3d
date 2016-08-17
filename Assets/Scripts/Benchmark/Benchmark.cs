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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Scripts.Benchmark;
using GameDevWare.Serialization;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class Benchmark : MonoBehaviour
{
	private const int ItemsToSerialize = 100000;

	internal void Awake()
	{
		UnityEngine.Debug.Log(SystemInfo.processorType);
	}
	internal void OnGUI()
	{
		var width = 500.0f;
		var height = Screen.height - 40.0f;

		GUILayout.BeginArea(new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height));
		GUILayout.BeginVertical();

		if (GUILayout.Button("Measure Json Serializer (GameDevWare)"))
			new Action(this.MeasureJsonSerializer).BeginInvoke(null, null);
		if (GUILayout.Button("Measure MessagePack Serializer (GameDevWare)"))
			new Action(this.MeasureMsgPackSerializer).BeginInvoke(null, null);
		if (GUILayout.Button("Test MessagePack"))
			new Action(Assets.Scripts.Example.SerializeExample.SerializeToMsgPackMemoryStream).BeginInvoke(null, null);
			if (GUILayout.Button("Test Json"))
			new Action(Assets.Scripts.Example.SerializeExample.SerializeToJsonMemoryStream).BeginInvoke(null, null);

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	private void MeasureJsonSerializer()
	{
		try
		{
			this.SerializeTestObject<BigTestObject>(Json.Serialize<IEnumerable<BigTestObject>>);
			this.SerializeTestObject<MediumTestObject>(Json.Serialize<IEnumerable<MediumTestObject>>);
			this.SerializeTestObject<SmallTestObject>(Json.Serialize<IEnumerable<SmallTestObject>>);
			this.DeserializeTestObject<BigTestObject>(Json.Serialize<IEnumerable<BigTestObject>>, Json.Deserialize<List<BigTestObject>>);
			this.DeserializeTestObject<MediumTestObject>(Json.Serialize<IEnumerable<MediumTestObject>>, Json.Deserialize<List<MediumTestObject>>);
			this.DeserializeTestObject<SmallTestObject>(Json.Serialize<IEnumerable<SmallTestObject>>, Json.Deserialize<List<SmallTestObject>>);
		}
		catch (Exception e)
		{
			UnityEngine.Debug.LogError(e);
		}
	}
	private void MeasureMsgPackSerializer()
	{
		try
		{
			this.SerializeTestObject<BigTestObject>(MsgPack.Serialize<IEnumerable<BigTestObject>>);
			this.SerializeTestObject<MediumTestObject>(MsgPack.Serialize<IEnumerable<MediumTestObject>>);
			this.SerializeTestObject<SmallTestObject>(MsgPack.Serialize<IEnumerable<SmallTestObject>>);
			this.DeserializeTestObject<BigTestObject>(MsgPack.Serialize<IEnumerable<BigTestObject>>, MsgPack.Deserialize<List<BigTestObject>>);
			this.DeserializeTestObject<MediumTestObject>(MsgPack.Serialize<IEnumerable<MediumTestObject>>, MsgPack.Deserialize<List<MediumTestObject>>);
			this.DeserializeTestObject<SmallTestObject>(MsgPack.Serialize<IEnumerable<SmallTestObject>>, MsgPack.Deserialize<List<SmallTestObject>>);
		}
		catch (Exception e)
		{
			UnityEngine.Debug.LogError(e);
		}
	}

	private void SerializeTestObject<T>(Action<IEnumerable<T>, Stream> serializeFn) where T : ITestObject, new()
	{
		var output = new ByteCountingStream();
		var value = new T();
		value.Fill();
		var sw = Stopwatch.StartNew();

		// warmup
		//UnityEngine.Debug.Log(string.Format("[{0}] Warming-up {1} Serializer", typeof(T).Name, serializeFn.Method.DeclaringType.Name));
		serializeFn(InfiniteEnumerable(value).Take(ItemsToSerialize / 100), output);

		//UnityEngine.Debug.Log(string.Format("[{0}] Running {1} Serializer", typeof(T).Name, serializeFn.Method.DeclaringType.Name));
		// reset
		GC.Collect();
		output.SetLength(0);
		sw.Start();
		serializeFn(InfiniteEnumerable(value).Take(ItemsToSerialize), output);
		sw.Stop();
		//UnityEngine.Debug.Log(string.Format("[{0}] {1} Serializer finished in {2:F2}ms, {3} bytes are written.", typeof(T).Name, serializeFn.Method.DeclaringType.Name, sw.ElapsedMilliseconds, output.Length));
		UnityEngine.Debug.Log(string.Format("[{0}] {1} | size(bytes) {2} | object/s {3:F0} | bandwidth {4:F2} Mb/s", typeof(T).Name, serializeFn.Method.DeclaringType.Name,
			output.Length / ItemsToSerialize,
			ItemsToSerialize * (1 / sw.Elapsed.TotalSeconds),
			output.Length * (1 / sw.Elapsed.TotalSeconds) / 1024 / 1024));
	}
	private void DeserializeTestObject<T>(Action<IEnumerable<T>, Stream> serializeFn, Func<Stream, List<T>> deserializeFn) where T : ITestObject, new()
	{
		var output = new MemoryStream();
		var value = new T();
		value.Fill();
		var sw = Stopwatch.StartNew();

		// warmup
		//UnityEngine.Debug.Log(string.Format("[{0}] Warming-up {1} deserializer", typeof(T).Name, serializeFn.Method.DeclaringType.Name));
		serializeFn(InfiniteEnumerable(value).Take(ItemsToSerialize / 100), output);
		output.Position = 0;
		deserializeFn(output);

		//UnityEngine.Debug.Log(string.Format("[{0}] Running {1} deserializer", typeof(T).Name, serializeFn.Method.DeclaringType.Name));
		// reset
		GC.Collect();
		sw.Start();
		output.Position = 0;
		deserializeFn(output);
		sw.Stop();
		//UnityEngine.Debug.Log(string.Format("[{0}] {1} Deserializer finished in {2:F2}ms, {3} bytes are readed.", typeof(T).Name, serializeFn.Method.DeclaringType.Name, sw.ElapsedMilliseconds, output.Length));
		UnityEngine.Debug.Log(string.Format("[{0}] {1} | size(bytes) {2} | object/s {3:F0} | bandwidth {4:F2} Mb/s", typeof(T).Name, serializeFn.Method.DeclaringType.Name,
			output.Length / (ItemsToSerialize / 100),
			ItemsToSerialize / 100.0 * (1 / sw.Elapsed.TotalSeconds),
			output.Length * (1 / sw.Elapsed.TotalSeconds) / 1024 / 1024));
	}

	public IEnumerable<T> InfiniteEnumerable<T>(T value)
	{
		while (true)
			yield return value;
	}

	/*
	Intel(R) Core(TM) i5-3570 CPU @ 3.40GHz

	Serialization:
			| size(bytes)   | object/sec.	| bandwidth
	Json	| 744			| 7658			| 5.43 Mb/s
	Json	| 154			| 30853			| 4.53 Mb/s
	Json	| 102			| 64888			| 6.31 Mb/s
	MsgPack | 666			| 15206			| 9.66 Mb/s
	MsgPack | 146			| 123475		| 17.19 Mb/s
	MsgPack | 92			| 155038		| 13.60 Mb/s

	De-serialization:
	Json	| 744			| 268			| 0.19 Mb/s
	Json	| 154			| 7218			| 1.06 Mb/s
	Json	| 102			| 9981			| 0.97 Mb/s
	MsgPack | 666			| 474			| 0.30 Mb/s
	MsgPack | 146			| 15153			| 2.11 Mb/s
	MsgPack | 92			| 15278			| 1.34 Mb/s

	*/
}

