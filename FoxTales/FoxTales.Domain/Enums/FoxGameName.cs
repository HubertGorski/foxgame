namespace FoxTales.Domain.Enums;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FoxGameName
{
    [EnumMember(Value = "Psych")]
    Psych = 1,
    [EnumMember(Value = "Dylematy")]
    Dylematy = 2,
    [EnumMember(Value = "Killgame")]
    KillGame = 3
}
