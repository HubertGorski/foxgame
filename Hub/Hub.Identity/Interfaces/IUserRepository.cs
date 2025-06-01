using Hub.Identity.Entities;

namespace Hub.Identity.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetAllUsers();
    Task AddAsync(User user);
    Task<bool> ExistsByUsernameAsync(string username);
}
