using FoxTales.Domain.Enums;

namespace FoxTales.Domain.Entities;

public class Role
{
    public int RoleId { get; set; }
    public required RoleName Name { get; set; }
}
