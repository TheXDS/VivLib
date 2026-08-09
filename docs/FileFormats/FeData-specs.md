# FeData file format specs
In this document, we'll explore the format used by Front-End data (FeData) files.

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

Also, while Both Need For Speed III and Need For Speed IV cars include FeData files, their internal header structure is vastly different from each other. 

## Structures found in the file (Need For Speed 3)

Inside a FeData, there's a number of structures that serve a specific purpose. Here's a general map of a typical FeData file:

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
`0x0018` | 2 bytes |      `ushort`      | Car serial number, Like the ID, this value must be unique for each car.
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
`0x002E` | 2 bytes |      `ushort`      | Number of string entries in the string offset table. For Need For Speed 3, this value is always `0x28`, indicating 40 strings.




