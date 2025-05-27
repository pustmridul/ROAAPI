using AutoMapper.Execution;
using MediatR;
using MemApp.Application.App.Commands;
using MemApp.Application.App.Models;
using MemApp.Application.Com.Models;
using MemApp.Application.Com.Queries.SaveRolesByUser;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemberTemps.Models;
using MemApp.Application.Services;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Res.Domain.Entities;
using System.Globalization;
using System.Net.NetworkInformation;

namespace MemApp.Application.Mem.MemberTemps.Command
{
    public class CreateMemberTempCommand : IRequest<MemberLoginVm>
    {
        public MemberTempReq Model { get; set; } = new MemberTempReq();
        public string? WebRootPath { get; set; }
    }

    public class CreateMemberTempCommandHandler : IRequestHandler<CreateMemberTempCommand, MemberLoginVm>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly IUserLogService _userLogService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPasswordHash _passwordHash;
        public CreateMemberTempCommandHandler(
            IMemDbContext context,
            IMediator mediator,
            IUserLogService userLogService,
            ICurrentUserService currentUserService,
            IPasswordHash passwordHash
            )
        {
            _context = context;
            _mediator = mediator;
            _userLogService = userLogService;
            _currentUserService = currentUserService;
            _passwordHash = passwordHash;
        }
        public async Task<MemberLoginVm> Handle(CreateMemberTempCommand request, CancellationToken cancellation)
        {
            var result = new MemberLoginVm();
            try
            {
                var obj = await _context.MemberTemps
                               .Include(i => i.MemberChildrens)
                               .Include(i => i.MemberEducations)
                               .SingleOrDefaultAsync(q => q.Id == request.Model.Id, cancellation);


                if (obj == null)
                {
                    obj = new MemberTemp();
                    obj.AffiliateMember = request.Model.AffiliateMember;
                    obj.ColorOfEye = request.Model.ColorOfEye;
                    obj.ColorOfHair = request.Model.ColorOfHair;
                    obj.DateOfApplication = (request.Model.DateOfApplication == null || request.Model.DateOfApplication == "") ? null : DateTime.Parse(request.Model.DateOfApplication);
                    obj.Designation = request.Model.Designation;
                    obj.BatchNo = request.Model.BatchNo;
                    obj.SpouseOccupation = request.Model.SpouseOccupation;
                    obj.Spouse = request.Model.Spouse;
                    obj.CadetNo = request.Model.CadetNo;
                    obj.BloodGroupId = request.Model.BloodGroupId;
                    obj.AncestralHome = request.Model.AncestralHome;
                    obj.Anniversary = (request.Model.Anniversary == null || request.Model.Anniversary == "") ? null : DateTime.Parse(request.Model.Anniversary);
                    obj.CadetName = request.Model.CadetName;
                    obj.CCCertificate = request.Model.CCCertificate;
                    obj.ClubName = request.Model.ClubName;
                    obj.CollegeId = request.Model.CollegeId;
                    obj.Dob = (request.Model.Dob == null || request.Model.Dob == "") ? null : DateTime.Parse(request.Model.Dob);
                    obj.Email = request.Model.Email;
                    obj.FatherName = request.Model.FatherName;
                    obj.FullName = request.Model.FullName ??"";
                    obj.HeightCms = request.Model.HeightCms;
                    obj.Hobbies = request.Model.Hobbies;
                    obj.HomeAddress = request.Model.HomeAddress;
                    obj.HonorAndAwards = request.Model.HonorAndAwards;
                    obj.IdentificationMarks = request.Model.IdentificationMarks;
                    obj.ImgFileUrl = request.Model.ImgFileUrl;
                    obj.JoiningDate = (request.Model.JoiningDate == null || request.Model.JoiningDate == "") ? null : DateTime.Parse(request.Model.JoiningDate);
                    obj.Status = "Pending";
                    obj.LeavingDate = (request.Model.LeavingDate == null || request.Model.LeavingDate == "") ? null : DateTime.Parse(request.Model.LeavingDate);
                    obj.MemberProfessionId = request.Model.MemberProfessionId;
                    obj.MotherName = request.Model.MotherName;
                    obj.Nationality = request.Model.Nationality;
                    obj.NID = request.Model.NID;
                    obj.OfficeAddress = request.Model.OfficeAddress;
                    obj.Organaization = request.Model.Organaization;
                    obj.Phone = request.Model.Phone;
                    obj.PrimaryMember = request.Model.PrimaryMember;
                    obj.Specialization = request.Model.Specialization;
                    obj.SpouseBloodGroupId = request.Model.SpouseBloodGroupId;
                    obj.TIN = request.Model.TIN;
                    obj.WeightKgs = request.Model.WeightKgs;
                    obj.IsActive = true;

                    obj.PinNo= request.Model.PinNo;

                    string preFix = "T";

                    var max = _context.MemberTemps.Where(q => q.MemberShipNo.StartsWith(preFix))
                        .Select(s => s.MemberShipNo.Replace(preFix, "")).DefaultIfEmpty().Max();

                    if (string.IsNullOrEmpty(max))
                    {
                        obj.MemberShipNo = preFix + "0001";
                        obj.CardNo = "C0001";
                    }
                    else
                    {
                        obj.MemberShipNo = preFix + (Convert.ToInt32(max) + 1).ToString("0000");
                        obj.CardNo = "C" + (Convert.ToInt32(max) + 1).ToString("0000");
                    }
                  
                    _context.MemberTemps.Add(obj);
                    result.HasError = false;
                    result.Messages.Add("New MemberTemp Created");



                }
                else
                {
                    obj.AffiliateMember = request.Model.AffiliateMember;
                    obj.ColorOfEye = request.Model.ColorOfEye;
                    obj.ColorOfHair = request.Model.ColorOfHair;
                    obj.DateOfApplication = (request.Model.DateOfApplication == null || request.Model.DateOfApplication == "") ? null : DateTime.Parse(request.Model.DateOfApplication);
                    obj.Designation = request.Model.Designation;
                    obj.BatchNo = request.Model.BatchNo;
                    obj.SpouseOccupation = request.Model.SpouseOccupation;
                    obj.Spouse = request.Model.Spouse;
                    obj.CadetNo = request.Model.CadetNo;
                    obj.BloodGroupId = request.Model.BloodGroupId;
                    obj.AncestralHome = request.Model.AncestralHome;
                    obj.Anniversary = (request.Model.Anniversary == null || request.Model.Anniversary == "") ? null : DateTime.Parse(request.Model.Anniversary);
                    obj.CadetName = request.Model.CadetName;
                    obj.CCCertificate = request.Model.CCCertificate;
                    obj.ClubName = request.Model.ClubName;
                    obj.CollegeId = request.Model.CollegeId;
                    obj.Dob = (request.Model.Dob == null || request.Model.Dob == "") ? null : DateTime.Parse(request.Model.Dob);
                    obj.Email = request.Model.Email;
                    obj.FatherName = request.Model.FatherName;
                    obj.FullName = request.Model.FullName ?? "";
                    obj.HeightCms = request.Model.HeightCms;
                    obj.Hobbies = request.Model.Hobbies;
                    obj.HomeAddress = request.Model.HomeAddress;
                    obj.HonorAndAwards = request.Model.HonorAndAwards;
                    obj.IdentificationMarks = request.Model.IdentificationMarks;
                    obj.ImgFileUrl = request.Model.ImgFileUrl;
                    obj.JoiningDate = (request.Model.JoiningDate == null || request.Model.JoiningDate == "") ? null : DateTime.Parse(request.Model.JoiningDate);
                    obj.Status = request.Model.Status;
                    obj.LeavingDate = (request.Model.LeavingDate == null || request.Model.LeavingDate == "") ? null : DateTime.Parse(request.Model.LeavingDate);
                    obj.MemberProfessionId = request.Model.MemberProfessionId;
                    obj.MotherName = request.Model.MotherName;
                    obj.Nationality = request.Model.Nationality;
                    obj.NID = request.Model.NID;
                    obj.OfficeAddress = request.Model.OfficeAddress;
                    obj.Organaization = request.Model.Organaization;
                    obj.Phone = request.Model.Phone;
                    obj.PrimaryMember = request.Model.PrimaryMember;
                    obj.Specialization = request.Model.Specialization;
                    obj.SpouseBloodGroupId = request.Model.SpouseBloodGroupId;
                    obj.TIN = request.Model.TIN;
                    obj.WeightKgs = request.Model.WeightKgs;
                    obj.IsActive = true;
                    result.HasError = false;
                    result.Messages.Add("MemberTemp Updated");
                }


                await _context.SaveChangesAsync(cancellation);


                if (request.Model.MemberChildrenReqs != null)
                {
                    List<MemberChildren> childrens = new List<MemberChildren>();
                    foreach (var mc in request.Model.MemberChildrenReqs)
                    {
                        var exist = await _context.MemberChildrens.SingleOrDefaultAsync(q => q.Id == mc.Id);
                        if (exist == null)
                        {
                            exist = new MemberChildren()
                            {
                                CadetNo = mc.CadetNo,
                                ContactName = mc.ContactName,
                                Dob = (mc.Dob == null || mc.Dob == "") ? null : DateTime.Parse(mc.Dob),
                                Email = mc.Email,
                                Gender = mc.Gender,
                                Phone = mc.Phone,
                                RegisterMemberId = obj.Id
                            };
                            childrens.Add(exist);
                        }
                    }
                    _context.MemberChildrens.AddRange(childrens);
                }

                if (request.Model.MemberEducationReqs != null)
                {
                    List<MemberEducation> educations = new List<MemberEducation>();
                    foreach (var me in request.Model.MemberEducationReqs)
                    {
                        var exist = _context.MemberEducations.SingleOrDefault(q => q.Id == me.Id);
                        if (exist == null)
                        {
                            exist = new MemberEducation()
                            {
                                Board = me.Board,
                                Exam = me.Exam,
                                Grade = me.Grade,
                                Institution = me.Institution,
                                MemberId = obj.Id,
                                PassingYear = me.PassingYear
                            };
                            educations.Add(exist);
                        }

                    }
                    _context.MemberEducations.AddRange(educations);

                }
                if (request.Model.AppId == "WEBSITE")
                {


                    if (request.Model?.ProfileImageUrl != null && request.Model.ProfileImageUrl.Length != 0)
                    {
                        
                        string base64String = request.Model.ProfileImageUrl;

                        byte[] imageBytes = Convert.FromBase64String(base64String);

                        var content = new System.IO.MemoryStream(imageBytes);
                       // string guessedExtension = GuessFileExtension(imageBytes);
                       
                        string fileName = obj.MemberShipNo + ".png";
                      
                        var uploadsFolder = Path.Combine(request.WebRootPath ?? "", "MemTemp");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var nextPath = Path.Combine(uploadsFolder, fileName);
                        await CopyStream(content, nextPath);
                      

                        //var file = request.Model.ProfileImage;

                        //string fileExtension = Path.GetExtension(file.FileName).TrimStart('.');

                      
                        //var fileName = obj.Id.ToString() + "-" + obj.MemberShipNo + "." + fileExtension;

                        //var uploadsFolder = Path.Combine(request.WebRootPath ?? "", "MemTemp" );
                        //if (!Directory.Exists(uploadsFolder))
                        //    Directory.CreateDirectory(uploadsFolder);

                        //var filePath = Path.Combine(uploadsFolder, fileName);

                        //using (var stream = new FileStream(filePath, FileMode.Create))
                        //{
                        //    await request.Model.ProfileImage.CopyToAsync(stream);
                        //}
                        obj.ProfileImageUrl = nextPath;
                    }


                    await _context.SaveChangesAsync(cancellation);

                    result.Email = obj.Email??"";
                    result.Data.ImgFileUrl = obj.ProfileImageUrl?? "";
                    result.Data.MemberId = obj.Id;
                    result.Data.CardNo = obj.CardNo;
                    result.Data.MembershipNo = obj.MemberShipNo;
                    result.Data.FullName= obj.FullName;
                    result.Data.Phone = obj.Phone??"";
                    result.Data.Organaization = obj.Organaization ?? "";
                    result.Data.Specialaization = obj.Specialization ?? "";
                    result.Data.Designation = obj.Designation ?? "";
                    result.Data.OfficeAddress = obj.OfficeAddress ?? "";
                    result.Data.HomeAddress = obj.HomeAddress ?? "";
                    result.Data.Spouse = obj.Spouse;
                    result.Data.SpouseOccupation = obj.SpouseOccupation;
                    result.Data.Dob = obj.Dob==null ? "" : obj.Dob.ToString();
                    result.Data.CurrentBalance = 0;
                    result.Data.MemberProfessionId = obj.MemberProfessionId;
                    var memberProfession = await _context.MemberProfessions.Select(s=> new {s.Id, s.Name})
                        .FirstOrDefaultAsync(q => q.Id == obj.MemberProfessionId, cancellation);

                    result.Data.MemberProfessionText = memberProfession?.Name;

                  //  result.UserId = obj.
                  //  result.

                    //'UserId' => $data['UserId'],
                    //    'UserName' => $data['UserName'],
                    //    'Email' => $data['Email'],
                    //    'ImgFileUrl' => $data['Data']['ImgFileUrl'],
                    //    'MemberId' => $data['Data']['MemberId'],
                    //    'MembershipNo' => $data['Data']['MembershipNo'],
                    //    'Picture' => $data['Data']['Picture'],
                    //    'ExpireDate' => $data['Data']['ExpireDate'],
                    //    'IsMasterMember' => $data['Data']['IsMasterMember'],
                    //    'AccountNumber' => $data['Data']['AccountNumber'],
                    //    'CurrentBalance' => $data['Data']['CurrentBalance'],
                    //    'PaidTill' => $data['Data']['PaidTill'],
                    //    'FullName' => $data['Data']['FullName'],
                    //    'Phone' => $data['Data']['Phone'],
                    //    'Organaization' => $data['Data']['Organaization'],
                    //    'Designation' => $data['Data']['Designation'],
                    //    'Specialaization' => $data['Data']['Specialaization'],
                    //    'OfficeAddress' => $data['Data']['OfficeAddress'],
                    //    'HomeAddress' => $data['Data']['HomeAddress'],
                    //    'Dob' => $data['Data']['Dob'],
                    //    'MemberStatusId' => $data['Data']['MemberStatusId'],
                    //    'MemberStatusText' => $data['Data']['MemberStatusText'],
                    //    'MemberActiveStatusId' => $data['Data']['MemberActiveStatusId'],
                    //    'MemberActiveStatusText' => $data['Data']['MemberActiveStatusText'],
                    //    'MemberTypeId' => $data['Data']['MemberTypeId'],
                    //    'MemberTypeText' => $data['Data']['MemberTypeText'],
                    //    'Spouse' => $data['Data']['Spouse'],
                    //    'SpouseOccupation' => $data['Data']['SpouseOccupation'],
                    //    'IsActive' => $data['Data']['IsActive'],
                    //    'PinNo' => Hash::make($PinNo),
                    //    'CardNo' => $data['Data']['CardNo'],
                    result.HasError = false;
                    result.Messages.Add("Congratulations, your account has been successfully created.");
                    return result;
                }
                //  await _context.SaveChangesAsync(cancellation);


                var memberObj = await _context.RegisterMembers
                    .SingleOrDefaultAsync(q => q.MembershipNo == obj.MemberShipNo, cancellation);
              
                var newPin = (obj.PinNo == null || obj.PinNo == "") ? new Random(DateTime.Now.Millisecond).Next(1000, 9999).ToString() : obj.PinNo;

                var newPasswordHash = "";
                var newPasswordSaltHash = "";

                if (memberObj == null)
                {

                    memberObj = new RegisterMember()
                    {
                        IsActive = true,
                        CurrentBalance = 0,
                        IsMasterMember = true,

                    };


                    _passwordHash.CreateHash(newPin.ToString(CultureInfo.InvariantCulture), ref newPasswordHash, ref newPasswordSaltHash);

                    memberObj.PinNoHash = newPasswordHash;
                    memberObj.PinNoSalt = newPasswordSaltHash;
                    memberObj.PinNo = _passwordHash.GetEncryptedPassword(newPin.ToString());


                    memberObj.MembershipNo = obj.MemberShipNo;
                }



                memberObj.Dob = (request.Model.Dob == null || request.Model.Dob == "") ? null : DateTime.Parse(request.Model.Dob);
                memberObj.CadetName = request.Model.CadetName;
                memberObj.FullName = request.Model.FullName ?? "";
                memberObj.Phone = request.Model.Phone;
                memberObj.Email = request.Model.Email;

                memberObj.MemberProfessionId = request.Model.MemberProfessionId;
                memberObj.Organaization = request.Model.Organaization;
                memberObj.Designation = request.Model.Designation;
                memberObj.Specialization = request.Model.Specialization;
                memberObj.BloodGroupId = request.Model.BloodGroupId;

                memberObj.HomeAddress = request.Model.HomeAddress;
                memberObj.OfficeAddress = request.Model.OfficeAddress;
                memberObj.CadetNo = request.Model.CadetNo;
                memberObj.CollegeId = request.Model.CollegeId??0;
                memberObj.BatchNo = request.Model.BatchNo;
                memberObj.MemberFullId = memberObj.MembershipNo ?? "";
                memberObj.CardNo = obj.CardNo;
             
                
                memberObj.MemberTypeId = 77;
                memberObj.MemberActiveStatusId = 3;
                memberObj.MemberStatusId = 8;
               
                
                _context.RegisterMembers.Add(memberObj);

                if (await _context.SaveChangesAsync(cancellation) > 0)
                {
                    memberObj.PrvCusID = memberObj.Id.ToString();


                    var userObj = await _context.Users.SingleOrDefaultAsync(q => q.UserName == request.Model.CardNo && q.MemberId == memberObj.Id, cancellation);

                    if (userObj == null)
                    {
                        userObj = new User();
                        userObj.UserName = memberObj.CardNo ?? "";
                        userObj.MemberId = memberObj.Id;
                        userObj.Name = memberObj.FullName;
                        userObj.EmailId = memberObj.Email;
                        userObj.EmailConfirmed = true;
                        userObj.PhoneNo = memberObj.Phone;
                        userObj.PasswordSalt = newPasswordSaltHash;
                        userObj.PasswordHash = newPasswordHash;
                        userObj.AppId = "MOBILEAPP";
                        userObj.IsActive = true;
                        userObj.LoginFailedAttemptCount = 0;
                        _context.Users.Add(userObj);
                    }
                  
                    await _context.SaveChangesAsync(cancellation);

                    var roleobj = new UserRoleMap()
                    {
                        RoleId = 7,
                        UserId = userObj.Id,
                        IsActive = true
                    };
                    _context.UserRoleMaps.Add(roleobj);
                }
                var loginModel = new MemberLoginReq();

                loginModel.IpAddress = request.Model.IpAddress??"";
                loginModel.AppId = "MOBILEAPP";
                loginModel.CardNo = memberObj.CardNo ?? "";
                loginModel.PinNo = newPin;
                result = await _mediator.Send(new MemberLoginCommand()
                {
                    Model = loginModel,
                });
                return result;

            }
            catch(Exception ex) 
            {
                result.HasError = true;
                result.Messages.Add("something wrong");
             
            }
           

            return result;
        }
        public async Task CopyStream(Stream stream, string downloadPath)
        {
            using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }
        }
    }

}
