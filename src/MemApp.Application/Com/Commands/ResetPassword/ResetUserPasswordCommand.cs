using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using MemApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemApp.Application.Models;
using Microsoft.EntityFrameworkCore;
using MemApp.Application.Extensions;

namespace MemApp.Application.Com.Commands.ResetPassword
{

    public class ResetUserPasswordCommand : IRequest<Result>
    {
        public int UserId { get; set; }
    }

    public class ResetUserPasswordCommandHandler : IRequestHandler<ResetUserPasswordCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly IPasswordHash _passwordHash;
        private readonly ICurrentUserService _currentUserService;
        public ResetUserPasswordCommandHandler(IMemDbContext context, IMediator mediator, IPasswordHash passwordHash, ICurrentUserService currentUserService)
        {
            _context = context;
            _mediator = mediator;
            _passwordHash = passwordHash;
            _currentUserService = currentUserService;
        }
        public async Task<Result> Handle(ResetUserPasswordCommand request, CancellationToken cancellation)
        {
            var result = new Result();
            var obj = await _context.Users
                .SingleOrDefaultAsync(q => q.Id == request.UserId);

            if(obj == null)
            {
                result.HasError = true;
                result.Messages.Add("User Not Found");
            }
            else
            {

                var member = await _context.RegisterMembers.FirstOrDefaultAsync(q => q.CardNo == obj.UserName);
               
                var newPassword = new Random(DateTime.Now.Millisecond).Next(1000, 9999);

                string newPasswordHash = string.Empty;
                string newPasswordSaltHash = string.Empty;

                _passwordHash.CreateHash(newPassword.ToString(CultureInfo.InvariantCulture), ref newPasswordHash,
                    ref newPasswordSaltHash);
                obj.PasswordHash = newPasswordHash;
                obj.PasswordSalt = newPasswordSaltHash;

                if (member != null)
                {
                    member.PinNoHash= newPasswordHash;
                    member.PinNoSalt= newPasswordSaltHash;
                    member.PinNo = _passwordHash.GetEncryptedPassword(newPassword.ToString());
                }


                obj.LastPasswordResetBy = _currentUserService.UserId;
                obj.LastPasswordResetByName = _currentUserService.Username;
                obj.LastPasswordResetOn = DateTime.Now;

                await _context.SaveChangesAsync(cancellation);

                result.HasError = false;
                result.Messages.Add("New Password is : " + newPassword);

            }

            return result;
        }
    }
}


