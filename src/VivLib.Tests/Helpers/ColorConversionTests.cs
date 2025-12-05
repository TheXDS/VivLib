namespace TheXDS.Vivianne.Helpers;

[TestFixture]
internal class ColorConversionTests
{
    [TestCase(0, 255, 0, 0)]
    [TestCase(21, 255, 126, 0)]
    [TestCase(43, 252, 255, 0)]
    [TestCase(64, 126, 255, 0)]
    [TestCase(85, 0, 255, 0)]
    [TestCase(106, 0, 255, 126)]
    [TestCase(127, 0, 255, 252)]
    [TestCase(149, 0, 126, 255)]
    [TestCase(170, 0, 0, 255)]
    [TestCase(191, 126, 0, 255)]
    [TestCase(213, 255, 0, 252)]
    [TestCase(234, 255, 0, 126)]
    public void ToRgb_ConvertsHueCorrectly(byte h, int r, int g, int b)
    {
        var (R, G, B) = ColorConversion.ToRgb(h, 255, 255);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(R, Is.EqualTo(r));
            Assert.That(G, Is.EqualTo(g));
            Assert.That(B, Is.EqualTo(b));
        }
    }

    [TestCase(0, 255)]
    [TestCase(0, 128)]
    [TestCase(0, 64)]
    [TestCase(0, 0)]
    [TestCase(43, 255)]
    [TestCase(43, 128)]
    [TestCase(43, 64)]
    [TestCase(43, 0)]
    [TestCase(85, 255)]
    [TestCase(85, 128)]
    [TestCase(85, 64)]
    [TestCase(85, 0)]
    [TestCase(127, 255)]
    [TestCase(127, 128)]
    [TestCase(127, 64)]
    [TestCase(127, 0)]
    [TestCase(170, 255)]
    [TestCase(170, 128)]
    [TestCase(170, 64)]
    [TestCase(170, 0)]
    [TestCase(213, 255)]
    [TestCase(213, 128)]
    [TestCase(213, 64)]
    [TestCase(213, 0)]
    [TestCase(255, 255)]
    [TestCase(255, 128)]
    [TestCase(255, 64)]
    [TestCase(255, 0)]
    public void ToRgb_ZeroSaturation(byte hue, byte brightness)
    {
        var (R, G, B) = ColorConversion.ToRgb(hue, 0, brightness);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(R, Is.EqualTo(brightness));
            Assert.That(G, Is.EqualTo(brightness));
            Assert.That(B, Is.EqualTo(brightness));
        }
    }

    [TestCase(0, 255)]
    [TestCase(0, 128)]
    [TestCase(0, 64)]
    [TestCase(0, 0)]
    [TestCase(43, 255)]
    [TestCase(43, 128)]
    [TestCase(43, 64)]
    [TestCase(43, 0)]
    [TestCase(85, 255)]
    [TestCase(85, 128)]
    [TestCase(85, 64)]
    [TestCase(85, 0)]
    [TestCase(127, 255)]
    [TestCase(127, 128)]
    [TestCase(127, 64)]
    [TestCase(127, 0)]
    [TestCase(170, 255)]
    [TestCase(170, 128)]
    [TestCase(170, 64)]
    [TestCase(170, 0)]
    [TestCase(213, 255)]
    [TestCase(213, 128)]
    [TestCase(213, 64)]
    [TestCase(213, 0)]
    [TestCase(255, 255)]
    [TestCase(255, 128)]
    [TestCase(255, 64)]
    [TestCase(255, 0)]
    public void ToRgb_ZeroBrightness(byte hue, byte saturation)
    {
        var (R, G, B) = ColorConversion.ToRgb(hue, saturation, 0);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(R, Is.Zero);
            Assert.That(G, Is.Zero);
            Assert.That(B, Is.Zero);
        }
    }
}
