using FluentValidation;
using FoxTales.Application.DTOs.User;

namespace FoxTales.Application.Validators.User;

public class LoginUserDtoValidator : AbstractValidator<LoginUserDto>
{
    public LoginUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password).MinimumLength(6);
    }
}
