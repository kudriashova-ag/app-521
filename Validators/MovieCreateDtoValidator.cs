using Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using myApp.DTOs.Movies;


namespace myApp.Validators;


public class MovieCreateDtoValidator : AbstractValidator<MovieCreateDto>
{
    public MovieCreateDtoValidator(AppDbContext db)
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Назва фільму не може бути порожньою.")
            .MaximumLength(100).WithMessage("Назва фільму не може перевищувати 100 символів.");

        RuleFor(x => x.Genre)
           .NotEmpty().WithMessage("Жанр фільму не може бути порожньою.")
           .MaximumLength(50).WithMessage("Жанр фільму не може перевищувати 50 символів.");

        RuleFor(x => x.Year)
          .Must(year => year >= 1888 && year <= DateTime.Now.Year)
          .WithMessage("Рік випуску фільму повинен бути від 1888 до поточного року.");

        RuleFor(x => x.DirectorId)
            .GreaterThan(0)
            .WithMessage("Ідентифікатор режисера повинен бути додатнім числом.")
            .DependentRules(() =>
            {
                // async-правило, фільтр викликати ValidateAsync  (Validate)
                RuleFor(x => x.DirectorId)
                    .MustAsync(async (directorId, ct) => await db.Directors.AnyAsync(d => d.Id == directorId, ct))
                    .WithMessage("Режисера з таким ідентифікатором не існує.");
            })
            .When(x => x.DirectorId.HasValue);
    }
}