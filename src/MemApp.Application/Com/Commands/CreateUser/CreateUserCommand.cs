using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Domain.Entities;
using MemApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Res.Domain.Entities;
using System.Globalization;

namespace MemApp.Application.Com.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<Result<UserDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
      //  public bool EmailConfirmed { get; set; }
       // public string PasswordHash { get; set; } = string.Empty;
      //  public string PasswordSalt { get; set; } = string.Empty;
       // public string OldPassword { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
     //   public string ConfirmPassword { get; set; } = string.Empty;

     //   public int LoginFailedAttemptCount { get; set; }
      //  public DateTime? LastLoginFailedAttemptDate { get; set; }
    //    public DateTime? LastLoginDate { get; set; }
     //   public UserStatus Status { get; set; }
        public string PhoneNo { get; set; } = string.Empty;
     //   public bool IsActive { get; set; }
        public string AppId { get; set; } = string.Empty;

        public string? CompanyName { get; set; }
        public string? TradeLicense { get; set; }
        public string? UserNID { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly IPasswordHash _passwordHash;
        private readonly IPermissionHandler _permissionHandler;
        public CreateUserCommandHandler(IMemDbContext context, IMediator mediator, IPasswordHash passwordHash, IPermissionHandler permissionHandler)
        {
            _context = context;
            _mediator = mediator;
            _passwordHash = passwordHash;
            _permissionHandler = permissionHandler;
        }
        public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellation)
        {
            var result = new Result<UserDto>();

            var checkUserNameExist= await _context.MemberRegistrationInfos
                .AnyAsync(q => q.UserName == request.UserName );

            var checkEmailExist = await _context.MemberRegistrationInfos
               .AnyAsync(q => q.Email == request.EmailId);

            var checkUserNIDExist = await _context.MemberRegistrationInfos
              .AnyAsync(q => q.MemberNID == request.UserNID);

            if (checkUserNIDExist)
            {
                result.HasError = true;
                result.Messages.Add("NID is already exist!!!");
                return result;
            }

            //if (checkUserNameExist)
            //{
            //    result.HasError = true;
            //    result.Messages.Add("Username exist!!!");
            //    return result;
            //}

            if (checkEmailExist)
            {
                result.HasError = true;
                result.Messages.Add("Email exist!!!");
                return result;
            }

            //var obj = await _context.Users
            //    .SingleOrDefaultAsync(q => q.Id == request.Id);
            if (!checkUserNameExist  && !checkEmailExist)
            {

                var memberNew = new MemberRegistrationInfo
                {
                    Name = request.Name,
                    Email=request.EmailId,
                    UserName = request.UserName,
                    InstituteNameEnglish = request.CompanyName,
                    MemberTradeLicense = request.TradeLicense,
                    MemberNID = request.UserNID,
                    IsActive=true,
                    IsApproved=false,
                    IsFilled=false,
                    PhoneNo=request.PhoneNo,
                  //  MemberShipNo=GenerateMembershipNo()
                };


               

                string newPasswordHash = string.Empty;
                string newPasswordSaltHash = string.Empty;

                //_passwordHash.CreateHash(request.Password.ToString(CultureInfo.InvariantCulture), ref newPasswordHash,
                //    ref newPasswordSaltHash);

                _passwordHash.CreateHash(request.Password, ref newPasswordHash,
                   ref newPasswordSaltHash);

                memberNew.Password = _passwordHash.GetEncryptedPassword(request.Password.ToString());
                memberNew.PasswordHash = newPasswordHash;
                memberNew.PasswordSalt = newPasswordSaltHash;

                _context.MemberRegistrationInfos.Add(memberNew);

                if (await _context.SaveChangesAsync(cancellation) > 0)
                {

                    var user = new User();
                    user.IsActive = true;

                    user.IsActive = true;

                    user.PasswordHash = newPasswordHash;
                    user.PasswordSalt = newPasswordSaltHash;
                    user.EmailConfirmed = true;


                    user.Name = request.Name;
                    user.PhoneNo = request.PhoneNo;
                    user.UserName = request.UserName;
                    user.EmailId = request.EmailId;
                    user.AppId = request.AppId;
                    user.MemberInfoId=memberNew.Id;

                  //  user.CompanyName = request.CompanyName;
                 //   user.UserNID = request.UserNID;
                 //   user.TradeLicense = request.TradeLicense;
                    _context.Users.Add(user);
                }


                
                if (await _context.SaveChangesAsync(cancellation) > 0)
                {

                    //result.Data.Name = request.Name;
                    //result.Data.UserName = request.UserName;
                    //result.Data.EmailId = request.EmailId;
                    //result.Data.PhoneNo = request.PhoneNo;
                    //result.Data.Id = user.Id;

                    result.HasError = false;
                    result.Messages.Add("User Created");

                    return result;
                }
                else
                {
                    result.HasError = true;
                    result.Messages.Add("something wrong");
                }
            }
            else
            {
                result.HasError = true;
                result.Messages.Add("Username or Email exist!!!");
            }
            //if (obj == null)
            //{
            //    //if(!await _permissionHandler.HasRolePermissionAsync(1001))
            //    //{
            //    //    result.HasError = true;
            //    //    result.Messages.Add("User Create Permission Denied!!");
            //    //    return result;
            //    //}
            //    obj = new User();
            //    obj.IsActive = true;
            //    _context.Users.Add(obj);
            //    result.HasError = false;
               
            //    var  newPassword = new Random(DateTime.Now.Millisecond).Next(1000, 9999);

            //    string newPasswordHash = string.Empty;
            //    string newPasswordSaltHash = string.Empty;

            //    _passwordHash.CreateHash(newPassword.ToString(CultureInfo.InvariantCulture), ref newPasswordHash,
            //        ref newPasswordSaltHash);
            //    obj.PasswordHash = newPasswordHash;
            //    obj.PasswordSalt = newPasswordSaltHash;
            //    obj.EmailConfirmed = true;
               
            //    result.Messages.Add("New User created Password is :  " + newPassword);
            //}
            //else
            //{
            //    //if (!await _permissionHandler.HasRolePermissionAsync(1002))
            //    //{
            //    //    result.HasError = true;
            //    //    result.Messages.Add("User Update Permission Denied!!");
            //    //    return result;
            //    //}
            //    result.HasError = false;
            //    result.Messages.Add("User Updated");
            //}

            //obj.Name = request.Name;
            //obj.PhoneNo = request.PhoneNo;
            //obj.UserName = request.UserName;
            //obj.EmailId = request.EmailId;
            //obj.AppId = request.AppId;


            //if (await _context.SaveChangesAsync(cancellation) > 0)
            //{

            //    result.Data.Name = obj.Name;
            //    result.Data.UserName = obj.UserName;
            //    result.Data.EmailId = obj.EmailId;
            //    result.Data.PhoneNo= obj.PhoneNo;
            //    result.Data.Id = obj.Id;
            //}
            //else
            //{
            //    result.HasError = true;
            //    result.Messages.Add("something wrong");
            //}
            return result;
        }

        public string GenerateMembershipNo()
        {
           // string initials = new string(fullName.Split(' ').Select(w => w[0]).ToArray()).ToUpper();
           
            string datePart = DateTime.Now.Year.ToString();
            int count = _context.MemberRegistrationInfos.Count(m => m.MemberShipNo!.StartsWith(datePart)) + 1;
            int randomNum = new Random().Next(1000, 9999);
            return $"MEM-{datePart}-{count}-{Guid.NewGuid().ToString("N").Substring(8)}"; // Example: "JS-240304-4567"
        }
    }
}
