using FluentValidation;

namespace ResApp.Application.Com.Commands.CreateUser
{

    public class CreateUserValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Name is required")
                .Matches("^[a-zA-Z ]+$").WithMessage("Name must contain only alphabetic characters.");

            //RuleFor(x => x.UserName)
            //    .NotEmpty().WithMessage("Username is required")
            //    .MinimumLength(4).WithMessage("Username must be at least 4 characters");

            RuleFor(x => x.EmailId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address");

            RuleFor(x => x.Password).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");

            RuleFor(x => x.PhoneNo)
                .NotEmpty().WithMessage("Phone number is required");

            //RuleFor(x => x.AppId)
            //    .NotEmpty().WithMessage("AppId is required");

            RuleFor(x => x.UserNID)
                .NotEmpty().WithMessage("NID is required");
        }
    }
}
