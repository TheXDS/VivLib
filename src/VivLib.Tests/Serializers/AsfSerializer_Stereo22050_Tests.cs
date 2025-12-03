#pragma warning disable CS1591

using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Audio.Mus;

namespace TheXDS.Vivianne.Serializers;

[TestFixture]
public class AsfSerializer_Stereo22050_Tests() : AsfSerializerTests("test_stereo22050.asf", GetDefaultFile())
{
    private static AsfFile GetDefaultFile() => new()
    {
        Channels = 2,
        Compression = CompressionMethod.None,
        SampleRate = 22050,
        BytesPerSample = 2,
        AudioBlocks =
        {
            new byte[]
            {
                0x00, 0x00, 0xff, 0x3f, 0xff, 0x7f,
                0xff, 0x3f, 0x00, 0x00, 0x01, 0xc0,
                0x00, 0x80, 0x01, 0xc0, 0x00, 0x00,
                0x00, 0x00, 0x01, 0xc0, 0x00, 0x80,
                0x01, 0xc0, 0x00, 0x00, 0xff, 0x3f,
                0xff, 0x7f, 0xff, 0x3f, 0x00, 0x00
            }
        }
    };
}
