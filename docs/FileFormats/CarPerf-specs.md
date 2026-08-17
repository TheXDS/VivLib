# CarPerf file format specification

In this document, we'll explore the Car Performance (CarPerf) file format used by Need For Speed 2, Need For Speed 3 and Need For Speed 4.

> Note: While VivLib internally refers to this format as "Carp" (for consistency with the `Carp` namespace), the files themselves have a `.txt` (or, for NFS2, `.qda`) file extension. This document refers to them as CarPerf files to distinguish them from the internal terminology.

## Summary

The CarPerf file format stores car performance data for vehicles in the Need For Speed series. Each file contains engine specifications, gear ratios, tire dimensions, handling parameters, and various physics tuning values. The format evolved significantly between NFS2 (binary) and NFS3/NFS4 (human-readable text).

- **NFS2:** Binary format using fixed-point decimal encoding and `struct` marshalling
- **NFS3:** Plain text key-value format with integer keys in parentheses
- **NFS4:** Extends NFS3 text format with one additional property (`understeer gradient`)

All three variants share the same conceptual property set, though NFS2 has a subset of properties and uses different numeric encodings.

## NFS2 CarPerf Format

The NFS2 CarPerf file is a **binary** format stored as a sequence of fixed-size fields with no header. All primitive numeric values are **little-endian** unless otherwise specified.

### NFS2 Binary Data Structure

The NFS2 format uses a `StructLayout.Sequential` layout with `Pack = 2`. All `FixedPointDecimal32` values are 4-byte fixed-point numbers encoding a signed decimal with 16-bit fractional and 16-bit integral parts.

> **FixedPointDecimal32:** A 32-bit fixed-point format with the lower 16 bits representing the fractional part and the upper 16 bits representing the signed integral part. Implicit conversions to/from `float` and `double` are provided.

| Offset | Length | Value Type | Description |
|--------|--------|:----------:| --- |
| `0x0000` | 4 bytes | `FixedPointDecimal32` | Car mass in kilograms. |
| `0x0004` | 4 bytes | `Int32` | Number of gears (includes reverse and neutral). |
| `0x0008` | 4 bytes | `Int32` | Gear shift delay in game ticks. |
| `0x000C` | 32 bytes | `FixedPointDecimal32[8]` | Velocity to RPM ratio for each gear. |
| `0x0028` | 32 bytes | `FixedPointDecimal32[8]` | Gear efficiency multiplier for each gear. |
| `0x0048` | 164 bytes | `FixedPointDecimal32[41]` | Engine torque curve (41 samples in Nm at equal RPM intervals). |
| `0x00ECA` | 4 bytes | `Int32` | Engine maximum RPM (redline). |
| `0x00EE8` | 4 bytes | `FixedPointDecimal32` | Maximum velocity in meters per second. |
| `0x00EEC` | 4 bytes | `FixedPointDecimal32` | Front drive ratio (0.0 to 1.0, FWD=1.0, RWD=0.0, AWD=mid-range). |
| `0x00EF0` | 4 bytes | `FixedPointDecimal32` | Maximum braking deceleration. |
| `0x00EF4` | 4 bytes | `FixedPointDecimal32` | Front brake bias ratio (0.0 to 1.0). |
| `0x00EF8` | 8 bytes | `byte[8]` | Gas pedal increase curve (8 steps). |
| `0x0100` | 8 bytes | `byte[8]` | Gas pedal decrease curve (8 steps). |
| `0x0108` | 8 bytes | `byte[8]` | Brake pedal increase curve (8 steps). |
| `0x0110` | 8 bytes | `byte[8]` | Brake pedal decrease curve (8 steps). |
| `0x0118` | 4 bytes | `FixedPointDecimal32` | Wheel base (distance between front and rear tires). |
| `0x011C` | 4 bytes | `FixedPointDecimal32` | Front grip bias (0.0 to 1.0). |
| `0x0120` | 4 bytes | `FixedPointDecimal32` | Maximum steering acceleration. |
| `0x0124` | 4 bytes | `FixedPointDecimal32` | Turn-in ramp value. |
| `0x0128` | 4 bytes | `FixedPointDecimal32` | Turn-out ramp value. |
| `0x012C` | 4 bytes | `FixedPointDecimal32` | Lateral acceleration grip multiplier. |
| `0x0130` | 4 bytes | `FixedPointDecimal32` | Aerodynamic downforce multiplier. |
| `0x0134` | 4 bytes | `FixedPointDecimal32` | Gas off factor. |
| `0x0138` | 4 bytes | `FixedPointDecimal32` | G-force transfer factor. |
| `0x013C` | 4 bytes | `FixedPointDecimal32` | Slide multiplier. |
| `0x0140` | 4 bytes | `FixedPointDecimal32` | Spin velocity cap. |
| `0x0144` | 4 bytes | `FixedPointDecimal32` | Slide velocity cap. |
| `0x0148` | 4 bytes | `FixedPointDecimal32` | Slide assistance factor. |
| `0x014C` | 4 bytes | `FixedPointDecimal32` | Push factor. |
| `0x0150` | 4 bytes | `FixedPointDecimal32` | Low turn factor. |
| `0x0154` | 4 bytes | `FixedPointDecimal32` | High turn factor. |

Total NFS2 CarPerf binary size: **344 bytes** (`0x0158`).

> Note: NFS2 CarPerf files do **not** include serial number, car class, automatic transmission data, final gear ratios, ABS flag, power steering, minimum steering acceleration, turning circle radius, tire specifications, tire wear, or any of the tuning/AI curve properties present in NFS3/NFS4.

## NFS3 CarPerf Format

The NFS3 CarPerf file is a **plain text** format using key-value pairs. Each property is represented by two consecutive lines: the first line contains the property label with its key in parentheses, and the second line contains the value.

### NFS3 Key-Value Properties

| Key | Label | Value Type | Description |
|-----|-------|:----------:| --- |
| `0` | `Serial Number` | `Int32` | Car serial number (must match FeData). |
| `1` | `Car Classification` | `Int32` | Car class index (A=0, B=1, C=2, must also match FeData). |
| `2` | `mass [kg]` | `double` | Car mass in kilograms. |
| `3` | `number of gears (reverse + neutral + forward gears)` | `Int32` | Number of gears for manual transmission. |
| `4` | `gear shift delay (ticks)` | `Int32` | Gear shift delay in game ticks. |
| `5` | `shift blip in rpm (size {count})` | `double[]` | RPM blip values when downshifting. Array size matches gear count. |
| `6` | `brake blip in rpm (size {count})` | `double[]` | RPM blip values when braking. |
| `7` | `velocity to rpm ratio (size {count})` | `double[]` | Velocity-to-RPM ratio per gear (manual). Array size matches gear count. |
| `8` | `gear ratios (size {count})` | `double[]` | Gear ratios (manual). Array size matches gear count. |
| `9` | `gear efficiency (size {count})` | `double[]` | Gear efficiency multiplier per gear (manual). Array size matches gear count. |
| `10` | `torque curve (size {count}) in {rpmIncr} rpm increments` | `double[]` | Engine torque curve samples. Size matches gear count × multipliers. RPM increment = `(EngineMaxRpm / count).Clamped(256, EngineMaxRpm)`. |
| `11` | `final gear` | `double` | Final drive ratio (manual). |
| `12` | `engine minimum rpm` | `Int32` | Engine idle RPM. |
| `13` | `engine redline in rpm` | `Int32` | Engine maximum RPM. |
| `14` | `Maximum velocity of car [m/s]` | `double` | Maximum velocity in m/s. |
| `15` | `top speed cap [m/s]` | `double` | Top speed cap in m/s. |
| `16` | `front drive ratio` | `double` | Front drive ratio (FWD=1.0, RWD=0.0, AWD=mid-range). |
| `17` | `Uses Antilock Brake System` | `Int32` | ABS flag (0 = false, non-zero = true). |
| `18` | `Maximum braking deceleration` | `double` | Maximum braking deceleration. |
| `19` | `front bias brake ratio` | `double` | Front brake bias (0.0 to 1.0). |
| `20` | `gas increasing curve` | `double[]` | Gas pedal increase curve. |
| `21` | `gas decreasing curve` | `double[]` | Gas pedal decrease curve. |
| `22` | `brake increasing curve` | `double[]` | Brake pedal increase curve. |
| `23` | `brake decreasing curve` | `double[]` | Brake pedal decrease curve. |
| `24` | `wheel base` | `double` | Wheel base distance. |
| `25` | `front grip bias` | `double` | Front grip bias (0.0 to 1.0). |
| `26` | `power steering (boolean)` | `Int32` | Power steering flag (0 = false, non-zero = true). |
| `27` | `minimum steering acceleration` | `double` | Minimum steering acceleration. |
| `28` | `turn in ramp` | `double` | Turn-in ramp value. |
| `29` | `turn out ramp` | `double` | Turn-out ramp value. |
| `30` | `lateral acceleration grip multiplier` | `double` | Lateral acceleration grip multiplier. |
| `31` | `aerodynamic downforce multiplier` | `double` | Aerodynamic downforce multiplier. |
| `32` | `gas off factor` | `double` | Gas off factor. |
| `33` | `g transfer factor` | `double` | G-force transfer factor. |
| `34` | `turning circle radius` | `double` | Turning circle radius. |
| `35` | `tire specs front` | `Int32[3]` | Front tire specs: `width,sidewall,rim`. |
| `36` | `tire specs rear` | `Int32[3]` | Rear tire specs: `width,sidewall,rim`. |
| `37` | `tire wear` | `double` | Tire wear factor. |
| `38` | `Slide Multiplier` | `double` | Slide multiplier. |
| `39` | `Spin Velocity Cap` | `double` | Spin velocity cap. |
| `40` | `Slide Velocity Cap` | `double` | Slide velocity cap. |
| `41` | `Slide Assistance Factor` | `double` | Slide assistance factor. |
| `42` | `Push Factor` | `double` | Push factor. |
| `43` | `Low Turn Factor (the lower the figure, the better the turn)` | `double` | Low turn factor. |
| `44` | `High Turn Factor (the lower the figure, the better the turn)` | `double` | High turn factor. |
| `45` | `pitch roll factor` | `double` | Pitch roll factor. |
| `46` | `road bumpiness factor` | `double` | Road bumpiness factor. |
| `47` | `spoiler function type` | `Int32` | Spoiler function type. |
| `48` | `spoiler activation speed [m/s]` | `double` | Spoiler activation speed. |
| `49` | `gradual turn cutoff` | `double` | Gradual turn cutoff threshold. |
| `50` | `medium turn cutoff` | `double` | Medium turn cutoff threshold. |
| `51` | `sharp turn cutoff` | `double` | Sharp turn cutoff threshold. |
| `52` | `medium turn speed modifier` | `double` | Medium turn speed modifier. |
| `53` | `sharp turn speed modifier` | `double` | Sharp turn speed modifier. |
| `54` | `extreme turn speed modifier` | `double` | Extreme turn speed modifier. |
| `55` | `subdivide level` | `double` | 3D model subdivision level. |
| `56` | `camera arm` | `double` | Camera arm length. |
| `57` | `Body Damage` | `double` | Body damage factor. |
| `58` | `Engine Damage` | `double` | Engine damage factor. |
| `59` | `Suspension Damage` | `double` | Suspension damage factor. |
| `60` | `Engine Tuning` | `double` | Engine tuning factor. |
| `61` | `Brake Balance` | `double` | Brake balance factor. |
| `62` | `Steering TopSpeed` | `double` | Steering top speed. |
| `63` | `Gear Rat Factor` | `double` | Gear ratio tuning factor. |
| `64` | `Suspension Stiffness` | `double` | Suspension stiffness factor. |
| `65` | `Aero Factor` | `double` | Aerodynamic factor. |
| `66` | `Tire Factor` | `double` | Tire factor. |
| `67` | `AI ACC0 acceleration table section` | `double[]` | AI acceleration curve section 0. |
| `68` | `AI ACC1 acceleration table section` | `double[]` | AI acceleration curve section 1. |
| `69` | `AI ACC2 acceleration table section` | `double[]` | AI acceleration curve section 2. |
| `70` | `AI ACC3 acceleration table section` | `double[]` | AI acceleration curve section 3. |
| `71` | `AI ACC4 acceleration table section` | `double[]` | AI acceleration curve section 4. |
| `72` | `AI ACC5 acceleration table section` | `double[]` | AI acceleration curve section 5. |
| `73` | `AI ACC6 acceleration table section` | `double[]` | AI acceleration curve section 6. |
| `74` | `AI ACC7 acceleration table section` | `double[]` | AI acceleration curve section 7. |
| `75` | `number of gears (automatic, r, n, forward)` | `Int32` | Number of gears for automatic transmission. |
| `76` | `velocity to rpm ratio (size {count})` | `double[]` | Velocity-to-RPM ratio per gear (automatic). |
| `77` | `gear ratios automatic (size {count})` | `double[]` | Gear ratios (automatic). |
| `78` | `gear efficiency automatic (size {count})` | `double[]` | Gear efficiency multiplier per gear (automatic). |

> Note: NFS3 does **not** include an `understeer gradient` property. This property is unique to NFS4.

## NFS4 CarPerf Format

The NFS4 CarPerf format is **identical** to NFS3 except for the addition of one extra key-value pair at the end of the file.

### Additional Properties (NFS4 only)

| Key | Label | Value Type | Description |
|-----|-------|:----------:| --- |
| `80` | `understeer gradient` | `double` | Understeer gradient value for steering physics. |

### NFS4 Example Fragment

```
tire wear(37)
0.95
Slide Multiplier(38)
1.0
...
gear efficiency automatic(78)
0.95,0.88,0.82,0.78,0.74,0.71,0.68
understeer gradient(80)
0.85
```

## File Size

CarPerf files are **variable-length text files** with no fixed header or size. The total file size depends on:

- Number of properties written (keys present)
- Array sizes for key-value pairs containing collections (ShiftBlip, TorqueCurve, AI curves, etc.)
- Number formatting precision (InvariantCulture double representation)
- NFS version (NFS2 is fixed 344 bytes; NFS3/NFS4 varies)

> Note: The text format uses UTF-8/Latin-1 encoding. Each property occupies exactly two lines (label + value), separated by a newline. Arrays are serialized as comma-separated values on a single line.

## Parsing Rules

### NFS2 Binary Parsing

1. Read the file as a contiguous binary blob of **344 bytes**.
2. Interpret fields using the `StructLayout.Sequential, Pack = 2` memory layout.
3. Convert `FixedPointDecimal32` fields by splitting the 32 bits into lower 16 (fractional) and upper 16 (integral signed) parts.
4. Array fields have fixed sizes determined by their type declaration (e.g., `byte[8]` for gas/brake curves, `FixedPointDecimal32[41]` for torque).

### NFS3/NFS4 Text Parsing

1. Read the file line by line.
2. For each property, the first line contains a label with an integer key in parentheses, e.g., `mass [kg](2)`.
3. Extract the key by parsing the number between `(` and `)`.
4. The next line contains the property value as a string.
5. Values are parsed as follows:
   - **Int32:** `int.TryParse(value, CultureInfo.InvariantCulture)`
   - **double:** `double.TryParse(value, CultureInfo.InvariantCulture)`
   - **double[] / int[]:** Split by `,` and parse each element
   - **bool (Int32):** Non-zero = `true`, zero = `false`
6. Continue until end of stream.
7. For NFS4, after reading key `78`, check for key `80` (understeer gradient).

> Note: Property keys are **not guaranteed to be sequential**. Keys 0–54 appear first, then 67–74 (AI curves), then 75–78 (automatic transmission), and finally key 80 (NFS4 only). Keys 55–66 are tuning/physics properties.

## Architecture Notes

### Comparison to Other Formats

The CarPerf format represents a significant divergence between NFS2 and NFS3/NFS4:

| Aspect | NFS2 | NFS3 | NFS4 |
|--------|------|------|------|
| **Encoding** | Binary (fixed-point) | Text (decimal) | Text (decimal) |
| **Header** | None | None | None |
| **Size** | Fixed 344 bytes | Variable | Variable (slightly larger than NFS3) |
| **Property count** | ~25 | ~79 | ~80 |
| **Numeric type** | `FixedPointDecimal32` | `double` | `double` |
| **Arrays** | Fixed-size structs | Variable-size comma-separated | Variable-size comma-separated |
| **Unique properties** | None | None | `understeer gradient` (key 80) |

### VivLib Serialization Architecture

VivLib uses a common base class `CarpSerializerBase<TCarClass, TFile>` for NFS3/NFS4 text formats, which handles:
- Line-by-line parsing with key extraction from `(...)` labels
- Type-safe conversion via `TryIntKey`, `TryDoubleKey`, `TryIntArray`, `TryDoubleArray` helpers
- Array population for all multi-value properties (ShiftBlip, TorqueCurve, AI curves, etc.)
- Tire specs parsing from comma-separated triplets

NFS2 uses a separate `CarpSerializer : IMarshalSerializer<CarPerf, CarpData>` that:
- Marshals the binary blob to/from a `StructLayout`-annotated `CarpData` struct
- Performs implicit conversions between `FixedPointDecimal32` and `double`

The `CarPerf` model hierarchy is:
- `CarPerf` (NFS2) — flat class with all NFS2-specific properties
- `CarPerf<CarClass>` (base for NFS3/NFS4) — generic base implementing `ICarPerf` and `ICarClass<T>`
- `CarPerf : CarPerf<CarClass>` (NFS3) — uses `CarClass : ushort` enum
- `CarPerf : CarPerf<CarClass>` (NFS4) — extends with `UndersteerGradient`, uses `CarClass : byte` enum

## CarClass Enums

### NFS3 CarClass (`ushort`)

| Value | Class |
|-------|-------|
| `0` | A |
| `1` | B |
| `2` | C |

### NFS4 CarClass (`byte`)

| Value | Class |
|-------|-------|
| `0x0` | AAA |
| `0x1` | AA |
| `0x2` | A |
| `0x3` | B |

## Field Reference Summary

The following table summarizes all properties across all three NFS versions, indicating which version includes each property.

| Property | NFS2 | NFS3 | NFS4 |
|----------|:----:|:----:|:----:|
| SerialNumber | ✗ | ✓ | ✓ |
| CarClass | ✗ | ✓ | ✓ |
| Mass | ✓ | ✓ | ✓ |
| NumberOfGears | ✓ | ✓ (manual) | ✓ (manual) |
| NumberOfGearsAuto | ✗ | ✓ | ✓ |
| GearShiftDelay | ✓ | ✓ | ✓ |
| ShiftBlip | ✗ | ✓ | ✓ |
| BrakeBlip | ✗ | ✓ | ✓ |
| VelocityToRpmManual | ✓ | ✓ | ✓ |
| VelocityToRpmAuto | ✗ | ✓ | ✓ |
| GearRatioManual | ✓ | ✓ | ✓ |
| GearRatioAuto | ✗ | ✓ | ✓ |
| GearEfficiencyManual | ✓ | ✓ | ✓ |
| GearEfficiencyAuto | ✗ | ✓ | ✓ |
| TorqueCurve | ✓ | ✓ | ✓ |
| FinalGearManual | ✗ | ✓ | ✓ |
| FinalGearAuto | ✗ | ✓ | ✓ |
| EngineMinRpm | ✗ | ✓ | ✓ |
| EngineMaxRpm | ✓ | ✓ | ✓ |
| MaxVelocity | ✓ | ✓ | ✓ |
| TopSpeed | ✗ | ✓ | ✓ |
| FrontDriveRatio | ✓ | ✓ | ✓ |
| Abs | ✗ | ✓ | ✓ |
| MaxBrakeDecel | ✓ | ✓ | ✓ |
| FrontBrakeBias | ✓ | ✓ | ✓ |
| GasIncreaseCurve | ✓ | ✓ | ✓ |
| GasDecreaseCurve | ✓ | ✓ | ✓ |
| BrakeIncreaseCurve | ✓ | ✓ | ✓ |
| BrakeDecreaseCurve | ✓ | ✓ | ✓ |
| WheelBase | ✓ | ✓ | ✓ |
| FrontGripBias | ✓ | ✓ | ✓ |
| PowerSteering | ✗ | ✓ | ✓ |
| MinimumSteerAccel | ✗ | ✓ | ✓ |
| TurnInRamp | ✓ | ✓ | ✓ |
| TurnOutRamp | ✓ | ✓ | ✓ |
| LateralAccGripMult | ✓ | ✓ | ✓ |
| AeroDownMult | ✓ | ✓ | ✓ |
| GasOffFactor | ✓ | ✓ | ✓ |
| GTransferFactor | ✓ | ✓ | ✓ |
| TurnCircleRadius | ✗ | ✓ | ✓ |
| TireWidthFront | ✗ | ✓ | ✓ |
| TireSidewallFront | ✗ | ✓ | ✓ |
| TireRimFront | ✗ | ✓ | ✓ |
| TireWidthRear | ✗ | ✓ | ✓ |
| TireSidewallRear | ✗ | ✓ | ✓ |
| TireRimRear | ✗ | ✓ | ✓ |
| TireWear | ✗ | ✓ | ✓ |
| SlideMult | ✓ | ✓ | ✓ |
| SpinVelocityCap | ✓ | ✓ | ✓ |
| SlideVelocityCap | ✓ | ✓ | ✓ |
| SlideAssistanceFactor | ✓ | ✓ | ✓ |
| PushFactor | ✓ | ✓ | ✓ |
| LowTurnFactor | ✓ | ✓ | ✓ |
| HighTurnFactor | ✓ | ✓ | ✓ |
| PitchRollFactor | ✗ | ✓ | ✓ |
| RoadBumpFactor | ✗ | ✓ | ✓ |
| SpoilerFunctionType | ✗ | ✓ | ✓ |
| SpoilerActivationSpeed | ✗ | ✓ | ✓ |
| GradualTurnCutoff | ✗ | ✓ | ✓ |
| MediumTurnCutoff | ✗ | ✓ | ✓ |
| SharpTurnCutoff | ✗ | ✓ | ✓ |
| MediumTurnSpdMod | ✗ | ✓ | ✓ |
| SharpTurnSpdMod | ✗ | ✓ | ✓ |
| ExtremeTurnSpdMod | ✗ | ✓ | ✓ |
| SubdivideLevel | ✗ | ✓ | ✓ |
| CameraArm | ✗ | ✓ | ✓ |
| BodyDamage | ✗ | ✓ | ✓ |
| EngineDamage | ✗ | ✓ | ✓ |
| SuspensionDamage | ✗ | ✓ | ✓ |
| EngineTuning | ✗ | ✓ | ✓ |
| BrakeBalance | ✗ | ✓ | ✓ |
| SteeringSpeed | ✗ | ✓ | ✓ |
| GearRatFactor | ✗ | ✓ | ✓ |
| SuspensionStiffness | ✗ | ✓ | ✓ |
| AeroFactor | ✗ | ✓ | ✓ |
| TireFactor | ✗ | ✓ | ✓ |
| AiCurve0–AiCurve7 | ✗ | ✓ | ✓ |
| UndersteerGradient | ✗ | ✗ | ✓ |
