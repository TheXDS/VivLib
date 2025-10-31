using Moq;

namespace TheXDS.Vivianne.Codecs.Audio;

internal class IAudioCodec_Tests
{
    private readonly IAudioCodec codec = new Mock<IAudioCodec>() { CallBase = true }.Object;
    
    [Test]
    public void Interface_Encode_throws_NotImplementedException()
    {
        Assert.That(() => codec.Encode([], new Serializers.Audio.PtHeader()), Throws.InstanceOf<NotImplementedException>());
    }
}