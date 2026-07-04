# VivLib API Reference

Welcome to the VivLib API documentation. This reference documents the complete public API of VivLib, organized by functional namespace.

## Quick Navigation

### Core Components

The VivLib API is organized around several key architectural layers:

| Layer | Namespaces | Purpose |
|-------|-----------|---------|
| **Serializers** | `Serializers.*` | Read and write file formats (VIV, FSH, FCE, BNK, etc.) |
| **Models** | `Models.*` | Data structures representing parsed file contents |
| **Codecs** | `Codecs.*` | Compression and encoding algorithms (RefPack, Huffman, EA-ADPCM) |
| **Tools** | `Tools.*` | High-level utilities for common modding tasks |
| **Helpers & Info** | `Helpers.*`, `Info.*` | Utility functions and metadata extractors |
| **Extensions** | `Extensions.*` | Extension methods for arrays, models, and collections |

---

## Namespace Organization

### 📦 Serializers

Entry point for reading and writing game files.

- **[`TheXDS.Vivianne.Serializers`](TheXDS.Vivianne.Serializers.html)** — Base serializer interfaces
  - **[`Audio`](TheXDS.Vivianne.Serializers.Audio.html)** — ASF, MUS, BNK serializers
  - **[`Viv`](TheXDS.Vivianne.Serializers.Viv.html)** — VIV archive serializers
  - **[`Fsh`](TheXDS.Vivianne.Serializers.Fsh.html)** — FSH/QFS texture serializers
  - **[`Fce`](TheXDS.Vivianne.Serializers.Fce.html)** — FCE 3D model serializers (NFS3/4)
  - **[`Fe`](TheXDS.Vivianne.Serializers.Fe.html)** — Front-end/UI data serializers
  - **[`Carp`](TheXDS.Vivianne.Serializers.Carp.html)** — Car performance data serializers
  - **[`Geo`](TheXDS.Vivianne.Serializers.Geo.html)** — GEO model serializers (NFS2)
  - **[`Misc`](TheXDS.Vivianne.Serializers.Misc.html)** — LIN, MAP, DAT serializers

### 📊 Models

Data structures for representing parsed file contents.

- **[`TheXDS.Vivianne.Models`](TheXDS.Vivianne.Models.html)** — Core model types
  - **[`Audio`](TheXDS.Vivianne.Models.Audio.html)** — Audio file models (BNK, MUS, ASF)
  - **[`Viv`](TheXDS.Vivianne.Models.Viv.html)** — VIV archive structure
  - **[`Fsh`](TheXDS.Vivianne.Models.Fsh.html)** — Texture blob models, with NFS3-specific dashboard data
  - **[`Fce`](TheXDS.Vivianne.Models.Fce.html)** — 3D car model structures (NFS3/4)
  - **[`Geo`](TheXDS.Vivianne.Models.Geo.html)** — Geometric model data (NFS2)
  - **[`Fe`](TheXDS.Vivianne.Models.Fe.html)** — Front-end data models (language strings, car metadata)
  - **[`Carp`](TheXDS.Vivianne.Models.Carp.html)** — Car performance parameters (NFS2/3/4)
  - **[`Shared`](TheXDS.Vivianne.Models.Shared.html)** — Shared data structures (materials, vertices)
  - **[`Tga`](TheXDS.Vivianne.Models.Tga.html)** — TGA image format structures

### 🔧 Codecs

Compression and encoding implementations.

- **[`TheXDS.Vivianne.Codecs`](TheXDS.Vivianne.Codecs.html)** — General compression codecs
  - [`RefPackCodec`](TheXDS.Vivianne.Codecs.RefPackCodec.html) — RefPack compression (used in textures)
  - [`HuffmanCodec`](TheXDS.Vivianne.Codecs.HuffmanCodec.html) — Huffman encoding
- **[`Audio`](TheXDS.Vivianne.Codecs.Audio.html)** — Audio codecs
  - [`EaAdpcmCodec`](TheXDS.Vivianne.Codecs.Audio.EaAdpcmCodec.html) — EA-ADPCM audio compression
  - [`IAudioCodec`](TheXDS.Vivianne.Codecs.Audio.IAudioCodec.html) — Audio codec interface
- **[`Textures`](TheXDS.Vivianne.Codecs.Textures.html)** — Texture codecs
  - [`Dxt1ImageCodec`](TheXDS.Vivianne.Codecs.Textures.Dxt1ImageCodec.html), [`Dxt3ImageCodec`](TheXDS.Vivianne.Codecs.Textures.Dxt3ImageCodec.html) — DirectX texture compression
  - [`RefPackImageCodec`](TheXDS.Vivianne.Codecs.Textures.RefPackImageCodec.html) — RefPack for textures
  - [`IImageCodec`](TheXDS.Vivianne.Codecs.Textures.IImageCodec.html) — Image codec interface

### 🛠️ Tools

High-level utilities for modding operations.

- **[`TheXDS.Vivianne.Tools`](TheXDS.Vivianne.Tools.html)** — Tool interfaces and base types
- **[`Base`](TheXDS.Vivianne.Tools.Base.html)** — Base tool interfaces
  - [`IConversionTool<TIn, TOut>`](TheXDS.Vivianne.Tools.Base.IConversionTool-2.html) — Convert between types
  - [`IInPlaceTransformTool<T>`](TheXDS.Vivianne.Tools.Base.IInPlaceTransformTool-1.html) — Transform in place
- **[`Audio`](TheXDS.Vivianne.Tools.Audio.html)** — Audio manipulation
  - [`AudioRender`](TheXDS.Vivianne.Tools.Audio.AudioRender.html), [`AudioNormalizer`](TheXDS.Vivianne.Tools.Audio.AudioNormalizer.html)
- **[`Fce`](TheXDS.Vivianne.Tools.Fce.html)** — 3D model tools
  - [`FceConverter`](TheXDS.Vivianne.Tools.Fce.FceConverter.html) — Convert between FCE formats
  - [`FceCenter`](TheXDS.Vivianne.Tools.Fce.FceCenter.html) — Calculate model center
  - [`FceDamageGenerator`](TheXDS.Vivianne.Tools.Fce.FceDamageGenerator.html) — Generate damaged models
- **[`Fe`](TheXDS.Vivianne.Tools.Fe.html)** — Front-end tools
  - [`FeDataTextProvider`](TheXDS.Vivianne.Tools.Fe.FeDataTextProvider.html) — Localized text utilities
  - [`FeData3SyncTool`](TheXDS.Vivianne.Tools.Fe.FeData3SyncTool.html), [`FeData4SyncTool`](TheXDS.Vivianne.Tools.Fe.FeData4SyncTool.html)
- **[`Carp`](TheXDS.Vivianne.Tools.Carp.html)** — Car performance analysis
- **[`Misc`](TheXDS.Vivianne.Tools.Misc.html)** — Miscellaneous utilities

### 🔍 Helpers & Info Extractors

Utility functions and metadata extraction.

- **[`TheXDS.Vivianne.Helpers`](TheXDS.Vivianne.Helpers.html)** — Utility classes
  - [`ColorConversion`](TheXDS.Vivianne.Helpers.ColorConversion.html) — Color space conversions
- **[`TheXDS.Vivianne.Info`](TheXDS.Vivianne.Info.html)** — Information extractors
  - [`IEntityInfoExtractor<T>`](TheXDS.Vivianne.Info.IEntityInfoExtractor-1.html) — Extract metadata from entities
  - [`NfsVersion`](TheXDS.Vivianne.Info.NfsVersion.html) — Enum for NFS 2/3/4 versions
  - [`VersionIdentifier`](TheXDS.Vivianne.Info.VersionIdentifier.html) — Detect NFS version from data
  - **[`Audio`](TheXDS.Vivianne.Info.Audio.html)** — Audio file info extractors
  - **[`Fce`](TheXDS.Vivianne.Info.Fce.html)** — FCE model info extractors
  - **[`Map`](TheXDS.Vivianne.Info.Map.html)** — Map/track info extractors

### 📝 Extensions

Extension methods for common types.

- **[`TheXDS.Vivianne.Extensions`](TheXDS.Vivianne.Extensions.html)**
  - [`ArrayExtensions`](TheXDS.Vivianne.Extensions.ArrayExtensions.html) — Array utilities
  - [`FshExtensions`](TheXDS.Vivianne.Extensions.FshExtensions.html) — Texture operations
  - [`FshBlobExtensions`](TheXDS.Vivianne.Extensions.FshBlobExtensions.html) — Texture blob operations
  - [`VivExtensions`](TheXDS.Vivianne.Extensions.VivExtensions.html) — Archive utilities
  - [`MiscExtensions`](TheXDS.Vivianne.Extensions.MiscExtensions.html) — General utilities

### 🏷️ Attributes & Resources

Metadata and localization resources.

- **[`TheXDS.Vivianne.Attributes`](TheXDS.Vivianne.Attributes.html)** — Custom attributes for metadata
- **[`TheXDS.Vivianne.Resources`](TheXDS.Vivianne.Resources.html)** — Localized strings and mappings

---

## Common Usage Patterns

### Reading a File

```csharp
using TheXDS.Vivianne.Serializers.Viv;

var serializer = new VivSerializer();
var vivFile = serializer.Deserialize(File.ReadAllBytes("archive.viv"));
```

### Working with Models

```csharp
using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Models.Fsh;

var fshFile = new FshFile();
var blob = new FshBlob { Name = "MyTexture" };
fshFile.Blobs.Add(blob);
```

### Using Codecs

```csharp
using TheXDS.Vivianne.Codecs;

byte[] compressed = RefPackCodec.Compress(data);
byte[] decompressed = RefPackCodec.Decompress(compressed);
```

### Applying Tools

```csharp
using TheXDS.Vivianne.Tools.Fce;

var converter = new FceConverter();
var fce3Model = converter.Convert(fce4Model, TargetFormat.Fce3);
```

---

## Version-Aware API

Many types support multiple NFS versions (2, 3, 4). Use [`NfsVersion`](TheXDS.Vivianne.Info.NfsVersion.html) and [`VersionIdentifier`](TheXDS.Vivianne.Info.VersionIdentifier.html) to handle version detection and selection.

---

## API Stability

VivLib follows semantic versioning. Public API additions are made thoughtfully, and breaking changes occur only in major version bumps. The API is designed for flexibility without over-engineering.
