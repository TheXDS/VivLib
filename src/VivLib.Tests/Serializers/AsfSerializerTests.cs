#pragma warning disable CS1591
using TheXDS.Vivianne.Models.Audio.Mus;
using TheXDS.Vivianne.Serializers.Audio.Mus;

namespace TheXDS.Vivianne.Serializers;

public abstract class AsfSerializerTests(string streamName, AsfFile referenceFile) : SerializerTestsBase<MusSerializer, AsfFile>(streamName, referenceFile)
{
    protected override void TestParsedFile(AsfFile expected, AsfFile actual)
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(actual.Channels, Is.EqualTo(expected.Channels));
            Assert.That(actual.Compression, Is.EqualTo(expected.Compression));
            Assert.That(actual.SampleRate, Is.EqualTo(expected.SampleRate));
            Assert.That(actual.BytesPerSample, Is.EqualTo(expected.BytesPerSample));
            Assert.That(actual.CalculatedDuration, Is.EqualTo(expected.CalculatedDuration));
            Assert.That(actual.TotalSamples, Is.EqualTo(expected.TotalSamples));
            Assert.That(actual.LoopStart, Is.EqualTo(expected.LoopStart));
            Assert.That(actual.LoopEnd, Is.EqualTo(expected.LoopEnd));
            Assert.That(actual.Interleaved, Is.EqualTo(expected.Interleaved));
            Assert.That(actual.LoopOffset, Is.EqualTo(expected.LoopOffset));
            Assert.That(actual.ByteAlignment, Is.EqualTo(expected.ByteAlignment));
        }
        foreach (var (First, Second) in expected.AudioBlocks.Zip(actual.AudioBlocks))
        {
            Assert.That(First, Is.EquivalentTo(Second));
        }
    }
}