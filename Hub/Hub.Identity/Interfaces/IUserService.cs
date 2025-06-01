using Hub.Identity.Entities;

namespace Hub.Identity.Interfaces;

public interface IUserService
{
    Task RegisterAsync(string username, string email, string password);
    Task<IEnumerable<User>> GetAllUsers();
}
