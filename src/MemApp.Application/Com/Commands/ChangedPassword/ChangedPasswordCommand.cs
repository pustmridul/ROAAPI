using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Enums;
using MemApp.Application.Extensions;
using ResApp.Application.Interfaces;

namespace MemApp.Application.Com.Commands.ChangedPassword
{

    public class ChangedPasswordCommand : IRequest<Result<UserDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;

        public int LoginFailedAttemptCount { get; set; }
        public DateTime? LastLoginFailedAttemptDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public UserStatus Status { get; set; }
        public string PhoneNo { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string AppId { get; set; } = string.Empty;
    }

    public class ChangedPasswordCommandHandler : IRequestHandler<ChangedPasswordCommand, Result<UserDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly IPasswordNewHash _passwordHash;
        private readonly ICurrentUserService _currentUserService;
        public ChangedPasswordCommandHandler(IMemDbContext context, IMediator mediator, IPasswordNewHash passwordHash, ICurrentUserService currentUserService)
        {
            _context = context;
            _mediator = mediator;
            _passwordHash = passwordHash;
            _currentUserService = currentUserService;
        }
        public async Task<Result<UserDto>> Handle(ChangedPasswordCommand request, CancellationToken cancellation)
        {
            var result = new Result<UserDto>();
            var obj = await _context.Users
                .SingleOrDefaultAsync(q => q.Id == request.Id);

            if(obj == null)
            {
                result.HasError = true;
                result.Messages.Add("User Not Found");
            }
            else
            {
                if (request.NewPassword != request.ConfirmPassword)
                {
                    result.HasError = true;
                    result.Messages.Add("New Password And Confirm Password is not matched");
                }
                else
                {
                    var isLoginValid = _passwordHash.ValidatePassword(request.OldPassword, obj.PasswordHash, obj.PasswordSalt);

                    if (!isLoginValid)
                    {
                        result.HasError = true;
                        result.Messages.Add("Your Current Password in wrong!");
                    }
                    else
                    {
                        string newPasswordHash = string.Empty;
                        string newPasswordSaltHash = string.Empty;

                        _passwordHash.CreateHash(request.NewPassword.ToString(CultureInfo.InvariantCulture), ref newPasswordHash,
                            ref newPasswordSaltHash);
                        var member = await _context.RegisterMembers.SingleOrDefaultAsync(q=>q.CardNo== obj.UserName && q.Id==obj.MemberId, cancellation);
                        if(member == null)
                        {

                        }
                        else
                        {
                            member.PinNoHash = newPasswordHash;
                            member.PinNoSalt= newPasswordSaltHash;
                            member.PinNo= _passwordHash.GetEncryptedPassword(request.NewPassword);
                        }
                       
                        obj.PasswordHash = newPasswordHash;
                        obj.PasswordSalt = newPasswordSaltHash;
                        obj.LastPasswordResetBy = _currentUserService.UserId;
                        obj.LastPasswordResetByName = _currentUserService.Username;
                        obj.LastPasswordResetOn = DateTime.Now;


                        if (await _context.SaveChangesAsync(cancellation) > 0)
                        {
                            result.HasError = false;
                            result.Messages.Add("Password Changed Success");
                        }
                        else
                        {
                            result.HasError = true;
                            result.Messages.Add("something wrong");
                        }
                    }
                }            
            }
            return result;
        }
    }
}


