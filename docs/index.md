# VivLib Documentation

Welcome to **VivLib** — a lightweight .NET library for reading and writing classic Need For Speed game file formats.

## What is VivLib?

VivLib is a modern, well-designed C# library that provides comprehensive support for modding classic Need For Speed titles (NFS 2, 3, and 4). Originally part of the [Vivianne](https://github.com/TheXDS/Vivianne) project, it has evolved into a focused library that prioritizes clarity, correctness, and ease of use for enthusiasts and modders.

### Key Characteristics
- **Managed Language:** Written in C# with .NET 8.0+ for type and memory safety
- **Read-Write Focus:** Full serialization support for the game files you want to modify
- **Modular Design:** Clean separation of codecs, serializers, models, and tools
- **Well-Documented:** Clear, minimal code with comprehensive XML documentation
- **Thoroughly Tested:** NUnit 4.x test suite with embedded test fixtures and high code coverage

## Getting Started

### Supported Formats

VivLib supports read and write operations for a wide range of classic NFS file formats:

| Format | Read | Write | Notes |
|--------|:----:|:-----:|-------|
| `.VIV` | ✔️ | ✔️ | Archive format |
| `.FSH`, `.QFS` | ✔️ | ✔️ | Textures with RefPack compression |
| `.FCE` | ✔️ | ✔️ | 3D car models with multi-version support |
| `.BNK` | ✔️ | ✔️ | Audio bank files with PCM/EA-ADPCM |
| `.ASF`, `.MUS` | ✔️ | ✔️ | Audio tracks (experimental write) |
| `.LIN`, `.MAP` | ✔️ | ❌ | Map/track data |
| Car Performance Data | ✔️ | ✔️ | NFS2 `.DAT`, NFS3/4 `.TXT` |
| Front-End Data | ✔️ | ✔️ | NFS3/4 car metadata (`.BRI`, `.ENG`, etc.) |

### Installation

Add VivLib to your project via NuGet:

```bash
dotnet add package TheXDS.Vivianne.VivLib
```

### Quick Example

```csharp
using TheXDS.Vivianne;
using TheXDS.Vivianne.Serializers.Viv;

// Read a VIV archive
var vivSerializer = new VivSerializer();
var vivFile = vivSerializer.Deserialize(File.ReadAllBytes("archive.viv"));

// Access entries
foreach (var entry in vivFile.Entries)
{
    Console.WriteLine($"File: {entry.Name}");
}

// Modify and save
vivFile.Entries.Add(new VivDirectoryEntry { Name = "new_file.fsh", /* ... */ });
var modifiedData = vivSerializer.Serialize(vivFile);
File.WriteAllBytes("modified.viv", modifiedData);
```

## Documentation

Browse the documentation to learn more:

- **[API Documentation](api/)** — Complete .NET API reference with namespace organization and type hierarchy
- **[Contributing](CONTRIBUTING.md)** — How to contribute to VivLib, including development setup, coding guidelines, and commit conventions
- **[FAQ](FAQ.md)** — Frequently asked questions about VivLib, its scope, and classic NFS modding
- **[Code of Conduct](CODE_OF_CONDUCT.md)** — Community standards and expectations

## Project Philosophy

VivLib is driven by curiosity and a passion for preserving and understanding classic games. It's designed as a focused, well-documented library rather than an all-encompassing framework. Key principles include:

- **Clarity over cleverness** — Code should be easy to understand and maintain
- **Correctness first** — Accurate implementations based on careful reverse engineering
- **Minimalism** — Only include what's necessary; avoid bloat and unnecessary abstractions
- **SOLID principles applied wisely** — Use design patterns when they genuinely improve the code, not for their own sake

## Architecture Overview

VivLib follows a layered architecture:

```
┌─────────────────────────────────────┐
│          Application Code           │
└────────┬────────────────────────────┘
         │
┌────────▼────────────────────────────┐
│  Tools / High-Level Utilities       │
│  (FceCleanupTool, FeDataProvider)   │
└────────┬────────────────────────────┘
         │
┌────────▼────────────────────────────┐
│  Serializers & Models               │
│  (Read/Write format-specific data)  │
└────────┬────────────────────────────┘
         │
┌────────▼────────────────────────────┐
│  Codecs & Helpers                   │
│  (Compression, color conversion)    │
└─────────────────────────────────────┘
```

## Community

- **GitHub:** [TheXDS/VivLib](https://github.com/TheXDS/VivLib)
- **Issue Tracker:** Report bugs and request features [here](https://github.com/TheXDS/VivLib/issues)
- **Discussions:** Ask questions and share ideas in [GitHub Discussions](https://github.com/TheXDS/VivLib/discussions)

## License

VivLib is licensed under the **MIT License**. See the [LICENSE](../LICENSE) file for details.

---

**Ready to dive in?** Start by exploring the [API Documentation](api/) or read the [Contributing Guide](CONTRIBUTING.md) if you'd like to get involved with the project.