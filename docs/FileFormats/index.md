# File Format Specifications

This section documents the binary file formats used by Electronic Arts in Need For Speed 2, 3, and 4, as reverse-engineered and implemented in VivLib.

> This list is not exhaustive, and it is in expansion. Check later if you don't see a format described here.

## Formats

| Format | Description | Files |
|--------|-------------|-------|
| **VIV** | Archive container for textures, models, and audio assets | `VIV-specs.md` |
| **FCE** | 3D mesh data for vehicles | `FCE-specs.md` |
| **SHPI/FSH** | Texture collection format (`.fsh` / `.qfs`) | `SHPI-specs.md` |
| **CarPerf** | Car performance data | `CarPerf-specs.md` |
| **FeData** | Localized front-end text | `FeData-specs.md` |
| **NFS 3 Unofficial** | Legacy notes from reverse-engineering (`.txt`) | `unofficial_nfs3_file_specs_10.txt` |

## Version Differences

Most formats evolved between NFS 3 and NFS 4, with NFS 4 adding:

- Damage states (damaged/destructed vertices and normals)
- Interior and driver color palettes
- Window-specific material flags
- Extended header tables

NFS 2 support is currently read-only for a subset of formats.
