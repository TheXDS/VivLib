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

Also, while Both Need For Speed III and Need For Speed IV cars include FeData files, their internal header structure is vastly different from each other. 

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