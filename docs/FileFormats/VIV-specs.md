# VIV file format specification

In this document we'll explore the VIV file format used by Electronic Arts in their late 90's and early 2000's games, particularly the Need For Speed series.

## Summary

The VIV file format (with the `.viv` file extension) is a generic archive format used extensively by Electronic Arts to bundle multiple game assets into a single file. It functions as a simple flat-file container with a directory of file names, offsets, and lengths, followed by a data pool containing the actual file contents.

## Structures found in the file

Inside a VIV file, there's a number of structures that serve a specific purpose. Here's a general map of a typical VIV file:

 Offset  | Length    | Description
-------- | --------- | ---
`0x0000` | 16 bytes  | [File header](#file-header). Contains basic information for parsing and reading the VIV file.
`0x0010` | Variable  | [Directory entries](#viv-directory-entry). Each entry includes offset, length, and a null-terminated filename.
Varies   | From header end to data pool | [Optional padding](#alignment-and-padding). Extra bytes for alignment purposes.
Varies   | From data pool offset to end of file | [Data pool](#viv-data-pool). Raw file contents referenced by the directory entries.

### File header

Located at the very beginning of the file, the VIV header includes a 4-byte magic identifier, file length, number of entries, and the offset to the data pool.

- Struct size: 16 bytes

All numeric values in the header are **big-endian** (network byte order), which was common for EA games of this era.

 Offset  | Length  | Value type         | Description
-------- | ------- | :----------------: | ---
`0x0000` | 4 bytes | `byte[]`           | Magic identifier. A valid VIV file will have the string `BIGF` (uppercase).
`0x0004` | 4 bytes | `Int32` (big-endian) | File length. Indicates the total size of the file in bytes, as stored in the header. Used for memory allocation or quick size validation.
`0x0008` | 4 bytes | `Int32` (big-endian) | Number of entries present in the file's directory.
`0x000C` | 4 bytes | `Int32` (big-endian) | Data pool offset. Indicates where the data pool begins within the file.

> Note: The magic value `BIGF` is case-sensitive. The file format is sometimes colloquially referred to as "VIV" based on the file extension, but the internal identifier is `BIGF`.

> Note: The file length in the header is primarily used for sanity checks. If the actual file size differs from this value, a warning may be issued, but parsing may still proceed depending on the implementation.

### VIV Directory Entry

Immediately after the file header, there's a series of directory entries that describe each file contained in the archive. Each entry consists of a fixed-size binary structure followed by a null-terminated filename string.

Each entry has the following structure (Offsets in the following table are relative to the start of the entry itself, not absolute within the VIV file):

- Binary struct size: 8 bytes

 Offset  | Length  | Value type         | Description
-------- | ------- | :----------------: | ---
`0x0000` | 4 bytes | `Int32` (big-endian) | File data offset. An absolute offset within the VIV file pointing to the start of this entry's data in the data pool.
`0x0004` | 4 bytes | `Int32` (big-endian) | File data length. The size of the file's data in bytes.

> Note: Immediately after the 8-byte binary struct, there's a null-terminated UTF-8/ASCII filename string. The total size of each directory entry is therefore `8 + filename_length + 1` bytes.

> Note: The filename is **not** a full path — it's just the filename with extension (e.g., `texture.fsh`, `car.fce`). Directory structures, if any, are not encoded in the VIV format itself; all files exist in a flat namespace.

### Alignment and Padding

Some VIV files contain extra bytes between the end of the directory entries and the start of the data pool (as indicated by `PoolOffset` in the header). These bytes are typically used for alignment purposes, possibly to optimize reads on legacy systems or buffered storage media.

Implementations should read from the `PoolOffset` value in the header rather than calculating the position based on the directory entries, as this ensures correct handling of any padding.

### VIV Data Pool

The data pool is the final section of the VIV file, starting at the offset specified in the file header's `PoolOffset` field. It contains the raw binary contents of all files bundled in the archive.

Unlike the SHPI (FSH) format, which requires parsers to infer blob boundaries based on adjacent entries, the VIV format explicitly stores the length of each file in its directory entry. This means:

- Each file's data is exactly `Length` bytes, starting at the specified `Offset`.
- There is no ambiguity about where one file's data ends and another begins.
- Files are stored in the order they appear in the directory entries, not necessarily sorted or ordered in any particular way.

> Note: The data pool does not contain any additional metadata, headers, or markers for individual files. It is a raw concatenation of file contents.

## Sorting and Organization

In VivLib, the VIV format supports several sorting strategies that determine how files are laid out when writing a VIV archive. These sorting options affect the `Offset` values in the directory entries and the order of file data in the pool.

### Sort options

The following sorting strategies are available:

| Option       | Description                                                  |
|-------------|--------------------------------------------------------------|
| `Directory`  | No sorting. Files are laid out exactly as declared in the directory. |
| `FileName`   | Sort by filename, case-insensitive, using invariant culture comparison. |
| `FileType`   | Sort by file extension, then by filename.                    |
| `FileKind`   | Sort by semantic file category, then by filename.            |
| `FileSize`   | Sort by file size in descending order.                       |
| `FileOffset` | Sort by data offset (ascending).                             |

### File kind categories

When using the `FileKind` sort option, files are grouped into the following categories (in priority order):

1. **Text files** (`.txt`)
2. **Front-End data files** (`.eng`, `.bri`, `.fre`, `.ger`, `.ita`, `.spa`, `.swe`)
3. **Texture files** (`.fsh`, `.qfs`)
4. **TGA textures** (`.tga`)
5. **3D models** (`.fce`, `.geo`)
6. **Audio files** (`.bnk`)

Files with extensions not matching any of these categories are placed after all categorized groups, sorted alphabetically.

## Filename Deduplication

The VIV format uses a flat namespace, which means duplicate filenames can occur. When writing a VIV file, VivLib handles duplicates by appending a numeric suffix:

- Original: `texture.fsh`
- Duplicate 1: `texture (1).fsh`
- Duplicate 2: `texture (2).fsh`

When reading a VIV file, if the format already contains deduplicated names (e.g., `texture (1).fsh`), VivLib strips the numeric suffix to preserve the original naming convention. This is done via the pattern ` <number>` at the end of the filename (without extension).

## File Size Calculation

The total file size of a VIV archive can be calculated from its directory entries using the following formula:

```
FileSize = 16 + Σ(FileNameLength + 9 + FileDataLength)
```

Where:
- `16` is the fixed size of the file header
- `FileNameLength` is the number of bytes in the filename (excluding the null terminator)
- `9` is the sum of the 8-byte directory entry struct + 1 byte for the null terminator
- `FileDataLength` is the size of the file's data in bytes

## Architecture Notes

### Comparison to SHPI (FSH) Format

The VIV format is conceptually similar to the SHPI (FSH) format in that both use a directory of entries followed by a data pool. However, there are key differences:

| Aspect          | VIV Format                    | SHPI (FSH) Format                  |
|-----------------|-------------------------------|------------------------------------|
| Entry size      | Fixed 8 bytes + filename      | Fixed 8 bytes + filename           |
| Data boundaries | Explicit `Length` field       | Inferred from next entry's offset  |
| Data pool       | Raw file contents             | May contain global data (e.g., palettes) |
| File metadata   | None (flat container)         | 16-byte blob header per entry      |
| Magic marker    | `BIGF`                        | `SHPI` (or `LZ` if compressed)     |
| Compression     | None                          | Optional LZ compression            |
| Main purpose    | General file archive          | Texture dictionary                 |

### Use Cases in NFS Games

VIV files are used across the Need For Speed series (NFS 2, 3, 4) to bundle various types of game assets, including:

- Texture files (`.fsh`, `.qfs`, `.tga`)
- 3D car models (`.fce`, `.geo`)
- Audio files (`.bnk`)
- Front-end car data (`.eng`, `.bri`, `.fre`, etc.)
- Configuration and text files (`.txt`)

The flat namespace and simple directory structure make VIV files easy to parse and modify, which was important for modding communities working with these games.
