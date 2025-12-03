#pragma warning disable CS1591

using TheXDS.Vivianne.Models.Audio.Base;
using TheXDS.Vivianne.Models.Audio.Mus;

namespace TheXDS.Vivianne.Serializers;

[TestFixture]
public class AsfSerializer_Mono44100_Tests() : AsfSerializerTests("test_mono44100.asf", GetDefaultFile())
{
    private static AsfFile GetDefaultFile() => new()
    {
        Channels = 1,
        Compression = CompressionMethod.None,
        SampleRate = 44100,
        BytesPerSample = 2,
        AudioBlocks =
        {
            new byte[]
            {
                0x00, 0x00, 0xFF, 0x1F, 0xFF, 0x3F,
                0xFF, 0x7F, 0xFF, 0x3F, 0xFF, 0x1F,
                0x00, 0x00, 0x00, 0xE0, 0x00, 0xC0,
                0x00, 0x80, 0x00, 0xC0, 0x00, 0xE0,
                0x00, 0x00
            }
        }
    };
}
