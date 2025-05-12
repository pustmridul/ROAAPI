using MediatR;
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
using ResApp.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text.Json.Serialization;


namespace MemApp.Application.Com.Commands.Login;

public class LoginCommand : IRequest<Result<TokenResponse>>
{
    public string Email { get; set; }
    public string Password { get; set; }
    [JsonIgnore]
    public string IpAddress { get; set; }
    public string AppId { get; set; }
    public string? DeviceToken { get; set; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<TokenResponse>>
{
    private readonly IMemDbContext _context;
    private readonly IPasswordHash _passwordHash;
    private readonly IPasswordNewHash _passwordNewHash;
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;
    private readonly JWTSettings _jwtSettings;

    public LoginCommandHandler(IMemDbContext context, IPasswordHash passwordHash, IPasswordNewHash passwordNewHash
    , IMediator mediator, ITokenService tokenService, IOptions<JWTSettings> jwtSettings)
    {
        _context = context;
        _mediator = mediator;
        _passwordHash = passwordHash;
        _passwordNewHash = passwordNewHash;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<Result<TokenResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = new Result<TokenResponse>();
        var user = await _context.Users
            .FirstOrDefaultAsync(q => (q.EmailId == request.Email /*&& q.AppId == request.AppId*/)
            , cancellationToken);

        if (user == null)
        {
            //result.HasError = true;
            //result.Messages.Add("Invalid user");
            //return result;

            throw new LoginFailedException();
        }

        bool isLoginValid;
        bool isLoginValidOld;

         isLoginValid = _passwordNewHash.ValidatePassword(request.Password, user.PasswordHash, user.PasswordSalt);
         isLoginValidOld = _passwordHash.ValidatePassword(request.Password, user.PasswordHash, user.PasswordSalt);

        if (!isLoginValid)

        {
            if (!isLoginValidOld)
            {
                user.LoginFailedAttemptCount++;
                user.LastLoginFailedAttemptDate = DateTime.Now;

                //throw new LoginFailedException();

                result.HasError = true;
                result.Messages.Add("Invalid User or Password");
                return result;
            }
            if(isLoginValidOld)
            {
                string newPasswordHash = string.Empty;
                string newPasswordSaltHash = string.Empty;

                //_passwordHash.CreateHash(request.Password.ToString(CultureInfo.InvariantCulture), ref newPasswordHash,
                //    ref newPasswordSaltHash);

                //_passwordHash.CreateHash(request.Password, ref newPasswordHash,
                //   ref newPasswordSaltHash);

                _passwordNewHash.CreateHash(request.Password, ref newPasswordHash,
                  ref newPasswordSaltHash);

                // memberNew.Password = _passwordHash.GetEncryptedPassword(request.Password.ToString());
              //  user.Password = _passwordNewHash.GetEncryptedPassword(request.Password.ToString());
                user.PasswordHash = newPasswordHash;
                user.PasswordSalt = newPasswordSaltHash;

                var member= _context.MemberRegistrationInfos.FirstOrDefault(x=>x.Id==user.MemberId);
                if (member != null)
                {
                    member.Password = _passwordNewHash.GetEncryptedPassword(request.Password.ToString());
                    member.PasswordHash = newPasswordHash;
                    member.PasswordSalt = newPasswordSaltHash;

                    _context.MemberRegistrationInfos.Update(member);
                }

                _context.Users.Update(user);
            }
           
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
            MemberId = user.MemberId,
        };


        var jwtSecurityToken = await _tokenService.GenerateJWToken(userProfile, request.IpAddress, request.AppId);
        var response = new Result<TokenResponse>();
        var res = new TokenResponse()
        {
            Id = user.Id.ToString(),
            JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            IssuedOn = jwtSecurityToken.ValidFrom.ToLocalTime(),
            ExpiresOn = jwtSecurityToken.ValidTo.ToLocalTime()
        };
        response.Data = res;


        //response.Email = user.EmailId ?? "";
        //response.UserName = user.UserName;
        var refreshToken = _tokenService.GenerateRefreshToken(request.IpAddress, request.AppId);
        //response.RefreshToken = refreshToken.Token;
        //response.IsVerified = true;
        var refuser = await _context.RefreshTokens
            .FirstOrDefaultAsync(q => q.UserName == user.UserName
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
        refuser.Expires = DateTime.Now.AddMonths(11);

        user.LastLoginDate = DateTime.Now;

        if (!(request.DeviceToken == null || request.DeviceToken == "" || request.DeviceToken == "null"))
        {
            var notificationTokenObj = await _context.NotificationTokens
          .SingleOrDefaultAsync(q => q.UserId == user.Id && q.DeviceToken == request.DeviceToken, cancellationToken);

            if (notificationTokenObj == null)
            {
                notificationTokenObj = new NotificationToken()
                {
                    DeviceToken = request.DeviceToken,
                    UserId = user.Id,
                };
                _context.NotificationTokens.Add(notificationTokenObj);
            }
        }

        _context.Save();

        await _mediator.Send(new CreateUserConferenceCommand()
        {
            Model = new UserConferenceReq()
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserRefToken = refuser.RefToken,
                UserToken = refuser.Token,
                IpAddress = request.IpAddress,
                AppId = request.AppId,

            }
        });

        return response;
    }

    public static string GetHash(string input)
    {
        HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

        byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(input);

        byte[] byteHash = hashAlgorithm.ComputeHash(byteValue);

        return Convert.ToBase64String(byteHash);
    }
}
