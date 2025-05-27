using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.ROA.MemberRegistration.Validation
{
    public class MemberValidator : AbstractValidator<IMemberValidator>
    {
        public MemberValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("Name is required");

          //  RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.EmailId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address");
            //  RuleFor(x => x.UserName).NotEmpty().MaximumLength(50);

            RuleFor(x => x.PhoneNo)
               .NotEmpty().WithMessage("Phone number is required");

            RuleFor(x => x.ApplicationNo)
               .NotEmpty().WithMessage("Application No is required");

            RuleFor(x => x.NomineeName)
                .NotEmpty().WithMessage("Nominee Name is required");

            RuleFor(x => x.InstituteNameEnglish)
                .NotEmpty().WithMessage("Institute Name is required");

            RuleFor(x => x.InstituteNameBengali)
                .NotEmpty().WithMessage("Institute Name(Bangla) is required");

            RuleFor(x => x.PermanentAddress)
                .NotEmpty().WithMessage("Permanent Address is required");

            RuleFor(x => x.MemberNID)
                .NotEmpty().WithMessage("NID is required");

            RuleFor(x => x.MemberTINNo)
                .NotEmpty().WithMessage("TIN No is required");

            RuleFor(x => x.MemberTradeLicense)
                .NotEmpty().WithMessage("Trade License No is required");

            RuleFor(x => x.BusinessStartingDate)
                .NotEmpty().WithMessage("Business Starting Date is required");
            //  RuleFor(x => x.PhoneNo).NotEmpty().Length(10, 15);
        }
    }
}
