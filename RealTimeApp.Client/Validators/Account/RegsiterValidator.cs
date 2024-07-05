using FluentValidation;
using RealTimeApp.Client.ViewModels;
using System.Text.RegularExpressions;

namespace RealTimeApp.Client.Validators.Account
{
    public class RegsiterValidator : AbstractValidator<Register_ViewModel>
    {
        public RegsiterValidator()
        {
            RuleFor(x => x.email)
                .EmailAddress().WithMessage("Email is required");

            RuleFor(x => x.firstname)
                .NotEmpty().WithMessage("Firstname is required");

            RuleFor(x => x.lastname)
               .NotEmpty().WithMessage("Lastname is required");

            RuleFor(x => x.roleId)
              .NotEmpty().WithMessage("You must select one role ");

            RuleFor(x => x.password)
                 .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
                 .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                 .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
                 .Matches("[0-9]").WithMessage("Password must contain at least one digit")
                 .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
            RuleFor(x => x.phoneNumber)
                  .Cascade(CascadeMode.Stop)
                  .NotEmpty().WithMessage("Enter phone number")
                  .When(x => !string.IsNullOrEmpty(x.phoneNumber))
                  .Matches(new Regex(@"^(?:\+994\s?)?(?:50|60|51|55|70|77)\d{7}$")).WithMessage("Enter correct formap PhoneNumber.");

            RuleFor(x => x.rePassword)
              .Matches(x => x.password).WithMessage("Passwords must match");
        }
    }
}
