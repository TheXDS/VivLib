namespace TheXDS.Vivianne.Helpers;

[TestFixture]
internal class ColorConversionTests
{
    [TestCase(0, 255, 255, 255, 0, 0)]
    [TestCase(85, 255, 255, 0, 255, 0)]
    [TestCase(170, 255, 255, 0, 0, 255)]
    public void ToRgb_ConvertsHueCorrectly(byte h, byte s, byte br, int r, int g, int b)
    {
        var (R, G, B) = ColorConversion.ToRgb(h, s, br);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(R, Is.EqualTo(r));
            Assert.That(G, Is.EqualTo(g));
            Assert.That(B, Is.EqualTo(b));
        }
    }

    [TestCase(0)]
    [TestCase(90)]
    [TestCase(180)]
    [TestCase(255)]
    public void ToRgb_ZeroSaturation(byte hue)
    {
        var (R, G, B) = ColorConversion.ToRgb(hue, 0, 255);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(R, Is.EqualTo(255));
            Assert.That(G, Is.EqualTo(255));
            Assert.That(B, Is.EqualTo(255));
        }
    }

    [TestCase(0, 255)]
    [TestCase(0, 128)]
    [TestCase(0, 0)]
    [TestCase(85, 255)]
    [TestCase(85, 128)]
    [TestCase(85, 0)]
    [TestCase(170, 255)]
    [TestCase(170, 128)]
    [TestCase(170, 0)]
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
