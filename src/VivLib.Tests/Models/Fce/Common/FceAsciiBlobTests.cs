namespace TheXDS.Vivianne.Models.Fce.Common;

internal class FceAsciiBlobTests
{
    [Test]
    public void Struct_initializes_from_string()
    {
        FceAsciiBlob blob = new("Test");
        using (Assert.EnterMultipleScope())
        {
            Assert.That(blob.Value, Has.Length.EqualTo(64));
            Assert.That(blob.Value, Is.EquivalentTo([.. "Test"u8.ToArray(), .. new byte[60]]));
        }
    }

    [Test]
    public void Struct_ctor_contract_test()
    {
        Assert.That((Func<FceAsciiBlob>)(() => new FceAsciiBlob(new string('x', 64))), Throws.ArgumentException);
    }

    [Test]
    public void ToString_converts_to_string()
    {
        FceAsciiBlob blob = new("Test");
        Assert.That(blob.ToString(), Is.EqualTo("Test"));
    }

    [Test]
    public void Struct_supports_implicit_operator_from_string()
    {
        FceAsciiBlob blob = "Test";
        Assert.That(blob.ToString(), Is.EqualTo("Test"));
    }


    [Test]
    public void Struct_supports_implicit_operator_to_string()
    {
        FceAsciiBlob blob = new("Test");
        string str = blob;
        Assert.That(str, Is.EqualTo("Test"));
    }
}