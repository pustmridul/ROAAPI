using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using System.Globalization;
using MemApp.Application.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using MemApp.Application.Extensions;

namespace MemApp.Application.Mem.Commands;


public class ResetMemberPinCommand : IRequest<Result>
{
    public int MemberId { get; set; }
    public string? OldPin { get; set; }
    public string? NewPin { get; set; }
}

public class ResetMemberPinCommandHandler : IRequestHandler<ResetMemberPinCommand, Result>
{
    private readonly IMemDbContext _context;
    private readonly IMediator _mediator;
    private readonly IPasswordHash _passwordHash;
    private readonly ICurrentUserService _currentUserService;
    public ResetMemberPinCommandHandler(IMemDbContext context, IMediator mediator, IPasswordHash passwordHash, ICurrentUserService currentUserService)
    {
        _context = context;
        _mediator = mediator;
        _passwordHash = passwordHash;
        _currentUserService = currentUserService;
    }
    public async Task<Result> Handle(ResetMemberPinCommand request, CancellationToken cancellation)
    {
        try
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result();
            var memObj = await _context.RegisterMembers.SingleOrDefaultAsync(q => q.Id == request.MemberId && q.IsActive, cancellation);
            if (memObj == null)
            {
                result.HasError = true;
                result.Messages.Add("Member Not Found");
                return result;

            }
            var obj = await _context.Users
                .SingleOrDefaultAsync(q => q.MemberId == request.MemberId && q.UserName == memObj.CardNo, cancellation);

            if (obj == null)
            {
                result.HasError = true;
                result.Messages.Add("User Not Found");

            }
            else
            {
                var newPassword = new Random(DateTime.Now.Millisecond).Next(1000, 9999);

                string newPasswordHash = string.Empty;
                string newPasswordSaltHash = string.Empty;

                _passwordHash.CreateHash(newPassword.ToString(CultureInfo.InvariantCulture), ref newPasswordHash,
                    ref newPasswordSaltHash);
                obj.PasswordHash = newPasswordHash;
                obj.PasswordSalt = newPasswordSaltHash;

                obj.LastPasswordResetBy = _currentUserService.UserId;
                obj.LastPasswordResetByName = _currentUserService.Username;
                obj.LastPasswordResetOn = DateTime.Now;

                memObj.PinNoHash = newPasswordHash;
                memObj.PinNoSalt = newPasswordSaltHash;

                memObj.PinNo = _passwordHash.GetEncryptedPassword(newPassword.ToString());

                await _context.SaveChangesAsync(cancellation);
                result.HasError = false;
                result.Messages.Add("New Pin is : " + newPassword);

            }

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
      
    }
   
}


