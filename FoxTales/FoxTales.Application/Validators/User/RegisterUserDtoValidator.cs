using FluentValidation;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Helpers;
using FoxTales.Domain.Interfaces;

namespace FoxTales.Application.Validators.User;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(DictHelper.Validation.EmailCannotBeEmpty)
            .EmailAddress().WithMessage(DictHelper.Validation.EmailFormatIsIncorrect)
            .Must(email => !ExistsByEmailAsync(email, userRepository))
            .WithMessage(DictHelper.Validation.EmailIsAlreadyTaken);

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(DictHelper.Validation.UsernameCannotBeEmpty)
            .Length(3, 21).WithMessage(DictHelper.Validation.UsernameFormatIsIncorrect)
            .Must(username => !ExistsByUsernameAsync(username, userRepository))
            .WithMessage(DictHelper.Validation.UsernameIsAlreadyTaken);

        RuleFor(x => x.Password).MinimumLength(6).WithMessage(DictHelper.Validation.PasswordIsTooShort);
        RuleFor(x => x.ConfirmPassword).Equal(e => e.Password).WithMessage(DictHelper.Validation.PasswordsAreNotIdentical);
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
