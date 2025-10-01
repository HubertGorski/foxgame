namespace FoxTales.Domain.Enums;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RoleName
{
    [EnumMember(Value = "User")]
    User = 1,
    [EnumMember(Value = "Super User")]
    SuperUser = 2,
    [EnumMember(Value = "Admin")]
    Admin = 3,
    [EnumMember(Value = "TmpUser")]
    TmpUser = 4
}
