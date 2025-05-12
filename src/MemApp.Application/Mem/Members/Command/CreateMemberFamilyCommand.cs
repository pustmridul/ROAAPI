using AutoMapper;
using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using MemApp.Application.Mem.MemberStatuss.Queries;
using MemApp.Application.Services;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Interfaces;
using System.Globalization;

namespace MemApp.Application.Mem.Members.Command
{

    public class CreateMemberFamilyCommand : IRequest<RegisterMemberVm>
    {
        public MemberFamilyReq Model { get; set; } = new MemberFamilyReq();
    }

    public class CreateMemberFamilyCommandHandler : IRequestHandler<CreateMemberFamilyCommand, RegisterMemberVm>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPasswordNewHash _passwordHash;

        public CreateMemberFamilyCommandHandler(
            IMediator mediator,
            IMapper mapper, 
            IMemDbContext context, 
            ICurrentUserService currentUserService, 
            IPasswordNewHash passwordHash)
        {
            _mediator = mediator;
            _mapper = mapper;
            _context = context;
            _currentUserService = currentUserService;
            _passwordHash = passwordHash;
        }
        public async Task<RegisterMemberVm> Handle(CreateMemberFamilyCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new RegisterMemberVm();
            try
            {
               
                var obj = await _context.RegisterMembers
                    .SingleOrDefaultAsync(q => q.Id == request.Model.MemberId, cancellation);
                if (obj == null)
                {
                    result.HasError = true;
                    result.Messages.Add("Member Not Found");
                }
                else
                {
                    var memTypes = await _context.MemberTypes.SingleOrDefaultAsync(q => q.Id == obj.MemberTypeId, cancellation);

                    string newPasswordHash = string.Empty;
                    string newPasswordSaltHash = string.Empty;

                    obj.Spouse = request.Model.Spouse;
                    obj.SpouseOccupation= request.Model.SpouseOccupation;
                    obj.Anniversary= (request.Model.Anniversary == null || request.Model.Anniversary == "") ? null : DateTime.Parse(request.Model.Anniversary); 
                    obj.Nok= request.Model.Nok;

                    var spouseObj = await _context.RegisterMembers.SingleOrDefaultAsync(q => q.MemberId == obj.Id && q.IsMasterMember==false && q.IsActive);
                    if (spouseObj == null)
                    {
                        spouseObj = obj;
                        spouseObj.IsMasterMember = false;
                        spouseObj.MemberId = obj.Id;
                        spouseObj.Id = 0;
                        var newPin = new Random(DateTime.Now.Millisecond).Next(1000, 9999);


                        _passwordHash.CreateHash(newPin.ToString(CultureInfo.InvariantCulture), ref newPasswordHash,
                        ref newPasswordSaltHash);
                        spouseObj.PinNoHash = newPasswordHash;
                        spouseObj.PinNoSalt = newPasswordSaltHash;
                        spouseObj.PinNo = _passwordHash.GetEncryptedPassword(newPin.ToString());

                        _context.RegisterMembers.Add(spouseObj);
                    }
                    spouseObj.FullName = request.Model.Spouse ?? obj.FullName;

                    spouseObj.CardNo = request.Model.CardNo;
                    spouseObj.Spouse= request.Model.Spouse;
                    spouseObj.SpouseOccupation = request.Model.SpouseOccupation;
                    spouseObj.Anniversary = (request.Model.Anniversary == null || request.Model.Anniversary == "") ? null : DateTime.Parse(request.Model.Anniversary);
                    spouseObj.Nok= request.Model.Nok;

                    spouseObj.PrvCusID = spouseObj.Id.ToString();
                    spouseObj.CusCategory = memTypes?.Name;
                    spouseObj.Active = "YES";
                    spouseObj.ClubName = "CCCL";
                    spouseObj.CusName = request.Model.Spouse;
                    spouseObj.Opening = DateTime.Now;


                    var objList = await _context.MemberChildrens.Where(q => q.RegisterMemberId == request.Model.MemberId && q.IsActive).ToListAsync(cancellation);


                    foreach (var od in objList)
                    {
                        var has = request.Model.MemberchildrenReqs.Any(q => q.Id == od.Id);
                        if (!has)
                        {
                            od.IsActive = false;
                        }
                    }
                    foreach (var f in request.Model.MemberchildrenReqs)
                    {
                        var exObj = objList.SingleOrDefault(q => q.Id == f.Id);
                        if (exObj == null)
                        {
                            exObj = new MemberChildren
                            {
                                CadetNo = obj.CadetNo ?? "" ,
                                ContactName = f.ContactName,
                                Dob = (f.Dob == null || f.Dob == "") ? null : DateTime.Parse(f.Dob),
                                Email = f.Email,
                                Gender = f.Gender,
                                RegisterMemberId = request.Model.MemberId,
                                Phone= f.Phone,
                                IsActive = true
                            };
                            _context.MemberChildrens.Add(exObj);
                        }
                        else
                        {
                            exObj.CadetNo = obj.CadetNo ?? "";
                            exObj.ContactName = f.ContactName;
                            exObj.Dob = (f.Dob == null || f.Dob == "") ? null : DateTime.Parse(f.Dob);
                            exObj.Email = f.Email;
                            exObj.Gender = f.Gender;
                            exObj.Phone = f.Phone;
                            exObj.RegisterMemberId = request.Model.MemberId;
                        }
                    }

                    if (await _context.SaveChangesAsync(cancellation) > 0)
                    {
                        spouseObj.PrvCusID = spouseObj.Id.ToString();
                        if (spouseObj.Id > 0)
                        {
                            // var userObj= new User();

                            //  var existUObj = await _context.RegisterMembers.SingleOrDefaultAsync(q=>q.Email== request.Model.Email,cancellation);
                            var userObjList = await _context.Users.Where(q => q.MemberId == spouseObj.Id).ToListAsync();
                            if (userObjList.Count > 1)
                            {
                                var userObj = userObjList.FirstOrDefault(q => q.UserName == spouseObj.CardNo);

                                if (userObj == null)
                                {
                                    userObj = new User();
                                    userObj.MemberId = obj.Id;
                                    userObj.EmailConfirmed = true;
                                    userObj.PhoneNo = spouseObj.Phone;
                                    userObj.PasswordSalt = newPasswordSaltHash;
                                    userObj.PasswordHash = newPasswordHash;
                                    userObj.AppId = "WEBAPP";
                                    userObj.IsActive = true;
                                    userObj.LoginFailedAttemptCount = 0;
                                    _context.Users.Add(userObj);
                                }
                                userObj.Name = spouseObj.FullName;
                                userObj.UserName = spouseObj.CardNo ?? "";
                                userObj.EmailId = spouseObj.Email;
                            }
                            else
                            {
                                var userObj = userObjList.FirstOrDefault();
                                if (userObj == null)
                                {
                                    userObj = new User();
                                    userObj.MemberId = obj.Id;
                                    userObj.EmailConfirmed = true;
                                    userObj.PhoneNo = spouseObj.Phone;
                                    userObj.PasswordSalt = newPasswordSaltHash;
                                    userObj.PasswordHash = newPasswordHash;
                                    userObj.AppId = "WEBAPP";
                                    userObj.IsActive = true;
                                    userObj.LoginFailedAttemptCount = 0;
                                    _context.Users.Add(userObj);
                                }
                                userObj.Name = spouseObj.FullName;
                                userObj.UserName = request.Model.CardNo ?? "";
                                userObj.EmailId = spouseObj.Email;   
                            }
                        }
                        await _context.SaveChangesAsync(cancellation);
                        result.HasError = false;
                        result.Messages.Add("Save Success");
                    }
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add("Error" + ex.ToString());

            }

            return await _mediator.Send(new GetMemberByIdQuery()
            {
                Id = request.Model.MemberId
            });
        }
    }
}

