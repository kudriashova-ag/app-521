using FluentValidation;
using MyApp.DTOs.Identity;
using MyApp.Features.Auth.Commands.Register;

namespace MyApp.Features.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator(IValidator<LoginDto> dtoValidator)
    {
        RuleFor(x => x.Dto)
             .NotNull().WithMessage("Дані для входу не можуть бути порожніми.")
             .SetValidator(dtoValidator);
    }
}