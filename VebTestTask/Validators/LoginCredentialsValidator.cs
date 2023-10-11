using FluentValidation;
using VebTestTask.Models;

namespace VebTestTask.Validators;

public class LoginCredentialsValidator : AbstractValidator<LoginCredentials>
{
    public LoginCredentialsValidator()
    {
        RuleFor(login => login.Name)
            .NotNull().WithMessage("Name must not be empty");

        RuleFor(login => login.Email)
            .NotNull().WithMessage("Email address must not be empty")
            .EmailAddress().WithMessage("Wrong email format");
    }
}