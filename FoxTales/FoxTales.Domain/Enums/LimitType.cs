namespace FoxTales.Domain.Enums;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LimitType
{
    [EnumMember(Value = "Achievement")]
    Achievement,
    [EnumMember(Value = "Permission")]
    Permission,
    [EnumMember(Value = "PermissionGame")]
    PermissionGame
}
