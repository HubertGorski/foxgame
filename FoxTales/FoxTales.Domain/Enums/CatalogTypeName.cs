namespace FoxTales.Domain.Enums;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CatalogTypeName
{
    [EnumMember(Value = "Small")]
    Small = 1,
    [EnumMember(Value = "Medium")]
    Medium = 2,
    [EnumMember(Value = "Large")]
    Large = 3,
    [EnumMember(Value = "NoLimit")]
    NoLimit = 4,
    [EnumMember(Value = "Public")]
    Public = 5
}