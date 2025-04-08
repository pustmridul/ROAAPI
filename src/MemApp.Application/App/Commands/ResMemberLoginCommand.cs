using MediatR;
using MemApp.Application.App.Models;
using MemApp.Application.Com.Commands.UserConferences;
using MemApp.Application.Com.Models;
using MemApp.Application.Exceptions;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models;
using MemApp.Application.Models.DTOs;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.com;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Res.Domain.Entities;
using ResApp.Application.App.Models;
using ResApp.Application.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;


namespace MemApp.Application.App.Commands
{
    public class ResMemberLoginCommand : IRequest<Result<ResMemberLoginDto>>
    {
        //  public MemberLoginReq Model { get; set; } = new MemberLoginReq();

        public string Email { get; set; }
        public string Password { get; set; }
        [JsonIgnore]
        public string IpAddress { get; set; }
        public string AppId { get; set; }
        public string? DeviceToken { get; set; }
    }

    public class ResMemberLoginCommandHandler : IRequestHandler<ResMemberLoginCommand, Result<ResMemberLoginDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IPasswordHash _passwordHash;
        private readonly IMediator _mediator;
        private readonly ITokenService _tokenService;
        private readonly JWTSettings _jwtSettings;

        public ResMemberLoginCommandHandler(IMemDbContext context, IPasswordHash passwordHash, IMediator mediator, ITokenService tokenService, IOptions<JWTSettings> jwtSettings)
        {
            _context = context;
            _mediator = mediator;
            _passwordHash = passwordHash;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<Result<ResMemberLoginDto>> Handle(ResMemberLoginCommand request, CancellationToken cancellationToken)
        {

            var user = await _context.Users
                .FirstOrDefaultAsync(q => (q.EmailId == request.Email) , cancellationToken);

            if (user == null)
                throw new LoginFailedException();

            var member = await _context.MemberRegistrationInfos
              // .Include(i => i.MemberTypes)
             //  .Include(i => i.MemberStatus)
             //  .Include(i => i.MemberActiveStatus)
               .SingleOrDefaultAsync(q => q.Email == request.Email, cancellationToken);
            if (member == null)
                throw new LoginFailedException();

            var isLoginValid = _passwordHash.ValidatePassword(request.Password, user?.PasswordHash ?? "", user?.PasswordSalt ?? "");

            if (!isLoginValid)

            {
                user.LoginFailedAttemptCount++;
                user.LastLoginFailedAttemptDate = DateTime.Now;
                throw new LoginFailedException();

            }
            UserProfile userProfile = new UserProfile()
            {
                Id = user.Id,
                Name = user.Name,
                EmailConfirmed = user.EmailConfirmed,
                EmailId = user.EmailId,
                AppId = user.AppId,
                Roles = await _context.UserRoleMaps.Distinct().Where(q => q.UserId == user.Id && q.IsActive).Select(s => s.RoleId).ToListAsync(cancellationToken),
                PhoneNo = user.PhoneNo ?? "",
                UserName = user.UserName!,
                MemberId = user.MemberInfoId,
            };
            var jwtSecurityToken = await _tokenService.GenerateJWToken(userProfile, request.IpAddress, request.AppId);
            // var response = new MemberLoginVm();
            var response = new Result<ResMemberLoginDto>
            {
                Data=new ResMemberLoginDto()
            };

            response.Data.UserId = user.Id.ToString();
            response.Data.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.Data.IssuedOn = jwtSecurityToken.ValidFrom.ToLocalTime();
            response.Data.ExpiresOn = jwtSecurityToken.ValidTo.ToLocalTime();
            response.Data.Email = user.EmailId! ;
            response.Data.UserName = user.UserName!;
            //var rolesList = await _context.UserRoleMaps.Include(i => i.Role).Where(q => q.UserId == user.Id)
            //    .Select(s => s.Role.Id).ToListAsync(cancellationToken);
            //response.Roles = rolesList.ToList();
            response.Data.IsVerified = user.EmailConfirmed ?? true;
            var refreshToken = _tokenService.GenerateRefreshToken(request.IpAddress, request.AppId);
            response.Data.RefreshToken = refreshToken.Token;

            var refuser = await _context.RefreshTokens
                .SingleOrDefaultAsync(q => q.UserName == user.UserName
                && q.AppId == request.AppId
                && q.UserId == user.Id, cancellationToken);
            if (refuser == null)
            {
                refuser = new RefreshToken()
                {
                    AppId = request.AppId,
                    UserId = user.Id,
                    UserName = user.UserName
                };
                _context.RefreshTokens.Add(refuser);

            }
            refuser.Token = response.Data.JWToken;
            refuser.RefToken = refreshToken.Token;
            refuser.CreatedByIp = request.IpAddress;
            refuser.Expires = DateTime.Now.AddDays(15);

            user.LastLoginDate = DateTime.Now;
            //if (!(request.DeviceToken == null || request.DeviceToken == "" || request.DeviceToken == "null"))
            //{
            //    var notificationTokenObj = await _context.NotificationTokens
            //  .SingleOrDefaultAsync(q => q.UserId == user.Id && q.DeviceToken == request.DeviceToken, cancellationToken);

            //    if (notificationTokenObj == null)
            //    {
            //        notificationTokenObj = new NotificationToken()
            //        {
            //            DeviceToken = request.DeviceToken ?? "",
            //            UserId = user.Id,
            //        };
            //        _context.NotificationTokens.Add(notificationTokenObj);
            //    }
            //}
            _context.Save();
            response.Data.MemberInfo = new MemberRegistrationInfoDto();
            try
            {
                if (member != null)
                {
                    response.Data.MemberInfo.Id = member.Id;
                    response.Data.MemberInfo.IsApproved = member.IsApproved;
                    response.Data.MemberInfo.IsFilled = member.IsFilled;
                    // response.Data.MemberInfo.I = member.IsFilled;
                    response.Data.MemberInfo.ApplicationNo = member.ApplicationNo;
                    response.Data.MemberInfo.MemberShipNo = member.MemberShipNo;
                    response.Data.MemberInfo.ApproveTime = member.ApproveTime;

                    response.Data.MemberInfo.Name = member.Name;
                    response.Data.MemberInfo.NomineeName = member.NomineeName;
                    response.Data.MemberInfo.InstituteNameEnglish = member.InstituteNameEnglish;
                    response.Data.MemberInfo.InstituteNameBengali = member.InstituteNameBengali;

                    response.Data.MemberInfo.PermenantAddress = member.PermenantAddress;
                    response.Data.MemberInfo.DivisionId = member.DivisionId;
                    response.Data.MemberInfo.ApproveTime = member.ApproveTime;

                    response.Data.MemberInfo.DivisionId = member.DivisionId.GetValueOrDefault();
                    response.Data.MemberInfo.DistrictId = member.DistrictId.GetValueOrDefault();
                    response.Data.MemberInfo.ThanaId = member.ThanaId.GetValueOrDefault();

                    response.Data.MemberInfo.DivisionName = _context.Divisions.Where(x => x.Id == member.DivisionId.GetValueOrDefault()).FirstOrDefault()?.EnglishName ?? "";
                    response.Data.MemberInfo.DistrictName = _context.Districts.Where(x => x.Id == member.DistrictId.GetValueOrDefault()).FirstOrDefault()?.EnglishName ?? "";
                    response.Data.MemberInfo.ThanaName = _context.Thanas.Where(x => x.Id == member.ThanaId.GetValueOrDefault()).FirstOrDefault()?.EnglishName ?? "";

                    response.Data.MemberInfo.BusinessStartingDate = member.BusinessStartingDate;
                    response.Data.MemberInfo.MemberNID = member.MemberNID;
                    response.Data.MemberInfo.MemberTINNo = member.MemberTINNo;
                    response.Data.MemberInfo.MemberTradeLicense = member.MemberTradeLicense;

                    response.Data.MemberInfo.NIDImgPath = member.NIDImgPath;
                    response.Data.MemberInfo.SignatureImgPath = member.SignatureImgPath;
                    response.Data.MemberInfo.TinImgPath = member.TinImgPath;
                    response.Data.MemberInfo.TradeLicenseImgPath = member.TradeLicenseImgPath;

                    
                }
            }
            catch (Exception ex)
            {
                throw new LoginFailedException();
            }
            //await _mediator.Send(new CreateUserConferenceCommand()
            //{
            //    Model = new UserConferenceReq()
            //    {
            //        UserId = user.Id,
            //        UserName = user.UserName,
            //        UserRefToken = refuser.RefToken,
            //        UserToken = refuser.Token,
            //        IpAddress = request.Model.IpAddress,
            //        AppId = request.Model.AppId,

            //    }
            //});
            return response;
        }

       
    }

}
