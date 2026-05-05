using Moq;

namespace TheXDS.Vivianne.Codecs.Textures;

internal class IImageCodec_Tests
{
    [Test]
    public void Encode_when_not_implemented_throws_NotImplementedException()
    {
        Mock<IImageCodec> mock = new() { CallBase = true };
        Assert.That((Func<byte[]>)(() => mock.Object.Encode([], 0, 0)), Throws.InstanceOf<NotImplementedException>());
    }
}
