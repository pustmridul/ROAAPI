using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Interfaces;

namespace MemApp.Application.Com.Commands.ChangedPassword
{

    public class ChangedPinCommand : IRequest<ChangePin>
    {
        public ChangePin Model { get; set; } = new ChangePin();
    }

    public class ChangedPinCommandHandler : IRequestHandler<ChangedPinCommand, ChangePin>
    {
        private readonly IMemDbContext _context;
        private readonly IPasswordNewHash _passwordHash;
        private readonly ICurrentUserService _currentUserService;
        public ChangedPinCommandHandler(IMemDbContext context,  IPasswordNewHash passwordHash, ICurrentUserService currentUserService)
        {
            _context = context;
            _passwordHash = passwordHash;
            _currentUserService = currentUserService;
        }
        public async Task<ChangePin> Handle(ChangedPinCommand request, CancellationToken cancellation)
        {
            try
            {
                if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
                {
                    throw new UnauthorizedAccessException();
                }

                var result = new ChangePin();
                var obj = await _context.Users
                    .SingleOrDefaultAsync(q => q.MemberId == request.Model.MemberId, cancellation);

                var objCustomer = await _context.RegisterMembers
                    .SingleOrDefaultAsync(q => q.Id == request.Model.MemberId, cancellation);



                if (obj == null || objCustomer == null)
                {
                    result.HasError = true;
                    result.Messages.Add("User Not Found");
                    return result;
                }
                else
                {
                    if (request.Model.NewPin != request.Model.ConfirmPin)
                    {
                        result.HasError = true;
                        result.Messages.Add("New Password And Confirm Password is not matched");
                        return result;
                    }
                    else
                    {
                        var isLoginValid = _passwordHash.ValidatePassword(request.Model.OldPin, obj.PasswordHash, obj.PasswordSalt);
                        var isLoginValidCustomerMem = _passwordHash.ValidatePassword(request.Model.OldPin, objCustomer?.PinNoHash ?? "", objCustomer?.PinNoSalt ?? "");

                        if (!isLoginValid || !isLoginValidCustomerMem)
                        {
                            result.HasError = true;
                            result.Messages.Add("Your Current Password in wrong!");
                        }
                        else
                        {

                            string newPasswordHash = string.Empty;
                            string newPasswordSaltHash = string.Empty;

                            _passwordHash.CreateHash(request.Model.NewPin.ToString(CultureInfo.InvariantCulture), ref newPasswordHash,
                                ref newPasswordSaltHash);
                            obj.PasswordHash = newPasswordHash;
                            obj.PasswordSalt = newPasswordSaltHash;
                            obj.LastPasswordResetBy = _currentUserService.UserId;
                            obj.LastPasswordResetByName = _currentUserService.Username;
                            obj.LastPasswordResetOn = DateTime.Now;
                            obj.ChangePinCount += 1;
                            if (objCustomer != null)
                            {
                                objCustomer.PinNoHash = newPasswordHash;
                                objCustomer.PinNoSalt = newPasswordSaltHash;
                                objCustomer.PinNo = _passwordHash.GetEncryptedPassword(request.Model.NewPin);
                            }


                            if (await _context.SaveChangesAsync(cancellation) > 0)
                            {

                                result.NewPin = request.Model.NewPin;
                                result.MemberId = request.Model.MemberId;
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
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
           
        }
    }
}


