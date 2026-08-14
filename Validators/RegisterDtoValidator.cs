using Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyApp.DTOs.Identity;


namespace myApp.Validators;


public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator(AppDbContext db)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email не може бути порожньою.")
            .EmailAddress().WithMessage("Email повинен бути валідним.")
            .MustAsync(async (email, ct) => !await db.Users.AnyAsync(u => u.Email == email, ct))
            .WithMessage("Користувач з такою електронною адресою вже існує.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль не може бути порожнім.")
            .MinimumLength(6).WithMessage("Пароль повинен містити не менше 6 символів.")
            //.Matches(@"[A-Z]").WithMessage("Пароль повинен містити хоча б одну велику літеру.")
            //.Matches(@"[a-z]").WithMessage("Пароль повинен містити хоча б одну малу літеру.")
            .Matches(@"[0-9]").WithMessage("Пароль повинен містити хоча б одну цифру.");
            //.Matches(@"[^A-Za-z0-9]").WithMessage("Пароль повинен містити хоча б один спеціальний символ.");
      
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Підтвердження паролю не може бути порожнім.")
            .Equal(x => x.Password).WithMessage("Паролі не співпадають.");
    }
}