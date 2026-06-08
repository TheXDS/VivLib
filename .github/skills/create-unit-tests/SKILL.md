---
name: create-unit-tests
description: 'Create or extend unit tests in VivLib. Use when: writing new test files, adding test methods to existing tests, implementing round-trip serializer tests, testing codecs, adding Moq-based integration tests. Produces NUnit 4.x test classes matching project conventions.'
---

# Create Unit Tests

## Workflow

### 1. Locate the Source Under Test (SUT)

Identify the class, method, or module to test. Determine:
- **Which file** contains the SUT (check `src/VivLib/`)
- **What namespace** the SUT belongs to (usually `TheXDS.Vivianne.<Module>`)
- **What interfaces** it depends on (check if Moq mocks are needed)

### 2. Determine Test Location

Test files **mirror** the source structure under `src/VivLib.Tests/`:

| Source | Test File |
|--------|-----------|
| `src/VivLib/Codecs/RefPackCodec.cs` | `src/VivLib.Tests/Codecs/RefPackCodecTests.cs` |
| `src/VivLib/Serializers/FshSerializer.cs` | `src/VivLib.Tests/Serializers/FshSerializerTests.cs` |
| `src/VivLib/Tools/Fce/FceCleanupTool.cs` | `src/VivLib.Tests/Tools/Fce/FceCleanupToolTests.cs` |

If a test file already exists in the correct location, **add to it** rather than creating a new file.

### 3. Create the Test Class

```csharp
namespace TheXDS.Vivianne.<Module>;

internal class <SUTName>Tests
{
    // Test methods go here
}
```

**Rules:**
- Namespace must match the source namespace
- Class is `internal` (exposed via `InternalsVisibleTo`)
- Class name: `<SUTName>Tests` (e.g., `RefPackCodecTests`, `FshSerializerTests`)

### 4. Write Test Methods

Follow the pattern: `<MethodName>_<scenario>_<expected>`

```csharp
[Test]
public void IsCompressed_returns_false_if_signature_is_missing()
{
    Assert.That(RefPackCodec.IsCompressed([0, 1, 2, 3, 4]), Is.False);
}
```

### 5. Choose the Right Test Pattern

**A. Pure function / unit test** — no dependencies:
```csharp
[Test]
public void MethodName_returns_expected_result_for_valid_input()
{
    var result = Sut.Method(input);
    Assert.That(result, Is.EqualTo(expected));
}
```

**B. Round-trip / integration test** — read + write:
```csharp
[TestCaseSource(nameof(GetTestCases))]
public void Roundtrip_<Format>(byte[] testData)
{
    var obj = Serializer.Deserialize(testData);
    var output = Serializer.Serialize(obj);
    Assert.That(output.SequenceEqual(testData), Is.True);
}

private static IEnumerable<byte[]> GetTestCases()
{
    yield return GetTestFsh();          // Embedded resource
    yield return GetDeterministicRndArray(65536);
    yield return [.. Enumerable.Range(0, 256).Select(i => (byte)i)];
}
```

**C. Mock-based test** — interface dependency:
```csharp
[Test]
public void Tool_calls_serializer_deserialize()
{
    var mock = new Mock<IDependency>();
    mock.Setup(d => d.Operation(It.IsAny<byte[]>()))
        .Returns(new Result());

    var sut = new ClassUnderTest(mock.Object);
    // Act & assert
}
```

### 6. Add Test Fixtures (if needed)

For embedded resource files:
- Place `.resx` test files in `src/VivLib.Tests/Resources/Files/<NfsVersion>/`
- Load via `GetManifestResourceStream` in helper methods

### 7. Validate

- **No compiler warnings** in the test file
- **Tests compile** — run `dotnet build`
- **Tests pass** — run `dotnet test`
- **Deterministic** — no reliance on time, randomness, or system state

## Naming Conventions

| Element | Pattern | Example |
|---------|---------|---------|
| Test class | `<SUT>Tests` | `RefPackCodecTests` |
| Test method | `<Method>_<scenario>_<expected>` | `IsCompressed_returns_true_if_signature_present` |
| Helper method | `Get<Descriptor>()` | `GetTestFsh()`, `GetRoundTripTestCase()` |

## Common Pitfalls

1. **Wrong namespace** — must match the source namespace exactly
2. **Public test class** — must be `internal`
3. **Non-deterministic tests** — no `DateTime.Now`, `Random`, or file I/O without embedded resources
4. **Missing `TestCaseSource`** — use for data-driven tests, not loops inside `[Test]`
5. **Ignoring embedded resources** — use `Resources/Files/` for real binary fixtures
6. **Warnings** — zero compiler warnings, including in test files

## Running Tests

```bash
dotnet test                          # All tests
dotnet test --filter "FullyQualifiedName~RefPack"  # Subset
dotnet test -c Release               # Release mode
```
