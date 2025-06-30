using System.Reflection;
using System.Runtime.Serialization;
using FoxTales.Domain.Enums;

namespace FoxTales.Domain.Extensions;

public static class FoxGameNameExtensions
{
    public static string GetStringValue(this FoxGameName value)
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        if (name == null)
            return value.ToString();

        var field = type.GetField(name);
        if (field == null)
            return value.ToString();

        var attr = field.GetCustomAttribute<EnumMemberAttribute>();
        return attr?.Value ?? name;
    }
}
