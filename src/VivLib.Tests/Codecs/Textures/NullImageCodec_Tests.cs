namespace TheXDS.Vivianne.Codecs.Textures;

internal class NullImageCodec_Tests
{
    [Test]
    public void Encode_returns_same_array()
    {
        NullImageCodec codec = new();
        byte[] data = [1, 2, 3, 4, 5];
        byte[] result = codec.Encode(data, 1, 1);
        Assert.That(result, Is.EquivalentTo(data));
    }
    
    [Test]
    public void Decode_returns_same_array()
    {
        NullImageCodec codec = new();
        byte[] data = [1, 2, 3, 4, 5];
        byte[] result = codec.Decode(data, 1, 1);
        Assert.That(result, Is.EquivalentTo(data));
    }
}