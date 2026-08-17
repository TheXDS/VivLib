# FCE file format specification

In this document, we'll explore the FCE (3D Mesh) file format used by Electronic Arts in Need For Speed III and Need For Speed IV.

## Summary

The FCE file format stores 3D car model data for vehicles in Need For Speed III and Need For Speed IV. Each file contains a hierarchical mesh of parts (body panels, wheels, etc.), each with vertices, normals, and textured triangles. The format evolved significantly between NFS3 and NFS4, with NFS4 adding damaged/destroyed vehicle states, interior/driver colors, and window material flags.

- **NFS3:** Single state mesh with primary and secondary color palettes
- **NFS4:** Extended mesh with damage states (damaged vertices/normals), interior colors, driver/hair colors, and window-specific material flags

All three variants share the same fundamental mesh structure, with NFS4 extending the header and part data with additional tables.

## NFS3 FCE Format

The NFS3 FCE file is a **binary** format with a fixed-size header followed by variable-length data tables. All primitive numeric values are **little-endian** unless otherwise specified.

### NFS3 Header Structure

The header is **7,940 bytes** (`0x1F04`) in size and contains all table offsets, color palettes, part metadata, and dummy object definitions.

- Struct size: `0x1F04` (7,940 bytes)

The header begins with:

 Offset  | Length  | Value type         | Description
-------- | ------- | :----------------: | ---
`0x0000` | 4 bytes | `Int32`            | Magic identifier.
`0x0004` | 4 bytes | `Int32`            | Number of triangles in the model.
`0x0008` | 4 bytes | `Int32`            | Number of vertices in the model.
`0x000C` | 4 bytes | `Int32`            | Number of arts (texture pages). Typically `1`.
`0x0010` | 4 bytes | `Int32`            | Vertex table offset from `0x1F04`.
`0x0014` | 4 bytes | `Int32`            | Normals table offset from `0x1F04`.
`0x0018` | 4 bytes | `Int32`            | Triangles table offset from `0x1F04`.
`0x001C` | 4 bytes | `Int32`            | Reserved area #1 offset from `0x1F04`.
`0x0020` | 4 bytes | `Int32`            | Reserved area #2 offset from `0x1F04`.
`0x0024` | 4 bytes | `Int32`            | Reserved area #3 offset from `0x1F04`.
`0x0028` | 4 bytes | `float`            | X half-size of the whole model.
`0x002C` | 4 bytes | `float`            | Y half-size of the whole model.
`0x0030` | 4 bytes | `float`            | Z half-size of the whole model.
`0x0034` | 4 bytes | `Int32`            | Number of dummy objects. Valid values: `0` to `16`.
`0x0038` | 192 bytes | `Vector3[16]`    | Coordinates of dummy objects (light source positions).
`0x00F8` | 4 bytes | `Int32`            | Number of car parts. Valid values: `0` to `64`.
`0x00FC` | 768 bytes | `Vector3[64]`    | Global coordinates of car parts (local coordinate system origins).
`0x03FC` | 256 bytes | `Int32[64]`      | First vertex index for each part.
`0x04FC` | 256 bytes | `Int32[64]`      | Number of vertices for each part.
`0x05FC` | 256 bytes | `Int32[64]`      | First triangle index for each part.
`0x06FC` | 256 bytes | `Int32[64]`      | Number of triangles for each part.
`0x07FC` | 4 bytes | `Int32`            | Number of primary colors. Valid values: `0` to `16`.
`0x0800` | 256 bytes | `HsbColor[16]`    | Primary color table.
`0x0900` | 4 bytes | `Int32`            | Number of secondary colors. Valid values: `0` to `16`.
`0x0904` | 256 bytes | `HsbColor[16]`    | Secondary color table.
`0x0A04` | 1,024 bytes | `byte[64][16]` | Dummy object names (Latin-1 encoded, max 63 chars, null-terminated).
`0x0E04` | 4,096 bytes | `byte[64][64]` | Part names (Latin-1 encoded, max 63 chars, null-terminated).
`0x1E04` | 256 bytes | `byte[256]`      | Unknown data table. Purpose is undocumented.

> Note: The half-size values (`XHalfSize`, `YHalfSize`, `ZHalfSize`) represent half the total dimension of the model along each axis. To get the full size, multiply by `2.0`.

### Color Structure (NFS3)

Each color entry uses a 16-byte HSB (Hue-Saturation-Brightness) structure with an alpha channel:

- Struct size: 16 bytes

All components are stored as `Int32` values in the range 0–255:

 Offset (within struct) | Length  | Value type  | Description
----------------------- | ------- | :----------: | ---
`0x0000` | 4 bytes | `Int32` | Hue. Encoded as `degrees / 360 * 255`.
`0x0004` | 4 bytes | `Int32` | Saturation. Encoded as `percentage / 100 * 255`.
`0x0008` | 4 bytes | `Int32` | Brightness. Encoded as `percentage / 100 * 255`.
`0x000C` | 4 bytes | `Int32` | Alpha/transparency.

> Note: NFS3 colors are stored as `Int32` internally but clamped to `byte` range (0–255) when accessed. Conversion to RGB uses the standard HSB-to-RGB algorithm.

### Triangle Structure (NFS3)

Each triangle is **56 bytes** (`0x38`) in size:

 Offset (within struct) | Length  | Value type          | Description
----------------------- | ------- | :----------------: | ---
`0x0000` | 4 bytes | `Int32`           | Texture page number.
`0x0004` | 4 bytes | `Int32`           | Vertex #1 index (local to part).
`0x0008` | 4 bytes | `Int32`           | Vertex #2 index (local to part).
`0x000C` | 4 bytes | `Int32`           | Vertex #3 index (local to part).
`0x0010` | 12 bytes| `byte[12]`        | Unknown data. All items are `0xFF00` (little-endian `short` values).
`0x001C` | 4 bytes | `MaterialFlags`   | Smoothing and rendering flags.
`0x0020` | 4 bytes | `float`           | Vertex #1 texture U-coordinate.
`0x0024` | 4 bytes | `float`           | Vertex #2 texture U-coordinate.
`0x0028` | 4 bytes | `float`           | Vertex #3 texture U-coordinate.
`0x002C` | 4 bytes | `float`           | Vertex #1 texture V-coordinate.
`0x0030` | 4 bytes | `float`           | Vertex #2 texture V-coordinate.
`0x0034` | 4 bytes | `float`           | Vertex #3 texture V-coordinate.

> Note: Vertex indices are **local** to each part. To get global indices, add the offset from the `P1stVertices` table (file offset `0x03FC`).

### Material Flags (NFS3)

The `MaterialFlags` enum defines triangle rendering properties:

| Flag | Value | Description |
|------|-------|-------------|
| `None` | `0x00` | No flags. Rendered as semi-glossy textured material. |
| `NoBlending` | `0x01` | No blending. Rendered as matte textured material. |
| `HighBlending` | `0x02` | High blending. Rendered as high-gloss textured material. |
| `NoCulling` | `0x04` | No culling. Triangles drawn on both faces. |
| `Semitrans` | `0x08` | Semi-transparent, semi-glossy textured material. |
| `SemitransNoBlending` | `0x09` | Semi-transparent, matte textured material. |
| `SemitransHighBlending` | `0x0A` | Semi-transparent, high-gloss textured material. |
| `NcNoBlending` | `0x05` | No culling, no blending. |
| `NcHighBlending` | `0x06` | No culling, high blending. |
| `NcSemitrans` | `0x0C` | No culling, semi-transparent. |
| `NcSemitransNoBlending` | `0x0D` | No culling, semi-transparent, no blending. |
| `NcSemitransHighBlending` | `0x0E` | No culling, semi-transparent, high blending. |

### Data Tables (After Header)

All data tables are stored **after** the `0x1F04` byte header. Offsets in the header are relative to this base.

#### Vertex Table

 Offset      | Length                          | Type
------------ | ------------------------------- | ---
`0x1F04 + VertTblOffset` | `NumVertices * 12` bytes | `Vector3[NumVertices]`

Each vertex is a `Vector3` (12 bytes: X, Y, Z as `float`). All coordinates are **local**. Use the `Parts` table (header offset `0x00FC`) to transform them to global space.

#### Normals Table

 Offset      | Length                          | Type
------------ | ------------------------------- | ---
`0x1F04 + NormTblOffset` | `NumVertices * 12` bytes | `Vector3[NumVertices]`

Each normal is a `Vector3` (12 bytes). Should be normalized. Each normal at index `i` corresponds to the vertex at index `i` in the vertex table.

#### Triangles Table

 Offset      | Length                           | Type
------------ | -------------------------------- | ---
`0x1F04 + TriaTblOffset` | `NumTriangles * 56` bytes | `FceTriangle[NumTriangles]`

#### Reserved Areas

| Area | Offset | Length | Type |
|------|--------|--------|------|
| #1 | `0x1F04 + Reserve1offset` | `NumVertices * 32` bytes | Unknown |
| #2 | `0x1F04 + Reserve2offset` | `NumVertices * 12` bytes | Unknown |
| #3 | `0x1F04 + Reserve3offset` | `NumVertices * 12` bytes | Unknown |

> Note: All three reserved areas must exist in valid FCEv3 files. Their purpose is undocumented, but they may be related to rendering state or legacy features.

### Coordinate System

From the car's perspective:

- **X axis** points to the right
- **Y axis** points upward
- **Z axis** points forward

## NFS4 FCE Format

The NFS4 FCE format extends the NFS3 format with additional header fields, damage state data, interior colors, driver/hair colors, and window material flags.

### NFS4 Header Structure

The header is larger than NFS3's, approximately **`0x1E28` + 528 bytes** (varies based on internal padding):

- Struct size: Variable (approximately 7,720+ bytes)

 Offset  | Length  | Value type         | Description
-------- | ------- | :----------------: | ---
`0x0000` | 4 bytes | `Int32`            | Magic identifier.
`0x0004` | 4 bytes | `Int32`            | Unknown value. Purpose is undocumented.
`0x0008` | 4 bytes | `Int32`            | Number of triangles in the model.
`0x000C` | 4 bytes | `Int32`            | Number of vertices in the model.
`0x0010` | 4 bytes | `Int32`            | Number of arts (texture pages). Typically `1`.
`0x0014` | 4 bytes | `Int32`            | Vertex table offset from header start.
`0x0018` | 4 bytes | `Int32`            | Normals table offset from header start.
`0x001C` | 4 bytes | `Int32`            | Triangles table offset from header start.
`0x0020` | 4 bytes | `Int32`            | Reserved area #1 offset.
`0x0024` | 4 bytes | `Int32`            | Reserved area #2 offset.
`0x0028` | 4 bytes | `Int32`            | Reserved area #3 offset.
`0x002C` | 4 bytes | `Int32`            | Undamaged vertex table offset.
`0x0030` | 4 bytes | `Int32`            | Undamaged normals table offset.
`0x0034` | 4 bytes | `Int32`            | Damaged vertex table offset.
`0x0038` | 4 bytes | `Int32`            | Damaged normals table offset.
`0x003C` | 4 bytes | `Int32`            | Reserved area #4 offset.
`0x0040` | 4 bytes | `Int32`            | Animation table offset.
`0x0044` | 4 bytes | `Int32`            | Reserved area #5 offset.
`0x0048` | 4 bytes | `Int32`            | Reserved area #6 offset.
`0x004C` | 4 bytes | `float`            | X half-size of the whole model.
`0x0050` | 4 bytes | `float`            | Y half-size of the whole model.
`0x0054` | 4 bytes | `float`            | Z half-size of the whole model.
`0x0058` | 4 bytes | `Int32`            | Number of dummy objects. Valid values: `0` to `16`.
`0x005C` | 192 bytes | `Vector3[16]`    | Coordinates of dummy objects.
`0x011C` | 4 bytes | `Int32`            | Number of car parts. Valid values: `0` to `64`.
`0x0120` | 768 bytes | `Vector3[64]`    | Global coordinates of car parts.
`0x0420` | 256 bytes | `Int32[64]`      | First vertex index for each part.
`0x0520` | 256 bytes | `Int32[64]`      | Number of vertices for each part.
`0x0620` | 256 bytes | `Int32[64]`      | First triangle index for each part.
`0x0720` | 256 bytes | `Int32[64]`      | Number of triangles for each part.
`0x0820` | 4 bytes | `Int32`            | Number of color entries per palette. Valid values: `0` to `16`.
`0x0824` | 256 bytes | `HsbColor[16]`   | Primary color table.
`0x0924` | 256 bytes | `HsbColor[16]`   | Interior color table.
`0x0A24` | 256 bytes | `HsbColor[16]`   | Secondary color table.
`0x0B24` | 256 bytes | `HsbColor[16]`   | Driver/hair color table.
`0x0C24` | 4 bytes | `Int32`            | Unknown integer value.
`0x0C28` | 256 bytes | `byte[256]`      | Unknown data table.
`0x0D28` | 1,024 bytes | `byte[64][16]` | Dummy object names (Latin-1 encoded).
`0x1128` | 4,096 bytes | `byte[64][64]` | Part names (Latin-1 encoded).
`0x2128` | 528 bytes | `byte[528]`      | Unknown data table.

> Note: Unlike NFS3, the NFS4 header does **not** use `0x1F04` as the base offset for data tables. Instead, the vertex/normals/triangle data starts immediately after the header struct, and all table offsets are relative to the header start (not an arbitrary base).

### Color Structure (NFS4)

Each color entry uses an 8-byte HSB structure with an alpha channel:

- Struct size: 8 bytes

All components are stored as `byte` values (0–255):

 Offset (within struct) | Length  | Value type | Description
----------------------- | ------- | :--------: | ---
`0x0000` | 1 byte | `byte` | Hue. Encoded as `degrees / 360 * 255`.
`0x0001` | 1 byte | `byte` | Saturation. Encoded as `percentage / 100 * 255`.
`0x0002` | 1 byte | `byte` | Brightness. Encoded as `percentage / 100 * 255`.
`0x0003` | 1 byte | `byte` | Alpha/transparency.

> Note: NFS4 colors are more compact than NFS3, using 1-byte components instead of 4-byte integers. This reduces the color table size from 256 bytes per palette (NFS3) to 64 bytes per palette (NFS4).

### Triangle Structure (NFS4)

The triangle structure is identical to NFS3's **56-byte** (`0x38`) layout:

 Offset (within struct) | Length  | Value type          | Description
----------------------- | ------- | :----------------: | ---
`0x0000` | 4 bytes | `Int32`           | Texture page number.
`0x0004` | 4 bytes | `Int32`           | Vertex #1 index (local to part).
`0x0008` | 4 bytes | `Int32`           | Vertex #2 index (local to part).
`0x000C` | 4 bytes | `Int32`           | Vertex #3 index (local to part).
`0x0010` | 12 bytes| `byte[12]`        | Unknown data. All items are `0xFF00` (little-endian `short` values).
`0x001C` | 4 bytes | `MaterialFlags`   | Smoothing and rendering flags.
`0x0020` | 4 bytes | `float`           | Vertex #1 texture U-coordinate.
`0x0024` | 4 bytes | `float`           | Vertex #2 texture U-coordinate.
`0x0028` | 4 bytes | `float`           | Vertex #3 texture U-coordinate.
`0x002C` | 4 bytes | `float`           | Vertex #1 texture V-coordinate.
`0x0030` | 4 bytes | `float`           | Vertex #2 texture V-coordinate.
`0x0034` | 4 bytes | `float`           | Vertex #3 texture V-coordinate.

### Material Flags (NFS4)

NFS4 extends the NFS3 material flags with window-specific flags for the damage system:

| Flag | Value | Description |
|------|-------|-------------|
| `None` | `0x00` | No flags. Rendered as semi-glossy textured material. |
| `NoBlending` | `0x01` | No blending. Rendered as matte textured material. |
| `HighBlending` | `0x02` | High blending. Rendered as high-gloss textured material. |
| `NoCulling` | `0x04` | No culling. Triangles drawn on both faces. |
| `Semitrans` | `0x08` | Semi-transparent, semi-glossy textured material. |
| `Unknown_Elni` | `0x10` | Flag seen on NFS4's La Niña (Elni) car model. Unknown purpose, might be trash data. |
| `WindowGeneric` | `0x20` | Generic window element (breakable). |
| `FrontWindow` | `0x40` | Windshield (front window). |
| `LeftWindow` | `0x80` | Left window. |
| `BackWindow` | `0x100` | Rear window. |
| `RightWindow` | `0x200` | Right window. |
| `BrokenWindow` | `0x400` | Generic broken window. |
| `Unk_0x0800` | `0x800` | Unknown flag seen on some car models. Might be trash data. |
| `Unk_0x1000` | `0x1000` | Unknown flag seen on some car models. Might be trash data. |

> Note: The window flags are used by the NFS4 damage system to identify which triangles form the car's windows. When a window is destroyed, these triangles are replaced with fragments from the damaged vertex/normal buffers.

### NFS4 Additional Data Tables

NFS4 adds several new data tables to support the damage system:

#### Damaged Vertices & Normals

| Table | Offset | Length | Type |
|-------|--------|--------|------|
| Damaged vertices | `0x1F04 + DamagedVertexTblOffset` | `NumVertices * 12` bytes | `Vector3[NumVertices]` |
| Damaged normals | `0x1F04 + DamagedNormalsTblOffset` | `NumVertices * 12` bytes | `Vector3[NumVertices]` |

> Note: The damaged vertex/normal tables share the same vertex count as the original mesh. Each damaged vertex at index `i` corresponds to the original vertex at index `i`. When a part is damaged, the damaged vertices replace the original vertices during rendering.

#### Additional Reserved Areas

| Area | Offset | Length | Type |
|------|--------|--------|------|
| #4 | `0x1F04 + Rsvd4Offset` | `NumVertices * 4` bytes | Unknown |
| #5 | `0x1F04 + Rsvd5Offset` | `NumVertices * 4` bytes | Unknown |
| #6 | `0x1F04 + Rsvd6Offset` | `NumTriangles * 12` bytes | Unknown |

#### Animation Table

| Table | Offset | Length | Type |
|-------|--------|--------|------|
| Animation data | `0x1F04 + AnimationTblOffset` | `NumVertices * 4` bytes | Unknown |

> Note: The animation table's purpose is undocumented. It may be related to animated parts (doors, trunk, hood) or damage transitions.

### Part Structure (NFS4)

NFS4 parts extend the NFS3 structure with damaged state data:

| Property | Type | Description |
|----------|------|-------------|
| `Name` | `string` (64 bytes max) | Name of the car part. |
| `Origin` | `Vector3` | Global origin coordinate (from `Parts` table). |
| `Vertices` | `Vector3[]` | Original vertices (local coordinates). |
| `Normals` | `Vector3[]` | Original normals. |
| `Triangles` | `FceTriangle[]` | Triangle definitions. |
| `DamagedVertices` | `Vector3[]` | Damaged/destroyed state vertices. |
| `DamagedNormals` | `Vector3[]` | Damaged/destroyed state normals. |

> Note: The damaged vertices share the same count as the original vertices for each part. The `TransformedDamagedVertices` property (in VivLib) applies the part's origin transformation to the damaged vertices, similar to how original vertices are transformed.

## Color Table Summary

| Version | Tables | Size per color | Palette size |
|---------|--------|---------------|--------------|
| NFS3 | Primary, Secondary | 16 bytes (`Int32[4]`) | Up to 16 colors each |
| NFS4 | Primary, Interior, Secondary, Driver/Hair | 8 bytes (`byte[4]`) | Up to 16 colors each |

> Note: VivLib recommends keeping color palettes to **10 colors or fewer** due to limitations in the related FeData file format, which provides color names for up to 10 entries. When the secondary color table contains no elements, renderers should use the primary palette. When it contains a single element, that color is used for all color combinations.

## Geometry and Mesh Structure

### Parts and Vertices

Each FCE file contains a list of **parts** (body panels, wheels, mirrors, etc.), each with:

- **Name**: Null-terminated Latin-1 string, up to 63 characters (64-byte storage).
- **Origin**: `Vector3` global coordinate for the part's local coordinate system.
- **Vertices**: Array of `Vector3` positions (local coordinates).
- **Normals**: Array of `Vector3` surface normals (one per vertex, same count as vertices).
- **Triangles**: Array of `FceTriangle` definitions (one per triangle face).

Vertex and triangle indices within a part are **local** to that part. To get global indices:

```
GlobalVertexIndex = PartVertexOffset[partIndex] + LocalVertexIndex
GlobalTriangleIndex = PartTriangleOffset[partIndex] + LocalTriangleIndex
```

### Dummies

Each FCE file contains up to **16 dummy objects** (light sources or attachment points), each with:

- **Name**: Null-terminated Latin-1 string, up to 63 characters (16-byte storage).
- **Position**: `Vector3` global coordinate.

> Note: Dummies are commonly used for attachment points (e.g., exhaust particles, tire smoke) or light sources.

## File Size Calculation

The total file size of an FCE archive can be calculated as:

```
FileSize = 0x1F04 + Max(offset + length) for all tables
```

Where the offset and length of each table is determined by the header fields. The actual size depends on:

- Number of vertices (`NumVertices`)
- Number of triangles (`NumTriangles`)
- Number of parts (`NumParts`)
- Number of colors (`NumPriColors`, `NumSecColors`)

## Coordinate System

From the car's perspective (identical in NFS3 and NFS4):

- **X axis** points to the right
- **Y axis** points upward
- **Z axis** points forward

## Architecture Notes

### Comparison to Other 3D Formats

| Aspect | FCE | Common modern formats (FBX, OBJ, glTF) |
|--------|-----|---------------------------------------|
| Part hierarchy | Flat array (no parent-child) | Tree/hierarchy |
| UV mapping | Per-triangle (3 floats per vertex per face) | Per-vertex |
| Normals | Per-vertex (shared across triangles) | Per-vertex or per-face |
| Materials | None (all triangles share one texture page) | Per-material |
| Color | HSB palette (16 colors max) | Per-vertex or per-face |
| Damage states | None (NFS3) / Shared vertex buffer (NFS4) | N/A |
| Animation | None (NFS3) / Unknown table (NFS4) | Bone/skeleton animation |

### Use Cases in NFS Games

FCE files are used in Need For Speed III and Need For Speed IV to store all 3D car models. Each car typically consists of:

- **Multiple FCE files**: One per car variant (e.g., `CAR.FCE` for the base model, additional files for body kits, spoilers, etc.)
- **Paired with FeData**: For localized car information (name, specs, color names)
- **Paired with CARP.TXT**: For car performance data (engine, gears, handling)
- **Paired with .TGA files**: For car textures
- **Paired with .BNK files**: For car-specific sound effects

In NFS4, the damage system uses the additional vertex/normal buffers to swap between undamaged and damaged states of the car model, enabling the game's destruction effects.
