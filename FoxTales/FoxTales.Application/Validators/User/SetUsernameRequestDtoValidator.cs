using FluentValidation;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Helpers;
using FoxTales.Domain.Interfaces;

namespace FoxTales.Application.Validators.User;

public class SetUsernameRequestDtoValidator : AbstractValidator<SetUsernameRequestDto>
{
    public SetUsernameRequestDtoValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(DictHelper.Validation.UsernameCannotBeEmpty)
            .Length(3, 21).WithMessage(DictHelper.Validation.UsernameFormatIsIncorrect)
            .Must(username => !ExistsByUsernameAsync(username, userRepository))
            .WithMessage(DictHelper.Validation.UsernameIsAlreadyTaken);
    }

    private static bool ExistsByUsernameAsync(string username, IUserRepository userRepository)
    {
        return userRepository.ExistsByUsernameAsync(username).GetAwaiter().GetResult();
    }
}
