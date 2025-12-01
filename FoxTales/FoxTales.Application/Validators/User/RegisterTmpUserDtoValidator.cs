using FluentValidation;
using FoxTales.Application.DTOs.User;
using FoxTales.Application.Helpers;
using FoxTales.Domain.Interfaces;

namespace FoxTales.Application.Validators.User;

public class RegisterTmpUserDtoValidator : AbstractValidator<RegisterTmpUserDto>
{
    public RegisterTmpUserDtoValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(DictHelper.Validation.UsernameCannotBeEmpty)
            .Length(3, 21).WithMessage(DictHelper.Validation.UsernameFormatIsIncorrect)
            .Must(username => !ExistsByUsernameAsync(username, userRepository))
            .WithMessage(DictHelper.Validation.UsernameIsAlreadyTaken);

        RuleFor(x => x.TermsAccepted).Equal(true).WithMessage(DictHelper.Validation.TermsAcceptedCannotBeEmpty);
    }

    private static bool ExistsByUsernameAsync(string username, IUserRepository userRepository)
    {
        return userRepository.ExistsByUsernameAsync(username).GetAwaiter().GetResult();
    }
}