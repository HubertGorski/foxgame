
using FoxTales.Domain.Entities;

namespace FoxTales.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetAllUsers();
    Task AddAsync(User user);
    Task<bool> ExistsByUsernameAsync(string username);
}
