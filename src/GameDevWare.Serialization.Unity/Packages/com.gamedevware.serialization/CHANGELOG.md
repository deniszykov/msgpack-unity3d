# Changelog

All notable changes to this project will be documented in this file.

## [3.0.0] - 2026-04-12

### Breaking Changes
- **Unity Package Manager support**: Moved all code from a drop-in asset structure to a standard Unity Package format (`com.gamedevware.serialization`). This requires updating project references and relocation of files if you were using the old `Assets/Plugins` structure.
- **DateTime Serialization**: `DateTime` values with `DateTimeKind.Unspecified` are no longer automatically converted to `Utc` during serialization. This ensures that the original kind is preserved during roundtrips, especially for ISO 8601 strings.

### Added
- **Detailed Error Messages**: Rewrote all `JsonSerializationException` messages to provide integrated "Possible causes" and troubleshooting advice for common issues like missing constructors or invalid stream access.
- **Nesting Depth Checks**: Added a default nesting depth limit (64) to prevent infinite loops and `StackOverflowException` in deeply nested hierarchies or circular references.
- **Serialization Path Tracking**: Added `Path` property to `SerializationContext` to aid in debugging by identifying the exact location of an error in the object graph.
- **XML Documentation**: Added missing XML documentation to public members for better IntelliSense support.
- **Improved Test Suite**: Refactored unit tests to include edge cases for maximum/minimum values of primitive types and expanded coverage for `DefaultMessagePackExtensionTypeHandler`.

### Fixed
- **DateTime Roundtrips**: Fixed issues where `DateTime` and `DateTimeOffset` were not correctly parsed back in some edge cases.
- **Empty Stream Handling**: Fixed a `StackOverflowException` in `JsonReader` when attempting to read an empty stream.
- **Trailing Escapes**: Fixed a bug where `JsonReader` failed to correctly handle multiple backslash escape characters at the end of a string literal.
- **AOT Compatibility**: Improved the discovery of default constructors in AOT/IL2CPP environments.
- **Stream Direction Check**: Fixed a bug in `MsgPackWriter` where it would incorrectly check for `CanRead` instead of `CanWrite` during initialization.

### Changed
- **Documentation**: Updated `README.md` with modern installation instructions for Unity Package Manager and clearer AOT configuration guides.
