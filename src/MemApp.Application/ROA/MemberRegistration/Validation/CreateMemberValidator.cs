using FluentValidation;
using MemApp.Application.Com.Models;
using MemApp.Application.Mem.Members.Command;
using ResApp.Application.ROA.MemberRegistration.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.ROA.MemberRegistration.Validation
{
   
    public class CreateMemberValidator : AbstractValidator<CreateMemberRegCommand>
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };
        private const long MaxFileSize = 2 * 1024 * 1024; // 2MB in bytes
        public CreateMemberValidator()
        {

            Include(new MemberValidator());

            //RuleFor(file => file.ProfileImg)
            //   .Cascade(CascadeMode.Stop)
            //   .NotNull().WithMessage("Signature image is required.")
            //   .Must(file => AllowedExtensions.Contains(Path.GetExtension(file!.FileName).ToLower()))
            //       .WithMessage("Only image files (.jpg, .jpeg, .png) are allowed.")
            //   .Must(file => file!.Length <= MaxFileSize)
            //       .WithMessage("Image file size must be less than 2MB.");

            RuleFor(file => file.SignatureImgFile)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Signature image is required.")
                .Must(file =>  AllowedExtensions.Contains(Path.GetExtension(file!.FileName).ToLower()))
                    .WithMessage("Only image files (.jpg, .jpeg, .png) are allowed for Signature")
                .Must(file =>  file!.Length <= MaxFileSize)
                    .WithMessage("Image file size must be less than 2MB.");

            RuleFor(file => file.NIDImgFile)
                 .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("NID image is required.")
                .Must(file => file!=null && AllowedExtensions.Contains(Path.GetExtension(file!.FileName).ToLower()))
                    .WithMessage("Only image files (.jpg, .jpeg, .png) are allowed, for NID")
                .Must(file => file != null && file!.Length <= MaxFileSize)
                    .WithMessage("Image file size must be less than 2MB.");

            RuleFor(file => file.TradeLicenseImgFile)
               .Cascade(CascadeMode.Stop)
               .NotNull().WithMessage("Trade License image is required.")
               .Must(file => file != null && AllowedExtensions.Contains(Path.GetExtension(file!.FileName).ToLower()))
                   .WithMessage("Only image files (.jpg, .jpeg, .png) are allowed, for Trade License")
               .Must(file => file != null && file!.Length <= MaxFileSize)
                   .WithMessage("Image file size must be less than 2MB.");

            RuleFor(file => file.TinImgFile)
                 .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("TIN file image is required.")
                .Must(file => file != null && AllowedExtensions.Contains(Path.GetExtension(file!.FileName).ToLower()))
                  .WithMessage("Only image files (.jpg, .jpeg, .png) are allowed, for TIN")
                .Must(file => file != null && file!.Length <= MaxFileSize)
                  .WithMessage("Image file size must be less than 2MB.");

           
        }
    }
}
