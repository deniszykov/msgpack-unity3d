# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

GameDevWare.Serialization: allocation-light, AOT/IL2CPP-compatible JSON + MessagePack serializer for Unity and
.NET. Unified `Json` / `MsgPack` static APIs over a shared reader/writer/serializer core.

## Source of truth: one Runtime folder, compiled twice

All actual implementation lives in:

```
src/GameDevWare.Serialization.Unity/Packages/com.gamedevware.serialization/Runtime/**/*.cs
```

This is the Unity UPM package's Runtime folder. The .NET class library project
(`src/GameDevWare.Serialization/GameDevWare.Serialization.csproj`) does **not** contain its own source — it
pulls the same files in via a wildcard `<Compile Include>` pointing at that Runtime folder (see the .csproj).
So there is exactly one copy of the logic; editing a file under `GameDevWare.Serialization/` directly (outside
the two polyfill attribute files that live there) has no effect on the Unity build and vice versa.

- `src/GameDevWare.Serialization/`: only `NonSerializedAttribute.cs` / `SerializableAttribute.cs` polyfills for
  old TFMs, plus the multi-target .csproj that packages the Runtime sources for NuGet.
- `src/GameDevWare.Serialization.Unity/Packages/com.gamedevware.serialization/Runtime/`: the real
  implementation, edited directly for both Unity and .NET consumers.
- `src/GameDevWare.Serialization.Unity/Assets/Scripts/`: Unity example/benchmark scenes only, not shipped.
- `src/GameDevWare.Serialization.Tests/`: xunit test project, references the .NET class library project.

Always edit under `Runtime/`, never under the top-level `GameDevWare.Serialization/` folder.

## Build & Test

```powershell
cd src
dotnet build GameDevWare.Serialization/GameDevWare.Serialization.csproj -f netstandard2.0
```

The class library multi-targets `net35;net45;netstandard2.0;netcoreapp2.0`; `net35`/`net45` require the .NET
Framework reference assemblies and `netcoreapp2.0` an old shared runtime, so `netstandard2.0` is the reliable
local target. `net45`/`netcoreapp2.0` matter for CI/packaging, not local iteration.

```powershell
cd src/GameDevWare.Serialization.Tests
dotnet test -f net10.0
```

Tests target `net45;net10.0` and use **xunit** (not NUnit — `AGENT.md`/`README` references to NUnit 2.6.4 are
stale). Run a single test:

```powershell
dotnet test -f net10.0 --filter "FullyQualifiedName~JsonWriteReadTests.SomeTestName"
```

`.travis.yml` targets `netcoreapp2.0` and is stale/unused; there is no active CI workflow in this repo.

### Unity
The package under `src/GameDevWare.Serialization.Unity/Packages/com.gamedevware.serialization/` is consumed
directly via UPM (git URL with `?path=`) — there's no separate Unity build step for this repo. IL2CPP/AOT
consumers need a `link.xml` preserving `System.Runtime.Serialization` and `GameDevWare.Serialization` (see
README) — none is bundled in this repo since it's the responsibility of the *consuming* Unity project.

## Architecture

Core namespace `GameDevWare.Serialization`, all under `Runtime/`:

- **Entry points**: `Json.cs` and `MsgPack.cs` — static facades exposing `Serialize`/`Deserialize` overloads.
  `MsgPack` mostly proxies shared state (`DefaultFormat`, `DefaultSerializers`, etc.) onto `Json`'s statics;
  both formats share one serializer/metadata pipeline and differ only at the reader/writer layer.
- **Reader/Writer abstraction**: `IJsonReader` / `IJsonWriter` are the format-agnostic interfaces every
  serializer works against. JSON has multiple writer/reader implementations for different sources
  (`JsonTextReader`/`Writer`, `JsonStreamReader`/`Writer`, `JsonStringReader`/`Writer`,
  `JsonStringBuilderReader`/`Writer`). `MessagePack/MsgPackReader.cs` and `MsgPackWriter.cs` implement the same
  interfaces for the binary format — this is what lets one `TypeSerializer` implementation serve both formats.
- **`TypeSerializer`** (abstract): base class for all type-specific (de)serialization logic —
  `Deserialize(IJsonReader)` / `Serialize(IJsonWriter, object)`. `Serializers/` holds the built-ins (primitives,
  collections, `DateTime`, `Guid`, `Uri`, Unity types `Vector2/3/4`, `Quaternion`, `Color`, `Bounds`, `Rect`,
  `Matrix4x4`, etc.). `ObjectSerializer` is the generic reflection-driven fallback for `[DataContract]`/POCO
  types, including polymorphic type-tagging (see `ObjectSerializer.TYPE_MEMBER_NAME`, `"_type"` — deserializing
  that field instantiates an arbitrary type by name, so it's a deliberate attack surface guarded by
  `SerializationOptions.SuppressTypeInformation`, not something to loosen casually).
- **`SerializationContext`**: per-operation config (format provider, encoding, date formats, registered
  `Serializers` dictionary, `MaxHierarchyDepth`) plus live state during a walk — `Hierarchy` (object stack,
  compared by reference via `IdentityComparer`, not content, for circular-reference detection) and `Path`
  (`PathSegment` stack, for error messages).
- **`Metadata/`**: reflection layer that turns a CLR type into member lists (`TypeDescription`,
  `MemberDescription`/`FieldDescription`/`PropertyDescription`, `MetadataReflection`) honoring
  `[DataContract]`/`[DataMember]`/`[IgnoreDataMember]`. `ObjectSerializer` is built on top of this rather than
  raw reflection.
- **Custom serializers**: register via `Json.DefaultSerializers.Add(new MyCustomSerializer())` or the
  `[TypeSerializer(typeof(...))]` attribute on the target type — see README for the full pattern.

## Compatibility constraints

Because this ships to Unity IL2CPP/AOT targets and `net35`, avoid patterns that need JIT/reflection.Emit,
`dynamic`, or newer BCL-only APIs — check existing `Serializers/*` for the idioms already in use before adding
new dependencies or language features. Style: tabs, PascalCase, CRLF (see `.editorconfig`).
