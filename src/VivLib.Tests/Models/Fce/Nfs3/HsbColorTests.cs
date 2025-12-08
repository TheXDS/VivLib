using TheXDS.Vivianne.Models.Fce.Common;

namespace TheXDS.Vivianne.Models.Fce.Nfs3;

internal class HsbColorTests
{
    [Test]
    public void Struct_exposes_Hsb_props()
    {
        HsbColor color = new(1, 2, 3, 4);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(color.Hue, Is.EqualTo(1));
            Assert.That(color.Saturation, Is.EqualTo(2));
            Assert.That(color.Brightness, Is.EqualTo(3));
            Assert.That(color.Alpha, Is.EqualTo(4));
        }
    }

    [Test]
    public void Struct_implements_IHsbColor()
    {
        IHsbColor color = new HsbColor(1, 2, 3, 4);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(color.Hue, Is.EqualTo((byte)1));
            Assert.That(color.Saturation, Is.EqualTo((byte)2));
            Assert.That(color.Brightness, Is.EqualTo((byte)3));
            Assert.That(color.Alpha, Is.EqualTo((byte)4));
        }
    }

    [TestCase(0, 255, 0, 0)]
    [TestCase(85, 0, 255, 0)]
    [TestCase(170, 0, 0, 255)]
    public void ToRgba_converts_Hsba_to_Rgba(int hue, int expectedR, int expectedG, int expectedB)
    {
        HsbColor color = new(hue, 255, 255, 127);
        var (resultR, resultG, resultB, resultA) = color.ToRgba();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(resultR, Is.EqualTo(expectedR));
            Assert.That(resultG, Is.EqualTo(expectedG));
            Assert.That(resultB, Is.EqualTo(expectedB));
            Assert.That(resultA, Is.EqualTo(127));
        }
    }

    [TestCase(0, 255, 0, 0)]
    [TestCase(85, 0, 255, 0)]
    [TestCase(170, 0, 0, 255)]
    public void ToRgb_converts_Hsb_to_Rgb(int hue, int expectedR, int expectedG, int expectedB)
    {
        HsbColor color = new(hue, 255, 255, 255);
        var (resultR, resultG, resultB) = color.ToRgb();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(resultR, Is.EqualTo(expectedR));
            Assert.That(resultG, Is.EqualTo(expectedG));
            Assert.That(resultB, Is.EqualTo(expectedB));
        }
    }
}