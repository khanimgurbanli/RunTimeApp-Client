using FluentValidation;
using RealTimeApp.Client.ViewModels;
using System.Text.RegularExpressions;

namespace RealTimeApp.Client.Validators.Account
{
    public class LoginValidator : AbstractValidator<Login_ViewModel>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Login)
                .EmailAddress().WithMessage("Enter Your Email");

            RuleFor(x => x.Password)
                 .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
                 .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                 .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                 .Matches("[0-9]").WithMessage("Password must contain at least one digit")
                 .Matches("[^a-zA-Z0-9]").WithMessage("Enter Your Password");
        }
    }
}
