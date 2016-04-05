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
		UnityEngine.Debug.Log(string.Format("[{0}] Warming-up {1} Serializer", typeof(T).Name, serializeFn.Method.DeclaringType.Name));
		serializeFn(InfiniteEnumerable(value).Take(ItemsToSerialize / 100), output);

		UnityEngine.Debug.Log(string.Format("[{0}] Running {1} Serializer", typeof(T).Name, serializeFn.Method.DeclaringType.Name));
		// reset
		GC.Collect();
		output.SetLength(0);
		sw.Start();
		serializeFn(InfiniteEnumerable(value).Take(ItemsToSerialize), output);
		sw.Stop();
		UnityEngine.Debug.Log(string.Format("[{0}] {1} Serializer finished in {2:F2}ms, {3} bytes are written.", typeof(T).Name, serializeFn.Method.DeclaringType.Name, sw.ElapsedMilliseconds, output.Length));
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
		UnityEngine.Debug.Log(string.Format("[{0}] Warming-up {1} deserializer", typeof(T).Name, serializeFn.Method.DeclaringType.Name));
		serializeFn(InfiniteEnumerable(value).Take(ItemsToSerialize / 100), output);
		output.Position = 0;
		deserializeFn(output);

		UnityEngine.Debug.Log(string.Format("[{0}] Running {1} deserializer", typeof(T).Name, serializeFn.Method.DeclaringType.Name));
		// reset
		GC.Collect();
		sw.Start();
		output.Position = 0;
		deserializeFn(output);
		sw.Stop();
		UnityEngine.Debug.Log(string.Format("[{0}] {1} Deserializer finished in {2:F2}ms, {3} bytes are readed.", typeof(T).Name, serializeFn.Method.DeclaringType.Name, sw.ElapsedMilliseconds, output.Length));
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

	| size(bytes) 645 | object/s 7766  | bandwidth 5.00  Mb/s
	| size(bytes) 85  | object/s 34916 | bandwidth 2.96  Mb/s
	| size(bytes) 34  | object/s 72254 | bandwidth 2.45  Mb/s

	| size(bytes) 645 | object/s 570   | bandwidth 0.36  Mb/s
	| size(bytes) 85  | object/s 23923 | bandwidth 2.03  Mb/s
	| size(bytes) 34  | object/s 29334 | bandwidth 1.00  Mb/s

	| size(bytes) 570 | object/s 14974  | bandwidth 8.53  Mb/s
	| size(bytes) 79  | object/s 134048 | bandwidth 10.58 Mb/s
	| size(bytes) 26  | object/s 191938 | bandwidth 5.00  Mb/s

	| size(bytes) 570 | object/s 985   | bandwidth 0.56  Mb/s
	| size(bytes) 79  | object/s 35663 | bandwidth 2.81 Mb/s
	| size(bytes) 26  | object/s 37792 | bandwidth 1.00  Mb/s

	*/
}

