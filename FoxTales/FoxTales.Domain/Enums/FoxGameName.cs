namespace FoxTales.Domain.Enums;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FoxGameName
{
    [EnumMember(Value = "psych")]
    Psych,
    [EnumMember(Value = "dylematy")]
    Dylematy,
    [EnumMember(Value = "killgame")]
    KillGame
}
