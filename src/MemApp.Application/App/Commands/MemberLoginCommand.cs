using MediatR;
using MemApp.Application.App.Models;
using MemApp.Application.Com.Commands.UserConferences;
using MemApp.Application.Com.Models;
using MemApp.Application.Exceptions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models;
using MemApp.Application.Models.DTOs;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.com;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;


namespace MemApp.Application.App.Commands
{
    public class MemberLoginCommand : IRequest<MemberLoginVm>
    {
        public MemberLoginReq Model { get; set; } = new MemberLoginReq();
    }

    public class MemberLoginCommandHandler : IRequestHandler<MemberLoginCommand, MemberLoginVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPasswordHash _passwordHash;
        private readonly IMediator _mediator;
        private readonly ITokenService _tokenService;
        private readonly JWTSettings _jwtSettings;

        public MemberLoginCommandHandler(IMemDbContext context, IPasswordHash passwordHash, IMediator mediator, ITokenService tokenService, IOptions<JWTSettings> jwtSettings)
        {
            _context = context;
            _mediator = mediator;
            _passwordHash = passwordHash;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<MemberLoginVm> Handle(MemberLoginCommand request, CancellationToken cancellationToken)
        {

            var user = await _context.Users
                .FirstOrDefaultAsync(q => (q.UserName == request.Model.CardNo)
                , cancellationToken);

            if (user == null)
                throw new LoginFailedException();

            var member = await _context.RegisterMembers
               .Include(i => i.MemberTypes)
               .Include(i => i.MemberStatus)
               .Include(i => i.MemberActiveStatus)
               .SingleOrDefaultAsync(q => q.CardNo == request.Model.CardNo, cancellationToken);
            if (member == null)
                throw new LoginFailedException();

            var isLoginValid = _passwordHash.ValidatePassword(request.Model.PinNo, member?.PinNoHash ?? "", member?.PinNoSalt ?? "");

            if (!isLoginValid)

            {
                user.LoginFailedAttemptCount++;
                user.LastLoginFailedAttemptDate = DateTime.Now;
                throw new LoginFailedException();

            }
            UserProfile userProfile = new()
            {
                Id = user.Id,
                Name = user.Name,
                EmailConfirmed = user.EmailConfirmed,
                EmailId = user.EmailId,
                AppId = user.AppId,
                Roles = await _context.UserRoleMaps.Distinct().Where(q => q.UserId == user.Id && q.IsActive).Select(s => s.RoleId).ToListAsync(cancellationToken),
                PhoneNo = user.PhoneNo ?? "",
                UserName = user.UserName!,
                MemberId = user.MemberId,
            };
            var jwtSecurityToken = await _tokenService.GenerateJWToken(userProfile, request.Model.IpAddress, request.Model.AppId);
            var response = new MemberLoginVm();

            response.UserId = user.Id.ToString();
            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.IssuedOn = jwtSecurityToken.ValidFrom.ToLocalTime();
            response.ExpiresOn = jwtSecurityToken.ValidTo.ToLocalTime();
            response.Email = user.EmailId ?? "";
            response.UserName = user.UserName!;
            //var rolesList = await _context.UserRoleMaps.Include(i => i.Role).Where(q => q.UserId == user.Id)
            //    .Select(s => s.Role.Id).ToListAsync(cancellationToken);
            //response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed ?? true;
            var refreshToken = _tokenService.GenerateRefreshToken(request.Model.IpAddress, request.Model.AppId);
            response.RefreshToken = refreshToken.Token;

            var refuser = await _context.RefreshTokens
                .SingleOrDefaultAsync(q => q.UserName == user.UserName
                && q.AppId == request.Model.AppId
                && q.UserId == user.Id, cancellationToken);
            if (refuser == null)
            {
                refuser = new RefreshToken()
                {
                    AppId = request.Model.AppId,
                    UserId = user.Id,
                    UserName = user.UserName
                };
                _context.RefreshTokens.Add(refuser);

            }
            refuser.Token = response.JWToken;
            refuser.RefToken = refreshToken.Token;
            refuser.CreatedByIp = request.Model.IpAddress;
            refuser.Expires = DateTime.Now.AddDays(15);

            user.LastLoginDate = DateTime.Now;
            if (!(request.Model.DeviceToken == null || request.Model.DeviceToken == "" || request.Model.DeviceToken == "null"))
            {
                var notificationTokenObj = await _context.NotificationTokens
              .SingleOrDefaultAsync(q => q.UserId == user.Id && q.DeviceToken == request.Model.DeviceToken, cancellationToken);

                if (notificationTokenObj == null)
                {
                    notificationTokenObj = new NotificationToken()
                    {
                        DeviceToken = request.Model.DeviceToken ?? "",
                        UserId = user.Id,
                    };
                    _context.NotificationTokens.Add(notificationTokenObj);
                }
            }
            _context.Save();


            if (member != null)
            {
                response.Data.ImgFileUrl = member?.ImgFileUrl ?? "";
                response.Data.MemberId = member?.Id ?? 0;
                response.Data.Organaization = member?.Organaization ?? "";
                response.Data.Specialaization = member?.Specialization ?? "";
                response.Data.Dob = member?.Dob == null ? "" : member.Dob.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                response.Data.MembershipNo = member?.MembershipNo ?? "";
                response.Data.IsMasterMember = member?.IsMasterMember;
                response.Data.Phone = member?.Phone ?? "";
                response.Data.MemberTypeId = member?.MemberTypeId ?? 0;
                response.Data.MemberTypeText = member?.MemberTypes.Name ?? "";
                response.Data.MemberStatusId = member?.MemberStatusId ?? 0;
                response.Data.MemberStatusText = member?.MemberStatus?.Name ?? "";
                response.Data.MemberActiveStatusId = member?.MemberActiveStatusId ?? 0;
                response.Data.MemberActiveStatusText = member?.MemberActiveStatus?.Name ?? "";
                response.Data.OfficeAddress = member?.OfficeAddress ?? "";
                response.Data.HomeAddress = member?.HomeAddress ?? "";
                response.Data.FullName = member?.FullName ?? "";
                response.Data.CurrentBalance = await _context.MemLedgers
                    .Where(q => q.PrvCusID == (member != null ? member.Id.ToString() : "")).SumAsync(s => s.Amount) ?? 0;
                response.Data.CardNo = member?.CardNo ?? "";
                response.Data.PaidTill = member?.PaidTill == null ? "" : member.PaidTill.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                response.Data.HasSubscription = member?.MemberTypes.IsSubscribed ?? false;
            }
            await _mediator.Send(new CreateUserConferenceCommand()
            {
                Model = new UserConferenceReq()
                {
                    UserId = user.Id,
                    UserName = user.UserName!,
                    UserRefToken = refuser.RefToken,
                    UserToken = refuser.Token,
                    IpAddress = request.Model.IpAddress,
                    AppId = request.Model.AppId,

                }
            });
            return response;
        }

        //public static string GetHash(string input)
        //{
        //    HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

        //    byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

        //    byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

        //    return Convert.ToBase64String(byteHash);
        //}
    }

}
