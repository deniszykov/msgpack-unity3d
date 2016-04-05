# Introduction

This [package](https://www.assetstore.unity3d.com/#!/content/59918) provides API for data serialization/de-serialization into [MessagePack](https://en.wikipedia.org/wiki/MessagePack) and [JSON](https://ru.wikipedia.org/wiki/JSON) format. 

Supported Platforms:
* PC/Mac
* IOS
* Android
* WebGL

**API**
* Json
	* Serialize
	* SerializeToString
	* Deserialize
* MsgPack
	* Serialize
	* Deserialize

## Example
Serialize object into [Stream](https://msdn.microsoft.com/en-us/library/system.io.stream%28v=vs.90%29.aspx) using MessagePack serializer:
```csharp
var outputStream = new MemoryStream();
MsgPack.Serialize(new { field1 = 1, field2 = 2 }, outputStream);
```
Deserialize object from [Stream](https://msdn.microsoft.com/en-us/library/system.io.stream%28v=vs.90%29.aspx) using MessagePack serializer:
```csharp
Stream inputStream;
MsgPack.Deserialize(typeof(MyObject), inputStream); -> instance of MyObject
// or
MsgPack.Deserialize<MyObject>(inputStream); -> instance of MyObject
```

## Mapping Types

MessagePack/Json serializer is guided by [Data Contract](https://msdn.microsoft.com/en-us/library/ms733127%28v=vs.110%29.aspx) rules. 
Its behaviour can be changed with [DataContract](https://msdn.microsoft.com/en-us/library/system.runtime.serialization.datacontractattribute%28v=vs.110%29.aspx), [DataMember](https://msdn.microsoft.com/en-us/library/system.runtime.serialization.datamemberattribute%28v=vs.110%29.aspx), [IgnoreDataMember](https://msdn.microsoft.com/en-us/library/system.runtime.serialization.ignoredatamemberattribute%28v=vs.110%29.aspx) attributes. 

Attributes could be from *System.Runtime.Serialization.dll* or your attributes with same names. 

**Only public fields/properties could be serialized.**

## Supported Types

* Primitives: Boolean, Byte, Double, Int16, Int32, Int64, SBytes, Single, UInt16, UInt32, UInt64, String
* Standard Types: Decimal, DateTimeOffset, DateTime, TimeSpan, Guid, Uri, Version, DictionaryEntry
* Unity3D Types: Bounds, Vector, Matrix4x4, Quaternion, Rect, Color ...
* Binary: Stream, byte[]
* Lists: Array, ArrayList, List<T>, HashSet<T> and any other [IEnumerable](https://msdn.microsoft.com/en-us/library/system.collections.ienumerable%28v=vs.110%29.aspx) types with **Add** method.
* Maps: Hashtable, Dictionary<K,V>, and other [IDictionary](https://msdn.microsoft.com/en-us/library/system.collections.idictionary%28v=vs.110%29.aspx) types
* [Nullable](https://msdn.microsoft.com/en-us/library/b3h38hb0%28v=vs.110%29.aspx) types
* Enumeration
* Objects

## Custom Type Serializers
To implement a custom [TypeSerializer](https://github.com/deniszykov/msgpack-unity3d/blob/master/Assets/Plugins/GameDevWare.Serialization/TypeSerializer.cs), you need to inherit from *TypeSerializer* and override the Deserialize and Serialize methods. 
```csharp
public sealed class GuidSerializer : TypeSerializer
{
	public override Type SerializedType { get { return typeof(Guid); } }

	public override object Deserialize(IJsonReader reader)
	{
		var guidStr = reader.ReadString(advance: false); // advance mean 'call reader.NextToken' after ReadString
		var value = new Guid(guidStr);
		return value;
	}

	public override void Serialize(IJsonWriter writer, object valueObj)
	{
		var value = (Guid)valueObj; // you never get null here
		var guidStr = value.ToString();
		writer.Write(guidStr);
	}
}
```
Then you need to register a serializer in *Json.DefaultSerializers* collection or mark serialized type with [TypeSerializerAttribute](https://github.com/deniszykov/msgpack-unity3d/blob/master/Assets/Plugins/GameDevWare.Serialization/TypeSerializerAttribute.cs).

## Extra Type Information
With each serialized object additional information about its type is stored. This can lead to an increased size of the serialized data. If you do not want to store object's type information, specify *SuppressTypeInformation* when calling **Serialize** method.
```csharp
MsgPack.Serialize(value, stream, SerializationOptions.SuppressTypeInformation);
```
If you want to ignore type information when de-serializing object, specify *SuppressTypeInformation* when calling **Deserialize** method.
```csharp
Stream inputStream;
MsgPack.Deserialize(typeof(MyObject), stream, SerializationOptions.SuppressTypeInformation);
```

## License
This license does not allow you to include any source code or binaries from this package into other packages. If want to do this, contact support@gamedevware.com.
[Asset Store Terms of Service and EULA](License.md)
