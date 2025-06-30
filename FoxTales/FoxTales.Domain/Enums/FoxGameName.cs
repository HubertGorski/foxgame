namespace FoxTales.Domain.Enums;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FoxGameName
{
    [EnumMember(Value = "Psych")]
    Psych,
    [EnumMember(Value = "Dylematy")]
    Dylematy,
    [EnumMember(Value = "Killgame")]
    KillGame
}
