using FluentValidation;
using RealTimeApp.Client.ViewModels;

namespace RealTimeApp.Client.Validators.Account
{
    public class SendEmailOtpValidator : AbstractValidator<SendOtp_ViewModel>
    {
        public SendEmailOtpValidator()
        {
            RuleFor(x => x.email)
       .NotEmpty().WithMessage("Email is required")
       .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.username)
                .NotEmpty().WithMessage("Username is required");

            RuleFor(x => new { x.email, x.username, x.domain })
                .Must(model =>
                {
                    var emailParts = model.email?.Split('@');
                    return emailParts != null && emailParts.Length == 2 &&
                           model.domain == emailParts[1] && 
                           $"{model.username}@{model.domain}" == model.email;
                })
                .WithMessage("Domain must match the email domain when combined with username");
        }
    }
}
