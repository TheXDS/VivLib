# Unit Tests Instructions

## Framework & Structure

- **Test framework:** NUnit 4.x (`[Test]`, `[TestCaseSource]`, `[SetUp]`, `[TearDown]`)
- **Assertions:** `Assert.That(...)`, `Throws.InstanceOf<T>()`
- **Mocks:** Moq for interface-based dependencies
- **Test project:** `src/VivLib.Tests/VivLib.Tests.csproj`
- **Test classes are `internal`** — exposed to tests via `InternalsVisibleTo("VivLib.Tests")`
- **Global usings:** `NUnit.Framework` is imported globally in `GlobalUsings.cs`

## File Organization

- Tests mirror the source structure. Example:
  - `Serializers/FshSerializerTests.cs` tests `Serializers/FshSerializer.cs`
  - `Codecs/RefPackCodecTests.cs` tests `Codecs/RefPackCodec.cs`
  - `Tools/Fce/FceCleanupToolTests.cs` tests `Tools/Fce/FceCleanupTool.cs`

## Test Patterns

### Simple unit test (pure function)

```csharp
namespace TheXds.Vivianne.Codecs;

internal class RefPackCodecTests
{
    [Test]
    public void IsCompressed_returns_false_if_signature_is_missing()
    {
        Assert.That(RefPackCodec.IsCompressed([0, 1, 2, 3, 4]), Is.False);
    }

    [Test]
    public void IsCompressed_returns_true_if_signature_is_present()
    {
        Assert.That(RefPackCodec.IsCompressed([0x10, 0xFB]), Is.True);
    }
}
```

### Round-trip / integration test with embedded fixtures

```csharp
[TestCaseSource(nameof(GetTestCases))]
public void Codec_roundtrip_test(byte[] testData)
{
    var compressed = RefPackCodec.Compress(testData);
    var roundtrip = RefPackCodec.Decompress(compressed);
    Assert.That(roundtrip.SequenceEqual(testData), Is.True);
}

private static IEnumerable<byte[]> GetTestCases()
{
    yield return GetTestFsh();                        // Embedded resource
    yield return GetDeterministicRndArray(65536);     // Synthetic data
    yield return [.. Enumerable.Range(0, 65536).Select(i => (byte)i)];
}
```

### Testing with Moq

```csharp
[Test]
public void Tool_calls_serializer_deserialize()
{
    var mockSerializer = new Mock<ISerializer<MyModel>>();
    mockSerializer.Setup(s => s.Deserialize(It.IsAny<byte[]>()))
                  .Returns(new MyModel { /* ... */ });

    var tool = new MyTool(mockSerializer.Object);
    // ... act and assert
}
```

## Test Fixtures & Resources

- **Embedded test files** live in `VivLib.Tests/Resources/Files/<NfsVersion>/`
- Load embedded resources via `GetManifestResourceStream` using the project's namespace
- Organize fixtures by NFS version: `Nfs2/`, `Nfs3/`, `Nfs4/`
- Synthetic test data (deterministic arrays, edge cases) should be generated inline

## Naming Conventions

- **Test classes:** `<SUT>Tests` (e.g., `RefPackCodecTests`, `FshSerializerTests`)
- **Test methods:** `<MethodName>_<scenario>_<expected>` (e.g., `IsCompressed_returns_false_if_signature_missing`)
- **Private helpers:** `Get<TestName>()`, `Get<TestName>TestCase()`, etc.

## Running Tests

```bash
dotnet test                          # Run all tests (Debug)
dotnet test -c Release               # Run tests in Release
dotnet test --filter "FullyQualifiedName~RefPack"  # Run a subset
```

## Best Practices

1. **No compiler warnings** — tests must compile cleanly
2. **Tests should be deterministic** — avoid reliance on system state, time, or randomness
3. **Prefer embedded fixtures over file paths** — tests should work in CI
4. **Keep tests small and focused** — one assertion per concept, or one round-trip per test
5. **Use `TestCaseSource` for data-driven tests** rather than looping inside `[Test]` methods
6. **Cover edge cases** — empty inputs, nulls (where applicable), boundary values
7. **Integration tests should reflect real usage** — serializers should be tested read + write round-trips
