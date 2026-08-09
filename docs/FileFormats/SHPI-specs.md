# SHPI file format specification

In this document we'll explore the SHPI file format. All primitive numeric values described in this document are little-endian unless specified otherwise.

> Note: I might interchangeably call it ***FSH*** or ***SHPI***. It's the same file format, I just grew used to call it a FSH (I even pronounce it *"fesh"*) but to be technically correct, it's SHPI.

## Summary

The SHPI file format (with the `.fsh` or, if fully compressed, the `.qfs` file extension) is a common type of file used extensively by Electronic Arts in many of their late 90's and early 2000's games. They are generally (but not exclusively) used to hold textures in a collection where each element can be referenced by a 4 character ID.

## Structures found in the file

Inside an SHPI file, there's a number of structures that serve a specific purpose. Here's a general map of a typical SHPI file:

 Offset  | Length                     | Description
-------- | -------------------------- | ---
`0x0000` | 16 bytes                   | [File header](#file-header). Contains basic information for parsing and reading the SHPI file.
`0x0010` | Multiples of 8             | [SHPI blob entries](#shpi-blob-entry). Each one consumes 8 bytes.
Varies   | Often `0`, varies          | [Optional global data](#global-shpi-data). Some games store arbitrary data associated with the entire SHPI here.
Varies   | From offset to end of file | [Data pool](#shpi-data-pool). Entries reference offsets in this area to Mark as their own data.

### File header

Located at the very beginning of the file, the SHPI header includes a 4-byte magic identifier, file length, number of entries and a special marker known as the _Directory ID_.

- Struct size: 16 bytes

 Offset  | Length  | Value type | Description
-------- | ------- | :--------: | ---
`0x0000` | 4 bytes |  `byte[]`  | Magic identifier. A valid SHPI file will have the string `SHPI`... Unless [compressed](#lz-compression).
`0x0004` | 4 bytes |  `Int32`   | File length. Used to validate the length of the file and the expected amount of data present.
`0x0008` | 4 bytes |  `Int32`   | Number of entries present in the file.
`0x000C` | 4 bytes |  `byte[]`  | Directory ID. While there are several, Texture files (as used in NFS) will have a value of `GIMX`.

> A `QFS` file has its own magic marker which identifies it as an LZ-compressed data stream. After decompression, it contains the same data as any regular `FSH` file. We'll explore this concept [later](#lz-compression).

As for the Directory ID, other possible IDs are: `G240`, `G264`, `G266`, `G290`, `G315`, `G335`, `G344` and `G354`. This list is not exhaustive, and Need For Speed games will only support `GIMX`. Using a different Directory ID does not change how the FSH file is parsed, but might serve as an indication of the kind of FSH blobs that could be found within the FSH file, including non-texture data.

### SHPI Blob entry

Immediately after the header, there's a number of entries that describe the ID and position within the SHPI file for every blob contained.

Each entry has the following structure (Offsets in the following table are relative to itself, not absolute within the SHPI file):

- Struct size: 8 bytes

 Offset  | Length  | Value type | Description
-------- | ------- | :--------: | ---
`0x0000` | 4 bytes |  `byte[]`  | SHPI blob entry ID.
`0x0004` | 4 bytes |  `Int32`   | Entry data offset.

> Note: A 4 byte ID for this entry. Might be (incorrectly?) duplicated, so games and parsers alike need to be aware of that. A game might usually just pick the first match.

> Note: The size of each entry is calculated based on _offset boundaries_, that is, the data being read from the data pool belongs to the SHPI Blob entry being parsed until we reach the data offset of a new entry. This means that the size of the blob needs to be calculated, as it's not specified in the directory.

> Note: The offset is absolute, that is, it starts counting from the very beginning of the entire SHPI file, header included.

### Global SHPI data
You can consider this area as either _"The footer of the header"_ or as _"Data in the pool that is not associated with any specific Directory entry"_. A few games are known to make use of this area, sometimes to store a color palette to be used by all indexed textures in the SHPI file. We'll explore index-color (8-bit with palette) textures later on.

### SHPI data pool
Depending on your own interpretation of the [global SHPI data](#global-shpi-data) section, this block officially starts immediately after the directory entries or at the very first offset referenced from the directory entry table. It includes a non-structured pool of all the raw data in the SHPI file.

A parser can segment and separate this area into chunks wherever there is a reference to an offset. As mentioned before, this is necessary because there is no data within the SHPI blob directory that specifies the size of each blob.

## Data contained in the file
As mentioned earlier, SHPI files are generally used to store textures. For now, we'll explore the data format for textures within the SHPI file.

### SHPI blob
In _VivLib_, a fully parsed SHPI directory entry is known as a SHPI Blob (referred to in code as *FSH*, see the note at the beginning of this document). It includes the raw data pulled from the pool as a structured SHPI Blob texture, sometimes with a footer area that may contain other sub-structures.

Each blob starts with the following header:

- Struct size: 16 bytes

 Offset  | Length  | Value type                   | Description
-------- | ------- | :--------------------------: | ---
`0x0000` | 1 byte  | `byte`                       | Magic. It identifies the type and format of the data contained in the blob. Please refer to the [blob formats table](#blob-formats-table) for more information.
`0x0001` | 3 bytes | 24-bit `Int`                 | Offset (locally referenced) of the start of the footer data. Must be set to zero if there is no footer.
`0x0004` | 2 bytes | `Int16`                      | Image width.
`0x0006` | 2 bytes | `Int16`                      | Image height.
`0x0008` | 2 bytes | `Int16`                      | X Rotation axis coordinate.
`0x000A` | 2 bytes | `Int16`                      | Y Rotation axis coordinate.
`0x000C` | 2 bytes | `Int16`                      | X Location coordinate.
`0x000E` | 2 bytes | `Int16`                      | Y Location coordinate.
`0x0010` | Varies  | `byte[]`                     | Texture pixel data.
Varies   | From end of pixel data to end of blob  | `byte[]`     | Optional SHPI Blob footer.

> Note: When including a footer, the theoretical size limit for the standard SHPI blob (that is, its header and data excluding the footer) is 16777214 bytes. When not including a footer, it's still recommended to avoid surpassing this limitation, even if the SHPI format would technically allow it.

The range `0x0008` - `0x000F` for an SHPI blob is open on the SHPI specification. That is, any game may chose to repurpose those 8 bytes for anything else besides rotation axis. Currently, _VivLib_ assumes that those bytes always mean _'image transform coordinates'_.

The way in which texture data is interpreted depends on the _Magic_ field. Its size can be calculated based on the pixel format and the dimensions of the resulting image. Generally, when a blob contains a footer, the end of the pixel data and the start of the footer should occur at the exact same offset. If the footer were to start within pixel data, that means that the blob is either corrupt or invalid. A buffer may be allowed between the end of pixel data and the start of the footer data, in which case whatever is between these areas may be discarded.

If the blob does not contain any footer data, the footer offset must be set to `0x000000`, and everything up until the next blob offset within the data pool is pixel data. Just like when there is a footer, if the calculated pixel data size were to extend past the next blob offset, the current blob should be considered corrupt or invalid, and any space after the pixel data and the next blob offset may be discarded.

As a general rule, SHPI files should be kept clean of any unwanted space between pixel data and the footer/next blob, although some SHPI files have been observed where blank space exists for alignment reasons (maybe for faster reads on legacy systems or buffered storage media?).

### Blob formats table

- `0x2A`: 32-bit color palette with alpha in RGBA color space, always with 256 entries. It's yet unknown if the games would support color palettes of varying size, although the format technically supports it.
- `0x22`: 24-bit color palette in RGB color space. Like the 32-bit color palette, it always has 256 colors.
- `0x24`: 24-bit color palette in BGR color space.
- `0x29`: 16-bit color palette, Need For Speed 5 variant.
- `0x2D`: 16-bit color palette.
- `0x60`: DXT1 compressed texture. Older games won't support it.
- `0x61`: DXT3 compressed texture. Older games won't support it.
- `0x62`: DXT5 compressed texture. Older games won't support it.
- `0x6D`: 16-bit ARGB-4444 color image.
- `0x78`: 16-bit RGB-565 color image.
- `0x7B`: 8-bit indexed color image. It requires a color palette.
- `0x7D`: 32-bit ARGB32 color image.
- `0x7E`: 16-bit ARGB-1555 color image.
- `0x7F`: 24-bit RGB24 color image.
- `0xF8`: 16-bit RGB-565 color image with LZ compression.
- `0xFD`: 32-bit ARGB32 color image with LZ compression.
- `0xFE`: 16-bit ARGB-1555 color image with LZ compression.

This list may not be exhaustive, as there could be many SHPI blob formats that are not used for textures, not yet known or incompatible with _VivLib_.

### SHPI blob footer
Some arbitrary structures (sometimes referred to as attachments) have been found to exist in SHPI blob footers, depending on the version of the game and purpose of the blob.

The structures described below will have a known length, which allows for a single blob to contain more than one value in its footer data. In newer games, it's standard to see a '[Metal bin](#metal-bin)' attachment alongside a '[Blob name](#blob-name)' attachment.

Of course, Need For Speed III uses the blob footer extensively on cars; read below.

Attachments shall continue to be parsed as long as there is data in the footer still, unless reading an attachment would go outside of the footer data, in which case the remainder of the data shall be discarded.

#### Color palette
This is the most common type of data found in SHPI blobs. It's an embedded SHPI blob itself, with a pixel format that denotes a color palette, but otherwise it follows the same format as a normal SHPI blob texture, including the entire header data. This kind of footer can only be found on SHPI blobs with an indexed pixel format (see the [Blob formats table](#blob-formats-table) for more information).

Being a color palette, some values of the header are expected to fall within the following values:

Offset              | Expected value | Notes
------------------- | :------------: | ---
`0x0004`            | `256`          | Width of 256 pixels, indicating the number of colors defined in the palette.
`0x0006`            | `1`            | Height of 1 pixel. A required formality so that the SHPI blob parser can properly read the color table.
`0x0008` - `0x000F` | All `0x00`     | These 8 bytes are normally set to zero, as they do not represent any special property of the palette.

#### Car dashboard
> This attachment type is exclusive for Need For Speed III.

It's a 104-byte structure that contains several properties for a car dashboard to be drawn by Need For Speed III when the player uses the _Dashboard camera_.

Currently, there are some values whose purpose or actual utility has not yet been identified fully.

 Offset  | Length   | Value type | Description
-------- | -------- | :--------: | ---
`0x0000` | 4 bytes  | `Int32`    | _Unknown data._
`0x0004` | 4 bytes  | `Int32`    | _Unknown data._
`0x0008` | 4 bytes  | `Int32`    | Dial color X coordinate.
`0x000C` | 4 bytes  | `Int32`    | Dial color Y coordinate.
`0x0010` | 4 bytes  | `Int32`    | Dial width at its base.
`0x0014` | 4 bytes  | `Int32`    | Dial width at its tip.
`0x0018` | 40 bytes | Dial data  | Speedometer dial data.
`0x0040` | 40 bytes | Dial data  | Tachometer dial data.

##### Dial data struct
This same struct applies for both speedometers and tachometers.

 Offset  | Length   | Value type | Description
-------- | -------- | :--------: | ---
`0x0000` | 4 bytes  | `Int32`    | Center X coordinate.
`0x0004` | 4 bytes  | `Int32`    | Center Y coordinate.
`0x0008` | 4 bytes  | `Int32`    | Dial offset from the center.
`0x000C` | 4 bytes  | `Int32`    | Edge offset from the tip.
`0x0010` | 4 bytes  | `Int32`    | Minimum indicated value for the dial. Usually set to `0`.
`0x0014` | 4 bytes  | `Int32`    | Maximum indicated value for the dial.
`0x0018` | 4 bytes  | `Int32`    | Minimum indicated value X coordinate.
`0x001C` | 4 bytes  | `Int32`    | Minimum indicated value Y coordinate.
`0x0020` | 4 bytes  | `Int32`    | Maximum indicated value X coordinate.
`0x0024` | 4 bytes  | `Int32`    | Maximum indicated value Y coordinate.

#### _Metal Bin_
This is an 80-byte long structure that starts with the following header: `0x00005069`. The rest of its contents are unknown at this time, although it contains the following text: "`EAGL64 metal bin attachment for runtime texture management`"

This attachment is only seen in newer games that used DirectX 8 or later.

#### Blob name
This is a 16-byte long structure whose internal structure is currently unknown. It starts with the following header: `0x00000070`, and contains a string that should denote a long or friendly name for the current blob.

## LZ Compression
Several SHPI files use a variant of the standard LZ compression algorithm. On older SHPI files, this compression was being applied to the entire file, while in newer ones there is a specific pixel format Magic that indicates that the pixel data for an SHPI blob is compressed, leaving the rest of the SHPI file easily readable.

While this document does not go into details for how the LZ compression algorithm works in general, there are a few points worth mentioning.

When LZ compression is being applied to an entire SHPI file, there will be a magic signature with a value of `0xFB10` before the actual compressed data. Compressed SHPI files will usually have a `.qfs` file extension. This is not a requirement however, thanks to the magic signature it's possible to parse a `.fsh` file and determine if it's compressed or not.

As far as pixel data compression goes, it also includes the same magic signature at the beginning of the pixel data stream. This is to still comply with the same expected structure of the LZ compressed stream. Older games likely won't support LZ-compressed pixel formats, instead expecting the entire SHPI file to be compressed.

## Indexed color textures and color palettes
There's several variants on how indexed color palettes are defined.

- ### Local SHPI color palette
  Early SHPI files included one SHPI blob entry usually named `!pal`, which was a regular SHPI blob with a magic type that corresponds to a color palette. It was intended to be used as the color palette for all SHPI blobs within the same file that had a magic type of `0x7B` (8-bit indexed color image). Every indexed blob referenced colors defined in the `!pal` SHPI blob. A game that is known to use this format is Need For Speed II.

  It is still allowed to have non-indexed SHPI blobs within the same file. While not explicitly forbidden, you should not have more than one color palette SHPI blob inside of the same SHPI file.

- ### Embedded color palette
  Later on, SHPI files that had indexed color texture SHPI blobs opted to embed the color palette SHPI blob as a footer on each indexed texture. this had the advantage of allowing more flexibility in terms of color variety when two or more indexed textures existed within the same SHPI file, at a small cost in terms of file size.

- ### Unreferenced local SHPI color palette
  A closely related variant to the Local SHPI color palette. It took advantage of the unreferenced area at the beginning of the SHPI data pool, but it is otherwise the same as a SHPI file with a `!pal` SHPI blob entry.

  Therefore, it's entirely possible to go from one variant to another by just adding the `!pal` entry pointing to the palette data embedded right after the SHPI blob entries table.

- ### Global/undefined color palette
  In this case, there will be no color palettes defined for an SHPI blob, be it because the SHPI file does not contain one outright, or because the SHPI blob does not include it as an embedded palette. In this case, games may either generate a standard palette, not render the texture correctly if at all, or somehow reuse a palette defined in some other place.

  As an oddity, some GPU/OS configurations had historically caused the palette data to be either corrupted or lost in Need For Speed II, resulting in wild rainbow colors to be displayed when starting a race. This would be the same as not having a palette available in an SHPI file.

While it's not yet known if a single game would support more than one variant of these palette definitions at the same time, it's reasonable to assume that they would have to follow a priority of definition, where an embedded palette takes precedence over all other color palettes, and likewise, a global color palette can be overridden by any other color palette within an SHPI file.
