using MediatR;
using MemApp.Application.Com.Commands.UserConferences;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.com;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;

namespace MemApp.Application.Com.Commands.Login;

public class RefreshTokenCommand : IRequest<TokenResponse>
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string? DeviceToken { get; set; }
    public string? AppId { get; set; }
    [JsonIgnore]
    public string? IpAddress { get; set; }
}
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponse>
{
    private readonly IMemDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IMediator _mediator;
    public RefreshTokenCommandHandler(IMemDbContext context, ITokenService tokenService, IMediator mediator)
    {
        _context = context;
        _tokenService = tokenService;
        _mediator = mediator;
    }
    public async Task<TokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {


        string accessToken = request.AccessToken;
        string refreshToken = request.RefreshToken;


        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);

        var a = principal.Claims.ToList();



        var username = a[4].Value;
        var userId = Convert.ToInt32(a[3].Value);

        var refuser = await _context.RefreshTokens
            .SingleOrDefaultAsync(q => q.UserName == username 
            && q.UserId == userId 
            && q.AppId == request.AppId, cancellationToken);

        var user = await _context.Users.SingleOrDefaultAsync(q => q.Id == userId && q.UserName == username, cancellationToken);



        if (refuser is null || refuser.RefToken != refreshToken || refuser.Expires <= DateTime.Now)
            return BadRequestException("Invalid client request");

        UserProfile userProfile = new UserProfile()
        {
            Id = user.Id,
            Name = user.Name,
            UserName = user.UserName,
            EmailConfirmed = user.EmailConfirmed,
            EmailId = user.EmailId,
            AppId = user.AppId,
            Roles = await _context.UserRoleMaps.Distinct().Where(q => q.UserId == user.Id && q.IsActive).Select(s => s.RoleId).ToListAsync(cancellationToken),
            PhoneNo = user.PhoneNo ?? "",
            MemberId = user.MemberId,
        };

        var jwtSecurityToken = await _tokenService.GenerateJWToken(userProfile, request.IpAddress, request.AppId);

        var response = new TokenResponse();

        response.Id = user.Id.ToString();
        response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        response.IssuedOn = jwtSecurityToken.ValidFrom.ToLocalTime();
        response.ExpiresOn = jwtSecurityToken.ValidTo.ToLocalTime();
        response.Email = user.EmailId ?? "";
        response.UserName = user.UserName;
        //var rolesList = await _context.UserRoleMaps.Include(i => i.Role).Where(q => q.UserId == user.Id)
        //    .Select(s => s.Role.Name).ToListAsync(cancellationToken);
        //response.Roles = rolesList.ToList();
        response.IsVerified = user.EmailConfirmed ?? true;
        var newRefreshToken = _tokenService.GenerateRefreshToken(request.IpAddress, request.AppId);
        response.RefreshToken = newRefreshToken.Token;


        var obj = await _context.RefreshTokens
           .SingleOrDefaultAsync(q => q.UserName == user.UserName
           && q.AppId == request.AppId
           && q.UserId == user.Id, cancellationToken);
        if (obj == null)
        {
            obj = new RefreshToken()
            {
                AppId = request.AppId,
                UserId = user.Id,
                UserName = user.UserName
            };
            _context.RefreshTokens.Add(obj);

        }
        obj.Token = response.JWToken;
        obj.RefToken = newRefreshToken.Token;
        obj.CreatedByIp = request.IpAddress;
        obj.Expires = DateTime.Now.AddYears(15);

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
        if (_context.Save() > 0)
        {
            await _mediator.Send(new CreateUserConferenceCommand()
            {
                Model = new Models.UserConferenceReq()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserToken = obj.Token,
                    UserRefToken = obj.RefToken,
                    IpAddress = request.IpAddress,
                    AppId = request.AppId,
                    LogInDate = DateTime.Now,
                    LogOutStatus = false
                }
            });
        }

        return response;
    }

    private TokenResponse BadRequestException(string v)
    {
        throw new Exception(v);
    }
}
