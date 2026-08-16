# FeData file format specs
In this document, we'll explore the format used by Front-End data (FeData) files, as used in Need For Speed III and Need For Speed IV.

## Summary
The Front-End data files (known through _VivLib_ as FeData files) are special files that include localized text for all relevant information about a car in the game. They include the car name, basic car properties, car color scheme names and showcase text information.

They come in several file extensions, where each one describes their intended target language and culture. These extensions are:

- **`.BRI`**: English (UK) localization. Units of measure will be local to this region.
- **`.ENG`**: English (US) localization. Units of measure will be local to this region.
- **`.FRE`**: French localization. Units of measure will be local to Canadian-French region.
- **`.GER`**: German localization. Units of measure will be local to this region.
- **`.ITA`**: Italian localization. Units of measure will be local to this region.
- **`.SPA`**: Spanish localization. Units of measure will be local to the Iberian (Spain) region.
- **`.SWE`**: Sweedish localization. Units of measure will be local to this region.

Other regions are (to the best of my knowledge) not supported. Notably, there's the absence of Portuguese (Both Iberian and Brazilian), and Latin American regions (both Portuguese and Spanish are slightly different in America vs. Europe). Eastern-European, African and Asian cultures are also missing, but given the poor support for Unicode back when these games were made, this is expected to some extent.

Also, while Both Need For Speed III and Need For Speed IV cars include FeData files, their internal header structure is vastly different from each other. NFS III uses a compact 47-byte header with straightforward field layout, while NFS IV uses a much larger structure (over 900 bytes) with extensive padding, a single-byte magic identifier, and a completely rearranged field order. Despite these differences, both versions share the same string offset table mechanism and the same set of localized string fields.

## Structures found in the file (Need For Speed III)

Inside a FeData file, there's a number of structures that serve a specific purpose. Here's a general map of a typical FeData file:

 Offset  | Length    | Description
-------- | --------- | ---
`0x0000` | 47 bytes  | [File header](#file-header). Contains basic car properties, not including any localized text.
`0x002F` | 160 bytes | [String offsets table](#string-offsets-table). Contains 40 entries, each representing an offset 4 bytes long.
`0x00CF` | Varies    | [String data](#string-data). Pool that contains all the localized strings.

### File header

Located at the very beginning of the file, the FeData header includes basic properties for the car, like its serial number, car class, engine location, compare data, etc.

- Struct size: 47 bytes

 Offset  | Length  | Value type         | Description
-------- | ------- | :----------------: | ---
`0x0000` | 4 bytes |      `byte[]`      | 4-character car ID. Must be unique for each car.
`0x0004` | 2 bytes |      `ushort`      | Number of flags/basic props contained in the file. It always has a value of `9`.
`0x0006` | 2 bytes | `ushort` as `bool` | "Is Bonus". When not zero, indicates that the car is an unlockable bonus car.
`0x0008` | 2 bytes | `ushort` as `bool` | When not zero, indicates that the car should be available to AI drivers.
`0x000A` | 2 bytes |      `ushort`      | Car class, from top-tier ("A", `0x0000`) to lowest ("C", `0x0002`). Other values outside this range are not valid.
`0x000C` | 2 bytes |      `ushort`      | This value is currently undocumented. It must have a value of `0x0003`.
`0x000E` | 2 bytes | `ushort` as `bool` | When not zero, indicates that the car is part of a DLC (Downloadable Content Pack).
`0x0010` | 2 bytes | `ushort` as `bool` | When not zero, indicates that the car is a Police vehicle.
`0x0012` | 2 bytes |      `ushort`      | Indicates the seat position. `0x0000` indicates _Left_, `0x0001` indicates _Right_ and `0x0002` _Center_.
`0x0014` | 2 bytes |      `ushort`      | This value is currently undocumented. It must have a value of `0x0000`.
`0x0016` | 2 bytes |      `ushort`      | This value is currently undocumented. Likely to include custom flags.
`0x0018` | 2 bytes |      `ushort`      | Car serial number. Like the ID, this value must be unique for each car.
`0x001A` | 2 bytes |      `ushort`      | This value is currently undocumented. It must have a value of `0x0000`.
`0x001C` | 2 bytes |      `ushort`      | This value is currently undocumented. It must have a value of `0x0000`.
`0x001E` | 2 bytes |      `ushort`      | This value is currently undocumented. It must have a value of `0x0000`.
`0x0020` | 2 bytes |      `ushort`      | This value is currently undocumented. It must have a value of `0x0000`.
`0x0022` | 2 bytes |      `ushort`      | This value is currently undocumented. It must have a value of `0x0000`.
`0x0024` | 2 bytes |      `ushort`      | This value is currently undocumented. It must have a value of `0x0000`.
`0x0026` | 2 bytes |      `ushort`      | This value is currently undocumented. It must have a value of `0x0000`.
`0x0028` | 1 byte  |       `byte`       | Vehicle Acceleration comparison value.
`0x0029` | 1 byte  |       `byte`       | Vehicle Top Speed comparison value.
`0x002A` | 1 byte  |       `byte`       | Vehicle Handling comparison value.
`0x002B` | 1 byte  |       `byte`       | Vehicle Braking comparison value.
`0x002C` | 1 byte  |       `byte`       | Unknown value. Likely to have been an unused _"Overall"_ comparison value. Must be set to `0x05`.
`0x002E` | 2 bytes |      `ushort`      | Number of string entries in the string offset table. For Need For Speed III, this value is always `0x28`, indicating 40 strings.

> Note: An interesting nuance of the car serial number is that you would generally want it to be a small number, around 64 or less. It's been reported that large serial number values make the cars unavailable for AI opponents unless the player also selects that car.

### Offset table
Right after the FeData file header, there is a 160-byte table of offsets, where each one represents an absolute offset within the FeData file (headers and all included in the count) where a string may be read.

The strings in question are the front-end car information, like model, manufacturer, name of each color, etc.

Index | Field        | Notes
:---: | ------------ | ---
  0   | Manufacturer | Name of the car manufacturer (_Ferrari_, _Ford_, _Chevrolet_, etc.)
  1   | Model        | Model of the car (_F50_, _Taurus_, _Corvette_, etc.)
  2   | Car name     | Composite car name, would generally match the first two fields (_Ferrari F50_, _Ford Taurus_, _Chevrolet Corvette_, etc.)
  3   | Price        | Text that describes the price, localized to the speicfic FeData region or market where the car was sold in ("_$1,000,000_", "_€ 850,000_", etc.)
  4   | Status       | Describes if the car was a production model, a race car, or a prototype. You may put whatever value best describes that vehicle in a single word.
  5   | Weight       | Describes the gross weight of the car. Again, units are generally localized to the intended FeData region.
  6   | Weight Distribution | As the field name implies, this is a string describing the weight distribution of the car, normally in the form of "_40% front / 60% rear_" or similar.
  7   | Length       | Length of the vehicle localized to the intended FeData region.
  8   | Width        | Width of the vehicle localized to the intended FeData region.
  9   | Height       | Heigth of the vehicle localized to the intended FeData region.
  10  | Engine       | Usually, this is the model and type of engine in the vehicle, like "_Honda K-Series V6_" or "_Toyota 2JZ Inline-6_". The cyliner configuration may suffice as well ("_V6_", "_V8_", "_V10_", etc.)
  11  | Displacement | The engine displacement, localized to the intended FeData region.
  12  | HP           | Engine power output, localized to the intended FeData region.
  13  | Torque       | Engine torque output, localized to the intended FeData region.
  14  | Max engine speed | Describes the maximum RPM that the engine can achieve.
  15  | Brakes       | Describes the type and size of brakes in the car. Something like "_disk brakes (front) / drum brakes (rear)_" would work.
  16  | Tires        | Size of the tires, like "_305/55 R20 (front) / 315/60 R20 (rear)_" works here.
  17  | Top speed    | Car top speed, localized to the intended FeData region.
  18  | 0-to-60 MPH  | Acceleration up to 60 MPH (or, 96 KM/h) in seconds, localized to the intended FeData region.
  19  | 0-to-100 MPH | Acceleration up to 100 MPH (or, about 160 KM/h) in seconds, localized to the intended FeData region.
  20  | Transmission | Type of transmission, like "_Manual_" or "_Automatic_" (or both, like "_Manual/Automatic_")
  21  | Gearbox      | Number of speeds.
  22 to 29 | History X | Entry for history value, as shown in the car showcase
  30 to 39 | Color X | Entry for the name of a car color. Should describe the color present in the FCE model of the car in question at that index.
  
Each entry is null-terminated, and strings may share the same value in the string pool (for example, empty fields)

### Data pool
after the 160-byte string offsets table, there's the string data pool, where all strings are defined.

As stated before, several entries in the offset table may point to the same entry, like in the case of empty fields.
Strings are encoded using **Latin-1** (ISO-8859-1) encoding, which limits the character set to Western European languages. This is consistent with the localization targets of each file extension.

## Structures found in the file (Need For Speed IV)

The Need For Speed IV FeData format is fundamentally different from NFS III's format. It uses a much larger header structure with extensive padding, a single-byte magic identifier, and a completely rearranged field order.

Here's a general map of a typical NFS IV FeData file:

 Offset  | Length    | Description
-------- | --------- | ---
`0x0000` | ~924 bytes| [File header](#nfs-iv-file-header). Contains a single-byte magic, extensive padding, car properties, compare tables, and string entry count.
Varies   | ~1000+ bytes | [String offsets table](#nfs-iv-string-offsets-table). Variable-length based on string entry count.
Varies   | Varies    | [String data](#string-data). Pool that contains all the localized strings.

### NFS IV File header

Located at the very beginning of the file, the NFS IV header is significantly larger than the NFS III header. It includes a single-byte magic identifier, extensive padding blocks, car properties, compare tables, and localized string information.

- Struct size: Variable (approximately 924 bytes, depending on string count)

The header begins with:

 Offset  | Length  | Value type         | Description
-------- | ------- | :----------------: | ---
`0x0000` | 1 byte  | `byte`             | Magic identifier. Must be `0x04` for a valid NFS IV FeData file.

Following the magic byte, there's a 273-byte padding block (`0x111` bytes):

 Offset  | Length  | Value type         | Description
-------- | ------- | :----------------: | ---
`0x0001` | 273 bytes | `byte[]`         | Padding. Must be present but content is unspecified.

Then the 4-character car ID:

 Offset  | Length  | Value type         | Description
-------- | ------- | :----------------: | ---
`0x0112` | 4 bytes | `byte[]`           | 4-character car ID. Must be unique for each car.

Following the car ID, there's another padding block (`0x208` bytes), followed by car performance and configuration data:

 Offset  | Length  | Value type         | Description
-------- | ------- | :----------------: | ---
`0x031A` | 2 bytes | `ushort`           | Car serial number.
`0x031C` | 90 bytes | `byte[]`          | Padding.
`0x0376` | 1 byte  | `byte`             | Combined flags byte. Bits `0x01` indicate "Is Bonus". Bits `0x10` indicate police car (PursuitFlag). Special values: `0x20` (Mercedes non-police), `0xA0` (Ferrari non-police).
`0x0377` | 1 byte  | `byte`             | Combined flags byte. Bit `0x40` indicates "Upgradable" in career mode. Bits `0x03` indicate RoofFlag (`0x00`=Solid, `0x01`=Convertible, `0x02`=No roof). Bit `0x04` indicates DLC.
`0x0378` | 4 bytes | `byte[]`           | Unknown padding bytes (`0x378` through `0x37B`).
`0x037C` | 1 byte  | `byte`             | Unknown. Likely `0xB5` or `0x80`.
`0x037D` | 1 byte  | `byte`             | Unknown. Likely `0x01` or `0x00`.
`0x037E` | 1 byte  | `CarClass`         | Vehicle performance class (see [NFS IV Car Classes](#nfs-iv-car-classes)).
`0x037F` | 3 bytes | `byte[]`           | Unknown padding bytes.
`0x0382` | 1 byte  | `byte`             | Number of ushort values in compare tables. Always `0x0A` (10).
`0x0383` | 16 bytes | 4× CompareTableItem | Performance compare data (see [Compare Tables](#nfs-iv-compare-tables)).
`0x0393` | 1 byte  | `byte`             | Unknown.
`0x0394` | 4 bytes | `int`              | Default base price of the car.
`0x0398` | 4 bytes | `int`              | Price to upgrade to level 1.
`0x039C` | 4 bytes | `int`              | Price to upgrade to level 2.
`0x03A0` | 4 bytes | `int`              | Price to upgrade to level 3.
`0x03A4` | 13 bytes | `byte[]`          | Unknown data.
`0x03B1` | 1 byte  | `EngineLocation`   | Engine position (Front, Mid, or Rear).
`0x03B2` | 2 bytes | `ushort`           | Unknown padding.
`0x03B4` | 2 bytes | `ushort`           | Number of string entries in the string offset table.

> Note: The NFS IV header structure is much more complex than NFS III's. Many fields are packed into combined bytes with bit flags, and there's extensive padding throughout the header.

### NFS IV Compare Tables

NFS IV introduces a compare table system that allows cars to have performance stats at multiple upgrade levels. Each compare category (Acceleration, Top Speed, Handling, Braking, Overall) has four values: default, upgrade 1, upgrade 2, and upgrade 3.

Each **CompareTableItem** is 4 bytes (one byte per upgrade level):

 Offset (within struct) | Length  | Value type         | Description
----------------------- | ------- | :----------------: | ---
`0x0000` | 1 byte  | `byte`             | Default value (0-20 range).
`0x0001` | 1 byte  | `byte`             | Upgrade 1 value.
`0x0002` | 1 byte  | `byte`             | Upgrade 2 value.
`0x0003` | 1 byte  | `byte`             | Upgrade 3 value.

There are 5 CompareTableItem structs in total, consuming 20 bytes (at offset `0x0382` in the header):

| Index | Field        | Description |
|:-----:|--------------|-------------|
| 0     | Acceleration | Acceleration ratings at each upgrade level. |
| 1     | Top Speed    | Top speed ratings at each upgrade level. |
| 2     | Handling     | Handling ratings at each upgrade level. |
| 3     | Braking      | Braking ratings at each upgrade level. |
| 4     | Overall      | Overall performance ratings at each upgrade level. |

The total price for each upgrade level is stored separately:

- **Base Price** (`0x0394`): The default car price (no upgrades).
- **Upgrade 1 Price** (`0x0398`): Price to upgrade to level 1.
- **Upgrade 2 Price** (`0x039C`): Price to upgrade to level 2.
- **Upgrade 3 Price** (`0x03A0`): Price to upgrade to level 3.

> Note: Each performance value in the compare tables is a byte in the range 0-20, similar to the comparison values in the NFS III header.

### NFS IV String Offsets Table

The string offsets table in NFS IV follows the same mechanism as NFS III — a series of 4-byte absolute offsets into the string data pool. However, the offset table appears after the large header block (whose exact size depends on the `StringEntries` field), and the string pool begins immediately after it.

The number of string entries is the same between NFS III and NFS IV: **40 strings** (indices 0-39), with the same field assignments as described in the [String Offsets Table](#offset-table) section above.

### NFS IV Car Classes

NFS IV expands the car class system from 3 classes (A, B, C) to 4 classes with a more granular hierarchy:

| Value | Class | Description |
|:-----:|-------|-------------|
| `0x0` | AAA   | Top-tier performance class. |
| `0x1` | AA    | High performance class. |
| `0x2` | A     | Above-average performance class. |
| `0x3` | B     | Average performance class. |

> Note: Unlike NFS III, NFS IV does not include a "C" class. Classes are stored as a single byte (`CarClass` enum) rather than a ushort.

### NFS IV Pursuit Flag

The police car designation in NFS IV uses a bitmask system with special values for certain manufacturers:

| Value | Name | Description |
|:-----:|------|-------------|
| `0x00` | No | The car is not a police car. |
| `0x10` | Yes | The car is a police car. |
| `0x20` | No (Mercedes) | Special flag for Mercedes-Benz vehicles. |
| `0xA0` | No (Ferrari) | Special flag for Ferrari vehicles. |

The pursuit flag is combined with the "Is Bonus" bit in a single byte at offset `0x0376`. The police car bits are masked with `0xF0`, while the "Is Bonus" bit is masked with `0x01`.

### NFS IV Roof Flag

NFS IV includes a roof type flag that indicates the car's body configuration:

| Value | Name | Description |
|:-----:|------|-------------|
| `0x00` | Solid Roof | The car has a solid, non-removable roof. |
| `0x01` | Convertible | The car is a convertible. |
| `0x02` | No Roof | The car has no roof. |

This flag is combined with the "Upgradable" and "Is DLC" flags in a single byte at offset `0x0377`:
- Bits `0x03`: Roof type
- Bit `0x04`: Is DLC
- Bit `0x40`: Upgradable in career mode

## Comparison: NFS III vs NFS IV FeData

| Aspect | NFS III | NFS IV |
|--------|---------|--------|
| Header size | 47 bytes | ~924 bytes (variable) |
| Magic | None (implicit) | Single byte `0x04` |
| Car classes | A, B, C (3 classes) | AAA, AA, A, B (4 classes) |
| Compare data | 4 single bytes (Accel, TopSpeed, Handling, Braking) | 5 categories × 4 upgrade levels (20 bytes) + 4 price fields |
| Engine location | Not stored | Enum (Front, Mid, Rear) |
| Roof type | Not stored | Enum (Solid, Convertible, No roof) |
| Police flag | Boolean byte | Bitmask with manufacturer variants |
| Seat position | Stored (Left, Right, Center) | Not stored |
| Available to AI | Boolean flag | Not stored |
| String pool offset | Fixed at `0x00CF` | Variable (after large header) |
| String encoding | Latin-1 | Latin-1 |
| String count | Always 40 | Always 40 |

## String Data Format

Both NFS III and NFS IV use the same string data format:

- Strings are **null-terminated** (the null byte is not included in the offset calculation).
- Strings are encoded in **Latin-1** (ISO-8859-1).
- Multiple offset table entries may point to the same string in the pool (string deduplication).
- Empty strings are represented by a single null byte.
- The offset is **absolute** — it counts from the very beginning of the file, including all header data.

## File Size Calculation

The total file size of a FeData file can be calculated from its directory entries using the following formula:

```
FileSize = HeaderSize + (StringEntries × 4) + Σ(StringDataLength + 1)
```

Where:
- `HeaderSize` is 47 for NFS III, approximately 924+ for NFS IV (depends on `StringEntries`).
- `StringEntries × 4` is the size of the string offset table (4 bytes per entry).
- `StringDataLength + 1` is the length of each string in the pool plus one for the null terminator.

For NFS III specifically:

```
FileSize = 0x00CF + Σ(StringDataLength + 1)
```

Where `0x00CF` is the fixed start of the string pool (47-byte header + 160-byte offset table).