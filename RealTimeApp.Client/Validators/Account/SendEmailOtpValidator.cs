using FluentValidation;
using RealTimeApp.Client.ViewModels;

namespace RealTimeApp.Client.Validators.Account
{
    public class SendEmailOtpValidator : AbstractValidator<SendOtp_ViewModel>
    {
        public SendEmailOtpValidator()
        {
            RuleFor(x => x.email)
                .NotEmpty().WithMessage("Email is required");

            RuleFor(x => x.username)
                .NotEmpty().WithMessage("Username is required");

            RuleFor(x => x.domain)
                .Must((model, domain) =>
                {
                    var emailParts = model.email?.Split('@');
                    return emailParts != null && emailParts.Length > 1 && domain == emailParts[1];
                })
                .WithMessage("Domain must match the email domain when combined with username");
        }
    }
}
