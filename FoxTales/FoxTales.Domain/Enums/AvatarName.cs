namespace FoxTales.Domain.Enums;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AvatarName
{
    [EnumMember(Value = "Default")]
    Default = 1,
    [EnumMember(Value = "Crazy")]
    Crazy = 2,
    [EnumMember(Value = "Happy")]
    Happy = 3
}