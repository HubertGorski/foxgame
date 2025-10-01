using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FoxTales.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserStatus
{
    [EnumMember(Value = "Active")]
    Active = 1,
    [EnumMember(Value = "Deleted")]
    Deleted = 2,
    [EnumMember(Value = "Inactive")]
    Inactive = 3
}
