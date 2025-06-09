using FluentValidation;
using FoxTales.Application.DTOs.User;
using FoxTales.Domain.Interfaces;

namespace FoxTales.Application.Validators.User;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Must(email => !ExistsByEmailAsync(email, userRepository))
            .WithMessage("Email is already registered");

        RuleFor(x => x.Username)
            .NotEmpty()
            .Length(3, 21)
            .Must(username => !ExistsByUsernameAsync(username, userRepository))
            .WithMessage("Username already taken");

        RuleFor(x => x.Password).MinimumLength(6);
        RuleFor(x => x.ConfirmPassword).Equal(e => e.Password);
    }

    private static bool ExistsByEmailAsync(string email, IUserRepository userRepository)
    {
        return userRepository.ExistsByEmailAsync(email).GetAwaiter().GetResult();
    }

    private static bool ExistsByUsernameAsync(string username, IUserRepository userRepository)
    {
        return userRepository.ExistsByUsernameAsync(username).GetAwaiter().GetResult();
    }
}
