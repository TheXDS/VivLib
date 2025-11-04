using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using TheXDS.Vivianne.Models.Fsh;

namespace TheXDS.Vivianne.Extensions;

internal class FshBlobExtensions
{
    [Test]
    public void ToImage_returns_null_for_unsupported_FshBlob_format()
    {
        FshBlob blob = new() { Magic = 0 };
        Assert.That(blob.ToImage(), Is.Null);
    }

    [Test]
    public void ToImage_Index8_returns_32bit_image()
    {
        FshBlob blob = new()
        {
            Magic = FshBlobFormat.Indexed8,
            Height = 2,
            Width = 2,
            PixelData = [ 0x00, 0x01, 0x02, 0x03 ],
            Footer = ((Color[]) [
                Color.FromRgba(0x00, 0x11, 0x22, 0x33),
                Color.FromRgba(0x44, 0x55, 0x66, 0x77),
                Color.FromRgba(0x88, 0x99, 0xaa, 0xbb),
                Color.FromRgba(0xcc, 0xdd, 0xee, 0xff),
            ]).ToRawFooter()
        };
        var result = blob.ToImage();
        Assert.That(result, Is.InstanceOf<Image<Bgra32>>());
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Width, Is.EqualTo(2));
            Assert.That(result.Height, Is.EqualTo(2));
            // Ordering once loaded is... ARGB ? 
            Assert.That(((Image<Bgra32>)result)[0, 0].Bgra, Is.EqualTo((uint)0x33001122));
            Assert.That(((Image<Bgra32>)result)[1, 0].Bgra, Is.EqualTo((uint)0x77445566));
            Assert.That(((Image<Bgra32>)result)[0, 1].Bgra, Is.EqualTo((uint)0xbb8899aa));
            Assert.That(((Image<Bgra32>)result)[1, 1].Bgra, Is.EqualTo((uint)0xffccddee));
        }
    }

    [Test]
    public void ToImage_Index8_returns_24bit_image()
    {
        FshBlob blob = new()
        {
            Magic = FshBlobFormat.Indexed8,
            Height = 2,
            Width = 2,
            PixelData = [0x00, 0x01, 0x02, 0x03],
            Footer = ((Color[])[
                Color.FromRgb(0x00, 0x11, 0x22),
                Color.FromRgb(0x44, 0x55, 0x66),
                Color.FromRgb(0x88, 0x99, 0xaa),
                Color.FromRgb(0xcc, 0xdd, 0xee),
            ]).ToRawFooter(FshBlobFormat.Palette24)
        };
        var result = blob.ToImage();
        Assert.That(result, Is.InstanceOf<Image<Bgra32>>());
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Width, Is.EqualTo(2));
            Assert.That(result.Height, Is.EqualTo(2));
            Assert.That(((Image<Bgra32>)result)[0, 0].Bgra, Is.EqualTo(0xff001122));
            Assert.That(((Image<Bgra32>)result)[1, 0].Bgra, Is.EqualTo(0xff445566));
            Assert.That(((Image<Bgra32>)result)[0, 1].Bgra, Is.EqualTo(0xff8899aa));
            Assert.That(((Image<Bgra32>)result)[1, 1].Bgra, Is.EqualTo(0xffccddee));
        }
    }

    [Test]
    public void ToImage_Index8_returns_24bit_image_2()
    {
        FshBlob blob = new()
        {
            Magic = FshBlobFormat.Indexed8,
            Height = 2,
            Width = 2,
            PixelData = [0x00, 0x01, 0x02, 0x03],
            Footer = ((Color[])[
                Color.FromRgb(0x00, 0x11, 0x22),
                Color.FromRgb(0x44, 0x55, 0x66),
                Color.FromRgb(0x88, 0x99, 0xaa),
                Color.FromRgb(0xcc, 0xdd, 0xee),
            ]).ToRawFooter(FshBlobFormat.Palette24Dos)
        };
        var result = blob.ToImage();
        Assert.That(result, Is.InstanceOf<Image<Bgra32>>());
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Width, Is.EqualTo(2));
            Assert.That(result.Height, Is.EqualTo(2));
            Assert.That(((Image<Bgra32>)result)[0, 0].Bgra, Is.EqualTo(0xff001122));
            Assert.That(((Image<Bgra32>)result)[1, 0].Bgra, Is.EqualTo(0xff445566));
            Assert.That(((Image<Bgra32>)result)[0, 1].Bgra, Is.EqualTo(0xff8899aa));
            Assert.That(((Image<Bgra32>)result)[1, 1].Bgra, Is.EqualTo(0xffccddee));
        }
    }

    [Test]
    public void ToImage_Argb32_returns_32bit_image()
    {
        FshBlob blob = new()
        {
            Magic = FshBlobFormat.Argb32,
            Height = 2,
            Width = 2,
            PixelData = [
                0x12, 0x34, 0x56, 0x78, 0x9a, 0xbc, 0xde, 0xf0,
                0x0f, 0xed, 0xcb, 0xa9, 0x87, 0x65, 0x43, 0x21
            ]
        };
        var result = blob.ToImage();
        Assert.That(result, Is.InstanceOf<Image<Bgra32>>());
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Width, Is.EqualTo(2));
            Assert.That(result.Height, Is.EqualTo(2));
            Assert.That(((Image<Bgra32>)result)[0, 0].Bgra, Is.EqualTo((uint)0x78563412));
            Assert.That(((Image<Bgra32>)result)[1, 0].Bgra, Is.EqualTo(0xf0debc9a));
            Assert.That(((Image<Bgra32>)result)[0, 1].Bgra, Is.EqualTo(0xa9cbed0f));
            Assert.That(((Image<Bgra32>)result)[1, 1].Bgra, Is.EqualTo((uint)0x21436587));
        }
    }

    [Test]
    public void ToImage_Rgb24_returns_24bit_image()
    {
        FshBlob blob = new()
        {
            Magic = FshBlobFormat.Rgb24,
            Height = 2,
            Width = 2,
            PixelData = [
                0x12, 0x34, 0x56, 0x78, 0x9a, 0xbc,
                0x0f, 0xed, 0xcb, 0xa9, 0x87, 0x65,
            ]
        };
        var result = blob.ToImage();
        Assert.That(result, Is.InstanceOf<Image<Bgr24>>());
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Width, Is.EqualTo(2));
            Assert.That(result.Height, Is.EqualTo(2));
            Assert.That(((Image<Bgr24>)result)[0, 0].B, Is.EqualTo(0x12));
            Assert.That(((Image<Bgr24>)result)[0, 0].G, Is.EqualTo(0x34));
            Assert.That(((Image<Bgr24>)result)[0, 0].R, Is.EqualTo(0x56));
            Assert.That(((Image<Bgr24>)result)[1, 0].B, Is.EqualTo(0x78));
            Assert.That(((Image<Bgr24>)result)[1, 0].G, Is.EqualTo(0x9a));
            Assert.That(((Image<Bgr24>)result)[1, 0].R, Is.EqualTo(0xbc));
            Assert.That(((Image<Bgr24>)result)[0, 1].B, Is.EqualTo(0x0f));
            Assert.That(((Image<Bgr24>)result)[0, 1].G, Is.EqualTo(0xed));
            Assert.That(((Image<Bgr24>)result)[0, 1].R, Is.EqualTo(0xcb));
            Assert.That(((Image<Bgr24>)result)[1, 1].B, Is.EqualTo(0xa9));
            Assert.That(((Image<Bgr24>)result)[1, 1].G, Is.EqualTo(0x87));
            Assert.That(((Image<Bgr24>)result)[1, 1].R, Is.EqualTo(0x65));
        }
    }
}
