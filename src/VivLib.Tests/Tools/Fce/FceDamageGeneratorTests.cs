using System.Numerics;

namespace TheXDS.Vivianne.Tools.Fce;

[TestFixture]
internal class FceDamageGeneratorTests
{
    [Test]
    public void GenerateDamageMesh_WithNullInput_ReturnsEmptyArray()
    {
        Vector3[] result = FceDamageGenerator.GenerateDamageMesh(null!);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GenerateDamageMesh_WithEmptyInput_ReturnsEmptyArray()
    {
        Vector3[] result = FceDamageGenerator.GenerateDamageMesh([]);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GenerateDamageMesh_SingleVector_ProducesPerturbedResult()
    {
        Vector3 original = new(1f, 2f, 3f);
        Vector3[] result = FceDamageGenerator.GenerateDamageMesh([original], variation: 0.1f);

        Assert.That(result, Has.Length.EqualTo(1));
        foreach (var vec in result)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(vec.X, Is.InRange(original.X - 0.15f, original.X - 0.05f));
                Assert.That(vec.Y, Is.InRange(original.Y - 0.15f, original.Y + 0.05f));
                Assert.That(vec.Z, Is.InRange(original.Z - 0.15f, original.Z - 0.05f));
            }
        }
    }

    [Test]
    public void GenerateDamageMesh_MultipleVectors_ProducesPerturbedResults()
    {
        Vector3[] originals = [
            new(1f, 2f, 3f),
            new(4f, 5f, 6f)
        ];

        Vector3[] result = FceDamageGenerator.GenerateDamageMesh(originals, variation: 0.2f);

        Assert.That(result, Has.Length.EqualTo(2));
        for (int i = 0; i < originals.Length; i++)
        {
            var original = originals[i];
            var vec = result[i];

            using (Assert.EnterMultipleScope())
            {
                Assert.That(vec.X, Is.InRange(original.X - 0.3f, original.X - 0.1f));
                Assert.That(vec.Y, Is.InRange(original.Y - 0.1f, original.Y + 0.1f));
                Assert.That(vec.Z, Is.InRange(original.Z - 0.3f, original.Z - 0.1f));
            }
        }
    }

    [Test]
    public void GenerateDamageMesh_VariationParameter_AffectsPerturbation()
    {
        Vector3[] originals = { new(1f, 2f, 3f) };

        var resultLow = FceDamageGenerator.GenerateDamageMesh(originals, variation: 0.1f);
        var resultHigh = FceDamageGenerator.GenerateDamageMesh(originals, variation: 0.5f);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(resultLow[0].X, Is.InRange(0.85f, 0.95f));
            Assert.That(resultHigh[0].X, Is.InRange(0.25f, 0.75f));
        }
    }

    [Test]
    public void GenerateRandomFloat_ProducesValuesInRange()
    {
        for (int i = 0; i < 10000; i++)
        {
            float value = FceDamageGenerator.GenerateRandomFloat();
            Assert.That(value, Is.InRange(0f, 1f));
        }
    }

    [Test]
    public void GenerateDamageMesh_VariationZero_ProducesSameResult()
    {
        Vector3[] originals = [new(1f, 2f, 3f)];
        
        var result = FceDamageGenerator.GenerateDamageMesh(originals, variation: 0.0f);
        
        Assert.That(result[0], Is.EqualTo(originals[0]));
    }

    [Test]
    public void GenerateDamageMesh_DuplicateVectors_ProducesConsistentResults()
    {
        Vector3[] originals = [new(1f, 2f, 3f), new(1f, 2f, 3f)];

        var result = FceDamageGenerator.GenerateDamageMesh(originals, variation: 0.1f);

        Assert.That(result, Has.Length.EqualTo(2));
        Assert.That(result[0], Is.EqualTo(result[1]));
    }
    
    [Test]
    public void GenerateDamageMesh_DifferentVectors_ProducesConsistentResults()
    {
        Vector3[] originals = [new(1f, 2f, 3f), new(4f, 5f, 6f)];
        
        var result = FceDamageGenerator.GenerateDamageMesh(originals, variation: 0.1f);
        
        Assert.That(result, Has.Length.EqualTo(2));
        Assert.That(result[0], Is.Not.EqualTo(result[1]));
    }
}
