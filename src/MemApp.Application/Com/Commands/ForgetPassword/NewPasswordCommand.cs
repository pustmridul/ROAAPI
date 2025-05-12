using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using MemApp.Application.App.Models;
using ResApp.Application.Interfaces;
using MemApp.Application.Mem.Members.Models;
using ResApp.Application.Services;

namespace MemApp.Application.Com.Commands.ChangedPassword
{

    public class NewPasswordCommand : IRequest<UserVm>
    {
        public NewPasswordReq Model { get; set; } = new NewPasswordReq();
    }

    public class NewPasswordCommandHandler : IRequestHandler<NewPasswordCommand, UserVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPasswordNewHash _passwordHash;
        public NewPasswordCommandHandler(IMemDbContext context, IMediator mediator, IPasswordNewHash passwordHash)
        {
            _context = context;
            _passwordHash = passwordHash;
        }
        public async Task<UserVm> Handle(NewPasswordCommand request, CancellationToken cancellation)
        {
            var result = new UserVm();
            var obj = await _context.Users
                .SingleOrDefaultAsync(q => q.MemberId == request.Model.MemberId);

            if(obj == null)
            {
                result.HasError = true;
                result.Messages.Add("User Not Found");
                return result;
            }
            else
            {
                if (request.Model.NewPassword != request.Model.ConfirmPassword)
                {
                    result.HasError = true;
                    result.Messages.Add("New Password And Confirm Password is not matched");
                    return result;
                }
                else
                {
                    if (obj.OtpCreatedTime == null )
                    {
                        result.HasError = true;
                        result.Messages.Add("OTP Time is null");
                        return result;
                    }
                    else if(obj.OtpCreatedTime.Value.AddMinutes(5)< DateTime.Now)
                    {
                        result.HasError = true;
                        result.Messages.Add("OTP Time is Expire");
                        return result;
                    }
                    else if(obj.Otp==null || obj.Otp != request.Model.Otp)
                    {
                        result.HasError = true;
                        result.Messages.Add("OTP is not correct");
                        return result;
                    }
                    else
                    {   
                      //  var memberObj = await _context.RegisterMembers.SingleOrDefaultAsync(q=>q.Id==request.Model.MemberId, cancellation);
                        var memberObj = await _context.MemberRegistrationInfos.SingleOrDefaultAsync(q=>q.Id==request.Model.MemberId, cancellation);
                        if(memberObj == null)
                        {
                            throw new Exception();
                        }

                        string newPasswordHash = string.Empty;
                        string newPasswordSaltHash = string.Empty;

                        _passwordHash.CreateHash(request.Model.NewPassword!.ToString(CultureInfo.InvariantCulture), ref newPasswordHash,
                            ref newPasswordSaltHash);
                        obj.PasswordHash = newPasswordHash;
                        obj.PasswordSalt = newPasswordSaltHash;

                        obj.LastPasswordResetOn = DateTime.Now;

                        //memberObj.PinNoHash = newPasswordHash;
                        //memberObj.PinNoSalt = newPasswordSaltHash;

                        memberObj.Password = _passwordHash.GetEncryptedPassword(request.Model.NewPassword.ToString());
                        memberObj.PasswordHash = newPasswordHash;
                        memberObj.PasswordSalt = newPasswordSaltHash;

                        await _context.SaveChangesAsync(cancellation);

                        result.HasError = false;
                        result.Messages.Add("Password Changed Success & new password is "+ request.Model.NewPassword);
                        return result;
                    }
               

                }            
            }
        }
    }
}


