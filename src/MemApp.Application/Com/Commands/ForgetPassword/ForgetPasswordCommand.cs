using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using MemApp.Application.App.Models;
using System.Security.Cryptography;

namespace MemApp.Application.Com.Commands.ChangedPassword
{

    public class ForgetPasswordCommand : IRequest<ForgetPasswordRes>
    {
        public ForgetPasswordReq Model { get; set; } = new ForgetPasswordReq();
    }

    public class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, ForgetPasswordRes>
    {
        private readonly IMemDbContext _context;
        private readonly IBroadcastHandler _broadcastHandler;
        public ForgetPasswordCommandHandler(IMemDbContext context, IBroadcastHandler broadcastHandler)
        {
            _context = context;
            _broadcastHandler = broadcastHandler;
        }
        public async Task<ForgetPasswordRes> Handle(ForgetPasswordCommand request, CancellationToken cancellation)
        {
            try
            {
                var result = new ForgetPasswordRes();
                bool optSendSms = false;
                bool otpSendEmail =false;
                if(request.Model != null)
                {
                    var obj = await _context.Users.SingleOrDefaultAsync(q => q.MemberId == request.Model.MemberId, cancellation);
                    if(obj == null)
                    {
                        throw new Exception("User Not Found");
                    }
                    else
                    {
                        var otp = GenerateOTP();
                        string message = "Member, " + otp.ToString() + " is your One Time Password (OTP) to create new password into your account which is valid for next 5 minutes";
                       


                        if (!(request.Model.PhoneNo==null || request.Model.PhoneNo.Length == 0))
                        {
                            // make otp sms
                          optSendSms=  await _broadcastHandler.SendSms(request.Model.PhoneNo, message, "English");
                        }
                        
                        if(!(request.Model.Email==null || request.Model.Email.Length == 0))
                        {
                            // make otp email
                          otpSendEmail=  await _broadcastHandler.SendEmail(request.Model.Email, "OTP (Cadet College Club Ltd)", message);
                        }

                        if(optSendSms || otpSendEmail)
                        {
                            obj.Otp = otp;
                            obj.OtpCreatedTime = DateTime.Now;
                            await _context.SaveChangesAsync(cancellation);
                            result.Otp = otp;
                            result.OtpCreatedTime = obj.OtpCreatedTime;
                            result.MemberId = request.Model.MemberId;
                        }
                       
                    }
                }

                return result;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static string GenerateOTP()
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] data = new byte[4];
            rng.GetBytes(data);
            int otpValue = BitConverter.ToInt32(data, 0) % 1000000;
            if (otpValue < 0)
            {
                otpValue = otpValue * -1;
            }
            return otpValue.ToString("D6"); // Ensure 6 digits
        }

    }
}


