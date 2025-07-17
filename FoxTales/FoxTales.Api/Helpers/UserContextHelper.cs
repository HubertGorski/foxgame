using System.Security.Claims;
using FoxTales.Application.Helpers;

namespace FoxTales.Api.Helpers;

public static class UserContextHelper
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException(DictHelper.Validation.TokenDoesNotContainUserId);
        return int.Parse(userIdClaim.Value);
    }
}
