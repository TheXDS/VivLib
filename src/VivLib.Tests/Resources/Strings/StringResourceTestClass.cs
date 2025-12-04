using System.Globalization;
using System.Reflection;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Helpers;

namespace TheXDS.Vivianne.Resources.Strings;

internal abstract class StringResourceTestClass(Type resourceClass)
{
    private readonly Type resourceClass = resourceClass;
    private readonly PropertyInfo cultureProperty = resourceClass.GetProperty("Culture", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static) ?? throw new TamperException();

    private void SetCulture(CultureInfo culture)
    {
        cultureProperty.SetValue(null, culture);
    }

    private CultureInfo GetCulture()
    {
        return (CultureInfo)cultureProperty.GetValue(null)!;
    }

    [TestCase("es-MX")]
    [TestCase("en-US")]
    public void Translations_Test(string culture)
    {
        SetCulture(CultureInfo.CreateSpecificCulture(culture));
        Assert.That(GetCulture().Name, Is.EqualTo(culture));
        foreach (var property in resourceClass.GetPropertiesOf<string>(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
        {
            Assert.That(property.GetValue(null) as string, Is.Not.Null.And.Not.Empty);
        }
    }
}
