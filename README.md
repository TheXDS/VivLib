<table>
<tr>
<td>
<img src="./Art/VivLib.svg">
</td>
<td>
A classic NFS modding library
</td>
</tr>
</table>

## Introduction
Originally part of the [Vivianne](https://github.com/TheXDS/Vivianne) project, *VivLib* is a lightweight library that provides essential tools and utilities for modding various classic Need For Speed titles. It aims to simplify the modding process by offering a set of reusable components and functions, especially around file (de)serialization, data manipulation, and common modding tasks.

This differs from the [OpenNFS Project](https://github.com/OpenNFS/LibOpenNFS) in that *VivLib* is written in a managed language, and its focus is in read-write support from the ground up to create modding tools, and not necessarily for high-performance data loading for a game engine.

## Features
*VivLib* offers read-write support for several file formats used in classic EA Titles; focused on Need For Speed, although some of these formats apply to other games.

### File (de)serialization
VivLib can read and write the following file formats:

File format | Read | Write | Notes
----------- | :-: | :-: | ---
`.VIV` | ✔️ | ✔️
`.FSH`, `.QFS` | ✔️ | ✔️ | Currently, only *RefPack* compression is supported
`.ASF` | ✔️ | ❌
`.LIN`, `.MAP`, `.MUS` | ✔️ | ❌
`.BNK` | ✔️ | ✔️[^1] [^2] | Some NFS2 BNKs fail to load properly

#### Need For Speed 2 Specific
File format | Read | Write
----------- | :-: | :-:
`.GEO` | ✔️ | ✔️[^1]
`.DAT` car perf | ✔️ | ✔️[^1]

#### Need For Speed 3 Specific
File format | Read | Write 
----------- | :-: | :-:
`.FCE` | ✔️ | ✔️
Front-End car data (`.BRI`, `.ENG`, etc.) | ✔️ | ✔️
`DASH.QFS`-specific structures | ✔️ | ✔️
`.TXT` car perf | ✔️ | ✔️

#### Need For Speed 4 Specific
File format | Read | Write 
----------- | :-: | :-:
`.FCE` | ✔️ | ✔️
Front-End car data (`.BRI`, `.ENG`, etc.) | ✔️ | ✔️
`.TXT` car perf | ✔️ | ✔️

[^1]: Write support is experimental
[^2]: Partial support for the file's features

### File-specific features
Below you can find some file-specific highlights into features supported by *VivLib*. Other supported file formats not mentioned in this list can, in general, be sumarized as supporting read and modify all currently known data present in the file.

#### FSH/QFS
- All uncompressed, file compressed and blob compressed FSH/QFS support (Currently, only *RefPack* compression is supported)
- Editing of texture coords data (non-NFS games will interpret this data differently, but it can still be edited)
- Support for post-header arbitraty data
- Change directory ID (`GIMX` is the default for FSH files as used in Need For Speed games)
- Add(Import)/Replace/Export/Remove textures
- Rename blob
- FSH blob footer data import/export
- *NFS3*s `DASH.QFS` dashboard data edit
- Read global color palette from shared palette FSH Blob
- Read local color palette from FSH Blob footer.
- Write local color palette at import
- Several image formats supported (8-bit with palette, 16, 24, and 32 bit formats with and without alpha and RefPack variants, DXT1/DXT5)

#### FCE
- Edit all known FCE properties
- Edit color tables
- Rename parts/dummies
- FCE center
- Generate damaged model for NFS4
- FCE3/FCE4/FCE4m conversion with part auto-renaming
- FCE cleanup tooling[^3]

#### BNK
- Read PCM/EA ADPCM data
- Write PCM data
- Edit loop data
- Direct editing of PT Headers in general and audio tables
- Alt-stream support
- Render to `.WAV`
- Render looping section to `.WAV`
- BNK post-header/post-stream arbitrary data support

[^3]: This feature is a *Work In Progress* that is currently under development.

## Building VivLib
To compile VivLib, the [SDK for .NET 8.0](https://dotnet.microsoft.com/) or a later version with a targeting pack for .NET 8.0 must be installed on the system.

### Compiling VivLib
``` sh
dotnet build ./VivLib.slnx
```
The binaries will be found in the `Build/bin` folder at the root of the repository.

### Executing tests
``` sh
dotnet test ./VivLib.slnx
```
#### Coverage Report
It is possible to obtain a local code coverage report. To do this, it is necessary to install [`ReportGenerator`](https://github.com/danielpalme/ReportGenerator), which will read the results of the test execution and generate a web page with the coverage results.

To install `ReportGenerator`, run:
``` sh
dotnet tool install -g dotnet-reportgenerator-globaltool
```
After installing `ReportGenerator`, it will be possible to run the following command:
``` sh
dotnet test ./VivLib.slnx --collect:"XPlat Code Coverage" --results-directory:./Build/Tests ; reportgenerator.exe -reports:./Build/Tests/*/coverage.cobertura.xml -targetdir:./Build/Coverage/
```
The coverage results will be stored in `./Build/Coverage`

## Contribute

### Code contributions
Please ferer to [CONTRIBUTING.md](./docs/CONTRIBUTING.md) for more information.

### Donations
[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/W7W415UCHY)

If VivLib has been useful to you, or if you are interested in donating to support the development of the project, feel free to make a donation via [PayPal](https://paypal.me/thexds), [Ko-fi](https://ko-fi.com/W7W415UCHY), or contact me directly.

Unfortunately, I cannot offer other means of donation at the moment because my country (Honduras) is not supported by any platform (everybody avoids us like the plague).
