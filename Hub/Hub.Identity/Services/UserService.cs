using Hub.Identity.Interfaces;
using Hub.Identity.Entities;

namespace Hub.Identity.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task RegisterAsync(string username, string email, string password)
    {
        if (await _userRepository.ExistsByUsernameAsync(username))
            throw new Exception("Username already exists.");

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = password
        };

        await _userRepository.AddAsync(user);
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        IEnumerable<User> users = await _userRepository.GetAllUsers();
        return users;
    }
}
