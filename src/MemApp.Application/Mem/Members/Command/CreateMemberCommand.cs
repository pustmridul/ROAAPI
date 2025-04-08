using AutoMapper;
using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using MemApp.Application.Mem.MemberStatuss.Queries;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace MemApp.Application.Mem.Members.Command
{
    public class CreateMemberCommand : IRequest<RegisterMemberVm>
    {
        public RegisterMemberRes Model { get; set; } = new RegisterMemberRes();
    }

    public class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, RegisterMemberVm>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IMemDbContext _context;
        private readonly IPasswordHash _passwordHash;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserLogService _userLogService;
        private readonly IPermissionHandler _permissionHandler;
        private readonly IMemoryCache _memoryCache;

        public CreateMemberCommandHandler(IMediator mediator, IMapper mapper, IMemDbContext context,
            IPasswordHash passwordHash, ICurrentUserService currentUserService, IUserLogService userLogService,
            IPermissionHandler permissionHandler, IMemoryCache memoryCache)
        {
            _mediator = mediator;
            _mapper = mapper;
            _context = context;
            _passwordHash = passwordHash;
            _currentUserService = currentUserService;
            _userLogService = userLogService;
            _permissionHandler = permissionHandler;
            _memoryCache = memoryCache;
        }
        public async Task<RegisterMemberVm> Handle(CreateMemberCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new RegisterMemberVm();
            try
            {
                if (request.Model.Id == 0)
                {
                    var isExistingMember = await _context.RegisterMembers
                   .Where(q => (q.MembershipNo == request.Model.MemberShipNo || q.CardNo == request.Model.CardNo) && q.IsActive)
                   .AsNoTracking()
                   .ToListAsync(cancellation);


                    var memNoExist = isExistingMember.Where(q => q.MembershipNo == request.Model.MemberShipNo && q.IsMasterMember == true).ToList();
                    var memCardExist = isExistingMember.Where(q => q.CardNo == request.Model.CardNo).ToList();


                    if (memNoExist.Count > 0)
                    {
                        result.HasError = true;
                        result.Messages.Add("This member ship no is already exist.");
                        return result;
                    }
                    if (memCardExist.Count > 0)
                    {
                        result.HasError = true;
                        result.Messages.Add("This member Card no is already exist.");
                        return result;
                    }

                }

                var memTypes = await _context.MemberTypes.AsNoTracking()
                    .SingleOrDefaultAsync(q => q.Id == request.Model.MemberTypeId, cancellation);

                string newPasswordHash = string.Empty;
                string newPasswordSaltHash = string.Empty;
                var obj = await _context.RegisterMembers
                              .SingleOrDefaultAsync(q => q.Id == request.Model.Id, cancellation);

                if (obj == null)
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(2401))
                    {
                        result.HasError = true;
                        result.Messages.Add("Permission Denied When New Member Registration!");
                        return result;
                    }
                    obj = new RegisterMember();
                    obj.IsActive = true;
                    obj.CurrentBalance = 0;

                    var newPin = request.Model.PinNo ?? new Random(DateTime.Now.Millisecond).Next(1000, 9999).ToString();


                    _passwordHash.CreateHash(newPin.ToString(CultureInfo.InvariantCulture), ref newPasswordHash,
                        ref newPasswordSaltHash);
                    obj.PinNoHash = newPasswordHash;
                    obj.PinNoSalt = newPasswordSaltHash;
                    obj.PinNo = _passwordHash.GetEncryptedPassword(newPin.ToString());
                    obj.CurrentBalance = 0;
                    obj.IsMasterMember = true;

                    obj.MembershipNo = request.Model.MemberShipNo;

                    if (memTypes != null)
                    {
                        if (!memTypes.IsSubscribed)
                        {
                            obj.PaidTill = DateTime.Now.AddYears(100);
                        }
                        else
                        {
                            obj.PaidTill = (request.Model.PaidTill == null || request.Model.PaidTill == "") ? null : DateTime.Parse(request.Model.PaidTill);
                        }
                    }
                    else
                    {
                        obj.PaidTill = (request.Model.PaidTill == null || request.Model.PaidTill == "") ? null : DateTime.Parse(request.Model.PaidTill);
                    }

                    _context.RegisterMembers.Add(obj);

                    result.HasError = false;
                    result.Messages?.Add("New Member created and Pin No is" + newPin);

                }
                else
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(2402))
                    {
                        result.HasError = true;
                        result.Messages?.Add("Permission Denied When Member Update!!");
                        return result;
                    }

                    if (obj.MemberActiveStatusId != request.Model.MemberActiveStatusId)
                    {
                        obj.ActiveStatusDate = DateTime.Now;
                    }

                    result.HasError = false;
                    result.Messages?.Add("Member Updated");
                }

                obj.PaidTill = (request.Model.PaidTill == null || request.Model.PaidTill == "") ? null : DateTime.Parse(request.Model.PaidTill);

                obj.Dob = (request.Model.Dob == null || request.Model.Dob == "") ? null : DateTime.Parse(request.Model.Dob);
                obj.CadetName = request.Model.CadetName;
                obj.FullName = request.Model.FullName ?? "";
                obj.Phone = request.Model.Phone;
                obj.Email = request.Model.Email;
                obj.Address = request.Model.PostalAddress;
                obj.MemberProfessionId = request.Model.MemberProfessionId;
                obj.Organaization = request.Model.Organaization;
                obj.Designation = request.Model.Designation;
                obj.Specialization = request.Model.Specialization;
                obj.BloodGroupId = request.Model.BloodGroupId;
                obj.HscYear = request.Model.HscYear;
                obj.HomeAddress = request.Model.HomeAddress;
                obj.OfficeAddress = request.Model.OfficeAddress;
                obj.CadetNo = request.Model.CadetNo;
                obj.CollegeId = request.Model.CollegeId;
                obj.BatchNo = request.Model.BatchNo;
                obj.MemberTypeId = request.Model.MemberTypeId;
                obj.CreditLimit = request.Model.CreditLimit;
                obj.QBCusName = request.Model.QBCusName;



                obj.MemberActiveStatusId = request.Model.MemberActiveStatusId;
                obj.MemberStatusId = request.Model.MemberStatusId;
                obj.MemberFullId = request.Model.MemberFullId;
                obj.CardNo = request.Model.CardNo;
                //  obj.PaidTill = request.Model.PaidTill;                    
                obj.Nok = request.Model.Nok;
                if (obj.DeviceId == null || obj.DeviceId == "")
                {
                    obj.DeviceId = request.Model.DeviceId;

                }

                obj.PrvCusID = obj.Id.ToString();
                obj.CusCategory = memTypes?.Name;
                obj.Active = "YES";
                obj.ClubName = "CCCL";
                obj.CusName = request.Model.FullName;
                obj.Opening = DateTime.Now;

                obj.JoinDate = (request.Model.JoinDate == null || request.Model.JoinDate == "") ? null : DateTime.Parse(request.Model.JoinDate);
                obj.LeaveDate = (request.Model.LeaveDate == null || request.Model.LeaveDate == "") ? null : DateTime.Parse(request.Model.LeaveDate);
                obj.ClubJoinDate = (request.Model.ClubJoinDate == null || request.Model.ClubJoinDate == "") ? null : DateTime.Parse(request.Model.ClubJoinDate);

                obj.PermanentAddress = request.Model.PermanentAddress;
                obj.NID = request.Model.NID;
                obj.EmergencyContact = request.Model.EmergencyContact;

                if (await _context.SaveChangesAsync(cancellation) > 0)
                {
                    obj.PrvCusID = obj.Id.ToString();

                    if (obj.Id > 0)
                    {
                        var userObjList = await _context.Users.Where(q => q.MemberId == obj.Id).ToListAsync();
                        if (userObjList.Count > 1)
                        {
                            var userObj = userObjList.FirstOrDefault(q => q.UserName == obj.CardNo);

                            if (userObj == null)
                            {
                                userObj = new User();
                                userObj.MemberId = obj.Id;
                                userObj.EmailConfirmed = true;
                                userObj.PhoneNo = request.Model.Phone;
                                userObj.PasswordSalt = newPasswordSaltHash;
                                userObj.PasswordHash = newPasswordHash;
                                userObj.AppId = "WEBAPP";
                                userObj.IsActive = true;
                                userObj.LoginFailedAttemptCount = 0;
                                _context.Users.Add(userObj);
                            }
                            userObj.Name = request.Model.FullName;
                            userObj.UserName = request.Model.CardNo ?? "";
                            userObj.EmailId = request.Model.Email;
                        }
                        else
                        {
                            var userObj = userObjList.FirstOrDefault();
                            if (userObj == null)
                            {
                                userObj = new User();
                                userObj.MemberId = obj.Id;
                                userObj.EmailConfirmed = true;
                                userObj.PhoneNo = request.Model.Phone;
                                userObj.PasswordSalt = newPasswordSaltHash;
                                userObj.PasswordHash = newPasswordHash;
                                userObj.AppId = "WEBAPP";
                                userObj.IsActive = true;
                                userObj.LoginFailedAttemptCount = 0;
                                _context.Users.Add(userObj);
                            }
                            userObj.Name = request.Model.FullName;
                            userObj.UserName = request.Model.CardNo ?? "";
                            userObj.EmailId = request.Model.Email;
                        }
                    }

                    var homeAddress = await _context.MemberAddresses.SingleOrDefaultAsync(q => q.MemberId == obj.Id && q.Type == "Home");
                    if (homeAddress == null)
                    {
                        homeAddress = new MemberAddress()
                        {
                            Type = "Home",
                            IsActive = true,
                            MemberId = obj.Id,

                        };
                        _context.MemberAddresses.Add(homeAddress);
                    }
                    homeAddress.Title = obj.HomeAddress ?? "";
                    homeAddress.Description = obj.HomeAddress;

                    var officeAddress = await _context.MemberAddresses.SingleOrDefaultAsync(q => q.MemberId == obj.Id && q.Type == "Office");
                    if (officeAddress == null)
                    {
                        officeAddress = new MemberAddress()
                        {
                            Type = "Office",
                            IsActive = true,
                            MemberId = obj.Id,

                        };
                        _context.MemberAddresses.Add(officeAddress);
                    }
                    officeAddress.Title = obj.HomeAddress ?? "";
                    officeAddress.Description = obj.HomeAddress;

                    await _context.SaveChangesAsync(cancellation);

                    result.Data.Id = obj.Id;
                }
                else
                {
                    result.HasError = true;
                    result.Messages?.Add("something wrong");
                }

                //    _memoryCache.Set(StaticData.CacheKey.GetMemberList, _mapper.Map<MemberRes>(obj), StaticData.CacheKey.cacheEntryOptions);

                return await _mediator.Send(new GetMemberByIdQuery()
                {
                    Id = obj.Id
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
