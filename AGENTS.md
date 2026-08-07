# VivLib — AI Project Guide

## Project Overview

**VivLib** is a lightweight .NET 8+ library for modding classic Need For Speed titles (NFS 2, 3, 4). It provides read-write support for EA game file formats including `.VIV`, `.FSH`/`.QFS` (textures), `.ASF`/`.MUS`/`.BNK` (audio), `.FCE` (3D car models), `.GEO`, and various car performance data formats.

- **Language:** C# (.NET 8.0 / .NET 10.0)
- **Style:** Modern C# with implicit usings and nullable reference types enabled
- **Namespace:** `TheXDS.Vivianne`
- **External deps:** `SixLabors.ImageSharp`, `BCnEncoder.Net.ImageSharp`, `TheXDS.MCART`
- **Test framework:** NUnit 4.x + Moq + coverlet + Microsoft.NET.Test.Sdk
- **CI:** GitHub Actions + Codecov
- **Doc:** [docfx](https://dotnet.github.io/docfx/) → `docs/`

## Key Commands

```bash
dotnet build                         # Build (Debug)
dotnet build -c Release              # Build release
dotnet test                          # Run all tests (Debug)
dotnet test -c Release               # Run tests in Release
dotnet test --filter "FullyQualifiedName~RefPack"  # Run a subset
```

The solution file is `VivLib.slnx` and contains two projects:
- `src/VivLib/VivLib.csproj` — the library
- `src/VivLib.Tests/VivLib.Tests.csproj` — the test project

Build outputs go to `Build/bin/<ProjectName>/` and `Build/obj/<ProjectName>/` (controlled by `BuildTargets/BuildPaths.props`).

## Project Structure

```
VivLib/
├── src/
│   ├── Directory.Build.props       # Common MSBuild imports (BuildTargets/*.props)
│   ├── Directory.Build.targets     # (if present)
│   ├── VivLib/                     # Main library
│   │   ├── VivLib.csproj           # net8.0, ImplicitUsings, Nullable
│   │   ├── AssemblyInfo.cs         # InternalsVisibleTo("VivLib.Tests")
│   │   ├── Attributes/             # Custom attributes (e.g. OffsetTableIndexAttribute)
│   │   ├── Codecs/                 # Compression codecs (RefPack, Huffman)
│   │   │   ├── Audio/              # Audio-specific codecs
│   │   │   └── Textures/           # Texture-specific codecs
│   │   ├── Extensions/             # Extension methods for arrays, FshBlob, etc.
│   │   ├── Helpers/                # Utility helpers (ColorConversion, CommonHelpers)
│   │   ├── Info/                   # File format info extractors
│   │   │   ├── Audio/              # BNK, MUS info extractors
│   │   │   ├── Fce/                # FCE model info extractor
│   │   │   └── Map/                # Map/LIN info extractors
│   │   ├── Models/                 # Data models (POCOs)
│   │   │   ├── Audio/              # Audio file models
│   │   │   ├── Base/               # Shared base models
│   │   │   ├── Carp/               # Car performance data
│   │   │   ├── Fce/                # FCE model models
│   │   │   ├── Fe/                 # Front-end models
│   │   │   ├── Fsh/                # Texture blob models
│   │   │   ├── Geo/                # GEO model models
│   │   │   ├── Shared/             # Cross-format shared models
│   │   │   ├── Tga/                # TGA texture models
│   │   │   └── Viv/                # VIV archive models
│   │   ├── Resources/              # Localized strings (.resx) & mappings
│   │   ├── Serializers/            # Read/write for each file format
│   │   │   ├── Audio/              # ASF, MUS, BNK serializers
│   │   │   ├── Carp/               # Car perf data serializers
│   │   │   ├── Fce/                # FCE 3D model serializers
│   │   │   ├── Fe/                 # Front-end serializers
│   │   │   ├── Fsh/                # FSH/QFS texture serializers
│   │   │   ├── Geo/                # GEO serializers
│   │   │   ├── Misc/               # LIN, MAP, DAT serializers
│   │   │   └── Viv/                # VIV archive serializers
│   │   └── Tools/                  # High-level utilities
│   │       ├── Audio/              # Audio tools
│   │       ├── Base/               # Base tool interfaces (IConversionTool, etc.)
│   │       ├── Carp/               # Car perf analysis
│   │       ├── Fce/                # FCE model tools + cleanup analyzer
│   │       ├── Fe/                 # Front-end data text providers
│   │       └── Misc/               # Misc utilities
│   │
│   └── VivLib.Tests/               # Unit tests
│       ├── VivLib.Tests.csproj     # NUnit 4, Moq, coverlet
│       ├── GlobalUsings.cs         # global using NUnit.Framework
│       ├── Resources/Files/        # Embedded test fixtures (Nfs2/, Nfs3/, Nfs4/)
│       ├── Codecs/                 # Codec tests
│       ├── Extensions/             # Extension method tests
│       ├── Helpers/                # Helper tests
│       ├── Info/                   # Info extractor tests
│       ├── Models/                 # Model tests
│       ├── Serializers/            # Serializer tests
│       └── Tools/                  # Tool tests
│
├── BuildTargets/                   # Shared MSBuild props/targets
│   ├── BuildPaths.props            # Output paths (Build/bin/, Build/obj/)
│   ├── CompileOptions.targets      # Nullable, deterministic, SourceLink, etc.
│   ├── GlobalDirectives.props      # ExtraDefineConstants
│   ├── PackageInfo.props           # Package metadata
│   └── PackageVersion.props        # Package versions
├── docs/                           # docfx documentation
├── Art/                            # Project artwork
└── Build/                          # Build output (gitignored)
    ├── bin/
    └── obj/
```

## Coding Conventions

### Language & Compiler
- **C# 12+** features are fair game (collection expressions, primary constructors, pattern matching, etc.)
- `nullable` reference types are **enabled** — use `?`/`!` appropriately
- `implicitUsings` are **enabled** — no need for global using directives in user files
- Files use **top-level namespace declarations** (no `namespace` keyword indentation)
- **Zero compiler warnings** — no new warnings on compile

### Naming & Style
- **Public APIs:** XML documentation comments (`///`) — **80-char max per line**, with `<summary>`, `<param>`, `<typeparamref>`, `<remarks>`
- **Interfaces:** `I` prefix (e.g., `ISerializer<T>`, `ICarPerf`, `IInSerializer<T>`)
- **Serializers:** Named `<Format>Serializer` (e.g., `FshSerializer`, `VivSerializer`, `FceSerializer`)
- **Models:** PascalCase class names matching the domain (e.g., `VivHeader`, `FshBlob`, `Carp`)
- **Tools:** Named `<Feature>Tool` or `<Feature>Provider` (e.g., `FceCleanupTool`, `FeDataTextProvider`)
- **Private fields:** `_camelCase` or `readonly` backing fields

### Code Structure Rules
- **Max 3 levels of indentation.** If deeper — rethink the logic (segregate into small, manageable methods).
- Keep functions small, clear, and testable.
- User-facing strings go in `.resx` files — **no magic strings** (file-format magic numbers are fine).
- In-code comments should be minimal — code should be self-explanatory. Add comments only to explain *why* something looks odd or overly complex (see `GeoSerializer_privates.cs`, `MapSerializer.cs`, `BnkSerializer_Privates.cs` for examples).

### Serializer Pattern (key architecture)
All serializers implement the `ISerializer<T>` interface hierarchy:
```
ISerializer<T>
  ├── IInSerializer<T>   → T Deserialize(Stream) / T Deserialize(byte[])
  └── IOutSerializer<T>  → void Serialize(Stream) / byte[] Serialize()
```
Each file format has its own serializer class in the corresponding `Serializers/<Format>/` folder. The serializer reads/writes the format's binary structure and populates the `Models/<Format>/` POCO types.

### Tool Pattern
Tools live in `Tools/<Feature>/` and build on serializers + models:
- `Tools/Base/` defines shared interfaces: `IConversionTool`, `IInPlaceTransformTool<T>`
- `Tools/<Feature>/` contains concrete implementations
- Tools often consume serializers to read data, manipulate it via models, and write back

### Resources / Localization
- String resources are `.resx` files under `Resources/Strings/`
- Auto-generated `*.Designer.cs` files for strongly-typed access
- Embedded resources in the project file

## Testing Patterns

### Test Framework & Structure
- **Framework:** NUnit 4.x (`[Test]`, `[TestCaseSource]`, `[SetUp]`, etc.)
- **Assertions:** `Assert.That(...)`, `Throws.InstanceOf<T>()`
- **Mocks:** Moq for interface-based dependencies
- **Test fixture data:** Embedded `.resx` resources for test files in `Resources/Files/`

### Example Test Patterns

**Unit test for a pure function:**
```csharp
namespace TheXDS.Vivianne.Codecs;

internal class RefPackCodecTests
{
    [Test]
    public void IsCompressed_returns_false_if_RefPack_signature_is_missing()
    {
        Assert.That(RefPackCodec.IsCompressed([0, 1, 2, 3, 4]), Is.False);
    }

    [Test]
    public void IsCompressed_returns_true_if_RefPack_signature_is_present()
    {
        Assert.That(RefPackCodec.IsCompressed([0x10, 0xFB]), Is.True);
    }
}
```

**Round-trip / integration test with embedded fixture:**
```csharp
[TestCaseSource(nameof(GetTestCases))]
public void Codec_roundtrip_test(byte[] testFileContents)
{
    var compressed = RefPackCodec.Compress(testFileContents);
    var roundtrip = RefPackCodec.Decompress(compressed);
    Assert.That(roundtrip.SequenceEqual(testFileContents));
}

private static IEnumerable<byte[]> GetTestCases()
{
    yield return GetTestFsh();                              // Embedded resource
    yield return GetDeterministicRndArray(65536);           // Synthetic data
    yield return [.. Enumerable.Range(0, 65536).Select(i => (byte)i)];
}
```

**Testing with Moq:**
```csharp
[Test]
public void Tool_process_calls_serializer_deserialize()
{
    var mockSerializer = new Mock<ISerializer<MyModel>>();
    mockSerializer.Setup(s => s.Deserialize(It.IsAny<byte[]>()))
                  .Returns(new MyModel { ... });

    var tool = new MyTool(mockSerializer.Object);
    // ... act and assert
}
```

### Test File Organization
- Tests mirror the source structure: `Serializers/FshSerializerTests.cs` tests `Serializers/FshSerializer.cs`
- Test classes are `internal` by default (exposed via `InternalsVisibleTo`)
- Embedded test files go in `Resources/Files/<NfsVersion>/` and are included as `<EmbeddedResource>`

## Architecture Notes

### SOLID Philosophy
VivLib uses SOLID **responsibly**, not religiously. Key points:

- **No excessive interface segregation** for single implementations. If there's only one correct way to serialize an FCE model, a single serializer class is fine — no need for separate read/write interfaces just to satisfy ISP.
- **No unnecessary DI** for simple, testable classes. A well-coupled class that can be integration-tested is often better than layers of abstractions.
- SRP/ISP are good — but when types are closely related, group them together. Codecs and serializers are a good example: one class for both reading and writing is appropriate.

### Boy-Scout Rule
Small improvements to clarity are welcome, but **do not rewrite the entire codebase**. Incremental, focused changes are the way to go.

## Git & Contributing

- **Commit messages:** Use [Conventional Commits](https://www.conventionalcommits.org/) (e.g., `feat:`, `fix:`, `docs:`, `test:`).
- **Branch naming:** `feature/42-add-bnk-write-support`, `fix/15-refpack-decode-error`
- **Full contributing guide:** [docs/CONTRIBUTING.md](../../CONTRIBUTING.md)
- **Issue tracker & PRs:** [github.com/TheXDS/VivLib](https://github.com/TheXDS/VivLib)
 - **FAQ:** `docs/FAQ.md` — answers to common project questions

## Important Details

1. **InternalsVisibleTo:** `VivLib.Tests` has access to internal types via `AssemblyInfo.cs`
2. **Embedded test resources:** Test fixtures are embedded as resources, loaded via `GetManifestResourceStream`
3. **Nullable reference types:** Always pay attention to `null`/`NotNull` annotations in public APIs
4. **Async serializers:** `ISerializer<T>` provides async wrappers (`DeserializeAsync`, `SerializeAsync`) around the sync methods
5. **Version-aware code:** `NfsVersion` enum and `VersionIdentifier` class handle format differences across NFS 2/3/4
6. **Build props hierarchy:** `Directory.Build.props` → `BuildTargets/*.props` → per-project `*.csproj`
