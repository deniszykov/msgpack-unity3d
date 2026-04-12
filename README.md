# JSON & MessagePack Serialization for Unity

[![Build Status](https://travis-ci.org/deniszykov/msgpack-unity3d.svg?branch=master)](https://travis-ci.org/deniszykov/msgpack-unity3d)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](License.md)

High-performance, allocation-free (where possible), and AOT-compatible serialization library for Unity. Support for both **MessagePack** (binary) and **JSON** (text) formats with a unified API.

Designed specifically for Unity's unique constraints, including IL2CPP (AOT) and limited system library access on platforms like iOS, WebGL, and consoles.

---

## Key Features

- **Standard Compliance**: Full implementation of [MessagePack](https://msgpack.org/) and [JSON](https://json.org/) specifications.
- **AOT / IL2CPP Ready**: Works out-of-the-box on iOS, WebGL, and other AOT platforms without dynamic code generation.
- **Unity Built-in Types**: Native support for `Vector2/3/4`, `Quaternion`, `Color`, `Bounds`, `Rect`, and `Matrix4x4`.
- **Flexible Mapping**: Use standard `[DataContract]` and `[DataMember]` attributes or serialize plain classes/structs.
- **Polymorphism Support**: Can preserve and restore type information for interface or base class fields.
- **Customizable**: Easy to implement and register custom `TypeSerializer` for any type.
- **Lightweight**: Minimal dependencies and small binary size.

---

## Installation

### Unity Package Manager (UPM)
You can add this package to your project via the Package Manager window using the Git URL:
`https://github.com/deniszykov/msgpack-unity3d.git?path=/src/GameDevWare.Serialization.Unity/Packages/com.gamedevware.serialization`

### Unity Asset Store
Available as a free asset: [Json + MessagePack Serializer](https://assetstore.unity.com/packages/tools/network/json-messagepack-serialization-59918)

### NuGet (for .NET projects)
```bash
PM> Install-Package GameDevWare.Serialization
```

---

## Quick Start

### Basic Serialization

```csharp
using GameDevWare.Serialization;
using System.IO;

// Create your data
var myData = new MyData { Name = "Unity", Version = 2.0f };

// Serialize to MessagePack
var stream = new MemoryStream();
MsgPack.Serialize(myData, stream);

// Deserialize from MessagePack
stream.Position = 0;
var restoredData = MsgPack.Deserialize<MyData>(stream);
```

### Working with JSON

```csharp
// Serialize to JSON string
string json = Json.SerializeToString(myData);

// Deserialize from JSON string
var restoredData = Json.Deserialize<MyData>(json);
```

---

## Advanced Usage

### Data Contract Attributes
Use attributes to control how your fields are serialized:

```csharp
[DataContract]
public class User
{
    [DataMember(Name = "id")]
    public int Id;

    [DataMember(Name = "full_name")]
    public string Name;

    [IgnoreDataMember]
    public string SessionToken; // This won't be serialized
}
```

### Preserving Type Information (Polymorphism)
By default, the serializer handles concrete types. If you need to serialize interfaces or abstract classes, enable type information:

```csharp
// Serialize with type information
MsgPack.Serialize(hero, stream, SerializationOptions.None); 

// Deserialize (the correct concrete type will be instantiated automatically)
var hero = (BaseHero)MsgPack.Deserialize(typeof(BaseHero), stream, SerializationOptions.None);
```
*Note: Use `SerializationOptions.SuppressTypeInformation` to reduce payload size when you only deal with concrete types.*

### Custom Type Serializers
Implement `TypeSerializer` to handle complex types manually:

```csharp
public sealed class MyCustomSerializer : TypeSerializer {
    public override Type SerializedType { get { return typeof(MyCustomType); } }
    
    public override object Deserialize(IJsonReader reader) { 
        var value = reader.ReadString(nextToken: false);
        return new MyCustomType(value);
    }

    public override void Serialize(IJsonWriter writer, object valueObj) {
        var value = (MyCustomType)valueObj;
        writer.Write(value.ToString());
    }
}
```
Register it via `Json.DefaultSerializers.Add(new MyCustomSerializer())` or use `[TypeSerializer(typeof(MyCustomSerializer))]` attribute on your class.

---

## Platform Support & AOT (IL2CPP)

This library is fully compatible with **IL2CPP**. 

**Important for IL2CPP/AOT:** 
On platforms that use IL2CPP (iOS, WebGL, Android with IL2CPP), Unity might strip out necessary metadata for `System.Runtime.Serialization.dll` and `GameDevWare.Serialization.dll`. To prevent this, add a `link.xml` file to your project's root:

```xml
<linker>
  <assembly fullname="System.Runtime.Serialization" preserve="all"/>
  <assembly fullname="GameDevWare.Serialization" preserve="all"/>
</linker>
```

---

## FAQ

**Q: Is it faster than JsonUtility?**
A: `JsonUtility` is a C++ wrapper and is generally faster for very simple classes. However, this library supports Dictionaries, Interfaces, polymorphic types, and MessagePack, which `JsonUtility` does not.

**Q: Can I use it for network packets?**
A: Yes! Its MessagePack implementation is extremely efficient for binary networking, providing a much smaller footprint than JSON.

**Q: Does it support circular references?**
A: By default, it will throw an exception to prevent infinite loops. It is designed for data-transfer objects rather than complex object graphs.

---

## Contacts
Found a bug or have a suggestion? 
- Email: support@gamedevware.com
- GitHub Issues: [Report here](https://github.com/deniszykov/msgpack-unity3d/issues)

## License
This project is licensed under the [MIT License](License.md).
