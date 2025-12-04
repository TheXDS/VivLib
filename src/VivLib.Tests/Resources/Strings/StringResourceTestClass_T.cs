namespace TheXDS.Vivianne.Resources.Strings;

internal abstract class StringResourceTestClass<T>() : StringResourceTestClass(typeof(T)) where T : notnull;
