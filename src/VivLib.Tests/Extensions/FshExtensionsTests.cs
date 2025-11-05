using SixLabors.ImageSharp;
using TheXDS.Vivianne.Models.Fsh;
using St = TheXDS.Vivianne.Resources.Strings.Common;

namespace TheXDS.Vivianne.Extensions;

internal class FshExtensionsTests
{
    [TestCase(FshBlobFormat.Palette32, 1024)]
    [TestCase(FshBlobFormat.Palette24, 768)]
    [TestCase(FshBlobFormat.Palette24Dos, 768)]
    [TestCase(FshBlobFormat.Palette16, 512)]
    [TestCase(FshBlobFormat.Palette16Nfs5, 512)]
    public void GetPalette_searches_palette_FshBlob(FshBlobFormat format, int rawSize)
    {
        var paletteBlob = new FshBlob()
        {
            Magic = format,
            Width = 256,
            Height = 1,
            PixelData = new byte[rawSize]
        };
        var fsh = new FshFile()
        {
            Entries =
            {
                {"!pal", paletteBlob}
            }
        };
        var palette = fsh.GetPalette();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(palette, Is.Not.Null);
            Assert.That(palette, Is.InstanceOf<Color[]>());
            Assert.That(palette, Has.Length.EqualTo(256));
        }
    }

    [Test]
    public void GetPalette_returns_null_when_no_palette_found()
    {
        var fsh = new FshFile()
        {
            Entries =
            {
                {"0000", new FshBlob() { Magic = FshBlobFormat.Rgb24 } }
            }
        };
        var palette = fsh.GetPalette();
        Assert.That(palette, Is.Null);
    }

    [Test]
    public void IsGimxIdInvalid_detects_invalid_ids()
    {
        var invalidIds = new[]
        {
            (id: null, error: St.FshBlobEmptyId),
            (id: "", error: St.FshBlobEmptyId),
            (id: "ab", error: St.FshBlobIdTooLong),
            (id: "thisidiswaytoolongtobeavalidgimxid", error: St.FshBlobIdTooLong),
            (id: "invalid id!", error: St.FshBlobBadId),
        };

        foreach (var (id, error) in invalidIds)
        {
            var isInvalid = FshExtensions.IsGimxIdInvalid(id, out var errorMessage);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(isInvalid, Is.True, $"ID '{id}' should be invalid.");
                Assert.That(errorMessage, Is.EqualTo(error), $"ID '{id}' produced unexpected error message.");
            }
        }
    }

    [TestCase("abcd")]
    [TestCase("1234")]
    [TestCase("a1b2")]
    [TestCase("Z9Y8")]
    [TestCase("wXyZ")]
    public void IsGimxIdInvalid_accepts_valid_ids(string id)
    {
        var isInvalid = FshExtensions.IsGimxIdInvalid(id, out var errorMessage);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(isInvalid, Is.False, $"ID '{id}' should be valid.");
            Assert.That(errorMessage, Is.Null, $"ID '{id}' should not produce an error message.");
        }
    }

    [TestCase("TEST", true)]
    [TestCase("AAAA", false)]
    public void IsNewGimxIdInvalid_detects_existing_ids(string testId, bool shouldBeInvalid)
    {
        var fsh = new FshFile()
        {
            Entries =
            {
                {"TEST", new FshBlob() },
            }
        };

        var isInvalid = FshExtensions.IsNewGimxIdInvalid(fsh, testId, out var errorMessage);
        using (Assert.EnterMultipleScope())
        {
            if (shouldBeInvalid)
            {
                Assert.That(isInvalid, Is.True, $"ID '{testId}' should be invalid as it already exists.");
                Assert.That(errorMessage, Is.EqualTo(string.Format(St.FshBlobNewIdInUse, testId)), $"ID '{testId}' produced unexpected error message.");
            }
            else
            {
                Assert.That(isInvalid, Is.False, $"ID '{testId}' should be valid as it does not exist.");
                Assert.That(errorMessage, Is.Null, $"ID '{testId}' should not produce an error message.");
            }
        }
    }
}