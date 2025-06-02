using System;
using FoxTales.Domain.Enums;

namespace FoxTales.Domain.Entities;

public class DylematyCard
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Text { get; set; } = null!;
    public DylematyCardType Type { get; set; }
}
