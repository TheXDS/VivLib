namespace TheXDS.Vivianne.Models.Base;

internal class INameableTests
{
    private class TestNameable : INameable
    {
        string INameable.Name { get; set; } = "Test";
    }

    [Test]
    public void MCART_INameable_default_impl_calls_Name()
    {
        var test = new TestNameable();
        var result = ((TheXDS.MCART.Types.Base.INameable)test).Name;
        Assert.That(result, Is.EqualTo("Test"));
    }
}