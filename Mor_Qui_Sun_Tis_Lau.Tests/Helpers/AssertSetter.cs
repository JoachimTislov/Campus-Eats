namespace Mor_Qui_Sun_Tis_Lau.Tests.Helpers;

public class AssertSetter
{   
    public static void AssertProtected(Type type, string propertyName)
    {
        var property = type.GetProperty(propertyName);
        Assert.NotNull(property);
        var setter = property.GetSetMethod(true);
        Assert.NotNull(setter);
        Assert.True(setter.IsFamily, $"{propertyName} setter is not protected");
    }

    public static void AssertPrivate(Type type, string propertyName)
    {
        var property = type.GetProperty(propertyName);
        Assert.NotNull(property);
        var setter = property.GetSetMethod(true);
        Assert.NotNull(setter);
        Assert.True(setter.IsPrivate, $"{propertyName} setter is not private");
    }

    public static void AssertPublic(Type type, string propertyName)
    {
        var property = type.GetProperty(propertyName);
        Assert.NotNull(property);
        var setter = property.GetSetMethod(true);
        Assert.NotNull(setter);
        Assert.True(setter.IsPublic, $"{propertyName} setter is not public");
    }
}