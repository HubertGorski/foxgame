
using FoxTales.Domain.Entities;

namespace FoxTales.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<ICollection<User>> GetAllUsers();
    Task<ICollection<User>> GetAllUsersWithCards();
    Task AddAsync(User user);
    Task<bool> ExistsByUsernameAsync(string username);
}
