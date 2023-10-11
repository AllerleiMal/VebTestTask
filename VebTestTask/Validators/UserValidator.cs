using FluentValidation;
using VebTestTask.Data.Repositories;
using VebTestTask.Models;

namespace VebTestTask.Validators;

public class UserValidator : AbstractValidator<User>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    public UserValidator(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        RuleFor(user => user.Age)
            .NotNull().WithMessage("Age must not be empty")
            .GreaterThan(0).WithMessage("Age must be positive");
        RuleFor(user => user.Name)
            .NotNull().WithMessage("Name must not be empty");
        RuleFor(user => user.Email)
            .NotNull().WithMessage("Email address must not be empty")
            .EmailAddress().WithMessage("Wrong email format")
            .MustAsync(IsEmailUnique).WithMessage("This email address is already taken");
        RuleFor(user => user.Roles)
            .NotNull().WithMessage("User must have at least one role")
            .MustAsync(AreRolesValid).WithMessage("Roles ids are invalid");
    }

    private async Task<bool> IsEmailUnique(User user, string email, CancellationToken token)
    {
        var foundUser = await _userRepository.GetUserByEmailAsync(email);

        if (foundUser is null)
        {
            return true;
        }

        return user.Id == foundUser.Id;
    }

    private async Task<bool> AreRolesValid(User user, ICollection<Role> roles, CancellationToken token)
    {
        var allRoles = await _roleRepository.GetRolesAsync();

        if (roles.Count == 0)
        {
            return false;
        }
        
        var result = roles.All(role => allRoles.Select(r => r.Id).Contains(role.Id));

        return result;
    }
}