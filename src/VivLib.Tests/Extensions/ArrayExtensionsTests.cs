namespace TheXDS.Vivianne.Extensions;

internal class ArrayExtensionsTests
{
    [Test]
    public void Wrapping_wraps_collection_around()
    {
        int[] test = [.. Enumerable.Range(1, 5)];
        Assert.That(test.Wrapping(10), Is.EquivalentTo([..Enumerable.Range(1, 5), .. Enumerable.Range(1, 5)]));
    }

    [Test]
    public void SkipIfMore_skips_if_enough_items()
    {
        int[] test = [.. Enumerable.Range(1, 5)];
        Assert.That(test.SkipIfMore(2), Is.EquivalentTo(Enumerable.Range(3, 3)));
    }

    [Test]
    public void SkipIfMore_does_not_skip_if_not_enough_items()
    {
        int[] test = [.. Enumerable.Range(1, 5)];
        Assert.That(test.SkipIfMore(6), Is.EquivalentTo(Enumerable.Range(1, 5)));
    }
}
