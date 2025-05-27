using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Models.DTOs;
using Res.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using MemApp.Application.Services;
using Dapper;
using static Dapper.SqlMapper;
using FluentValidation;
using ResApp.Application.ROA.MemberRegistration.Validation;


namespace ResApp.Application.ROA.MemberRegistration.Command
{
    public class UpdateMemberRegCommand : IRequest<Result<MemberRegistrationInfoDto>>, IMemberValidator
    {
        public string? EmailId { get; set; }


        public string? ApplicationNo { get; set; }

        public string? Name { get; set; }

        public string? NomineeName { get; set; }
        public string? InstituteNameBengali { get; set; }
        public string? InstituteNameEnglish { get; set; }
        public string? PhoneNo { get; set; }
        public string? PermanentAddress { get; set; }
        public string? MemberNID { get; set; }
        public string? MemberTINNo { get; set; }
        public string? MemberTradeLicense { get; set; }
        public DateTime? BusinessStartingDate { get; set; }


        public int? DivisionId { get; set; }
        public int? DistrictId { get; set; }
        public int? ThanaId { get; set; }

        public int? ZoneId { get; set; }
        public int? MunicipalityId { get; set; }
        public int? UnionInfoId { get; set; }
        public int? WardId { get; set; }

        [JsonIgnore]
        public string? WebRootPath { get; set; }

        public IFormFile? SignatureImgFile { get; init; }
        public IFormFile? NIDImgFile { get; set; }
        public IFormFile? TradeLicenseImgFile { get; set; }
        public IFormFile? TinImgFile { get; set; }
        public IFormFile? ProfileImg { get; set; }

        public List<ContactDetailReq>? ContactDetailReq { get; set; } = new List<ContactDetailReq>();
    }

    public class UpdateMemberRegCommandHandler : IRequestHandler<UpdateMemberRegCommand, Result<MemberRegistrationInfoDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDapperContext _dapperContext;
        private readonly IValidator<UpdateMemberRegCommand> _validator;

        //  private readonly IWebHostEnvironment _hostingEnv;
        public UpdateMemberRegCommandHandler(IMemDbContext context, IMediator mediator,
                                             IPermissionHandler permissionHandler, ICurrentUserService currentUserService,
                                             IDapperContext dapperContext, IValidator<UpdateMemberRegCommand> validator)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
            _dapperContext = dapperContext;
            _validator = validator;
        }

        public async Task<Result<MemberRegistrationInfoDto>> Handle(UpdateMemberRegCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }

            var user = _currentUserService.Current();
            var checkUser = await _context.Users.FirstOrDefaultAsync(x => x.EmailId == user.EmailId, cancellationToken);
            if (checkUser == null)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result<MemberRegistrationInfoDto>();



            var validResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validResult.IsValid)
            {
                result.HasError = true;
                foreach (var error in validResult.Errors)
                {
                    result.Messages.Add(error.ErrorMessage);
                }
                return result;
            }

            var memberReg = _context.MemberRegistrationInfos.FirstOrDefault(x => x.Id == checkUser.MemberInfoId);

            if (memberReg == null)
            {
                result.HasError = true;
                result.Messages.Add("User does not exist!!!");
                return result;
            }

            //var checkUserNIDExist = await _context.MemberRegistrationInfos
            //   .AnyAsync(q => q.MemberNID == request.MemberNID);



            if (!string.IsNullOrEmpty(request.EmailId))
            {


                var checkUserExist = await _context.Users
                  .FirstOrDefaultAsync(q => q.EmailId == request.EmailId,cancellationToken);

                if (checkUserExist == null)
                {
                    result.HasError = true;
                    result.Messages.Add("User does not exist!!!");
                    return result;
                }

                //   var checkMemberExist=await _context.MemberRegistrationInfos.FirstOrDefaultAsync(x=>x.)

            }





            //if (checkUserNIDExist)
            //{
            //    result.HasError = true;
            //    result.Messages.Add("NID exist!!!");
            //    return result;
            //}
            if (request.NIDImgFile == null || request.SignatureImgFile == null || request.TinImgFile == null || request.TradeLicenseImgFile == null)
            {
                result.HasError = true;
                result.Messages.Add("Please upload all required files!!!");
                return result;
            }

            string[] allowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };

            var files = new Dictionary<string, string>
            {
                { "NID Image", request.NIDImgFile.FileName },
                { "Signature Image", request.SignatureImgFile.FileName },
                { "Trade License", request.TradeLicenseImgFile.FileName },
                { "TIN Document", request.TinImgFile.FileName }
            };

            var invalidFiles = files.Where(file => !allowedExtensions.Contains(Path.GetExtension(file.Value).ToLower()))
                                    .Select(file => file.Key)
                                    .ToList();

            if (invalidFiles.Any())
            {
                result.HasError = true;
                result.Messages.Add($"Invalid file type detected for: {string.Join(", ", invalidFiles)}. Only Image (.jpg, .jpeg, .png) or PDF (.pdf) files are allowed.");
                return result;
            }
            //string ext = Path.GetExtension(request.NIDImgFile.FileName);
            //string extSign = Path.GetExtension(request.SignatureImgFile.FileName);
            //string extTrade = Path.GetExtension(request.TradeLicenseImgFile.FileName);
            //string extTin = Path.GetExtension(request.TinImgFile.FileName);

            //if (ext != ".pdf" && ext != ".jpg" && ext != ".jpeg" && ext != ".png" )
            //{
            //    result.HasError = true;
            //    result.Messages.Add("Only Image or pdf file will be accepted!!!");
            //    return result;
            //}

            //if (!checkUserNIDExist)
            //{
            try
            {
                var maxId = _context.MemberRegistrationInfos
           .OrderByDescending(s => s.Id)
           .Select(s => s.Id)
           .FirstOrDefault();

                string nextId;
                if (maxId == 0)
                {
                    nextId = "000001";
                }
                else
                {
                    int nextNumber = maxId + 1;
                    nextId = nextNumber.ToString("D6");
                }

                string uniqueFileNameNID = string.Empty;
                string uniqueFileNameTIN = string.Empty;
                string uniqueFileNameTrade = string.Empty;
                string uniqueFileNameSign = string.Empty;
                string uniqueFileNameProfile = string.Empty;


                if (request.ProfileImg != null)
                {
                    // Create uploads directory if it doesn't exist
                    string uploadsFolder = Path.Combine(request.WebRootPath!, "uploadsMembers");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate unique filename
                    string guidPart = Guid.NewGuid().ToString("N").Substring(0, 8);
                    uniqueFileNameProfile = nextId + "_" + request.EmailId + "_" + guidPart + Path.GetExtension(request.ProfileImg.FileName);

                    string filePath = Path.Combine(uploadsFolder, uniqueFileNameProfile);

                    // Save the file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.ProfileImg.CopyToAsync(fileStream);
                    }

                    memberReg.ImgPath = uniqueFileNameProfile;
                }
                if (request.NIDImgFile != null)
                {
                    // Create uploads directory if it doesn't exist
                    string uploadsFolder = Path.Combine(request.WebRootPath!, "uploadsMemberNID");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate unique filename
                    string guidPart = Guid.NewGuid().ToString("N").Substring(0, 8);
                    uniqueFileNameNID = nextId + "_" + request.MemberNID + "_" + guidPart + Path.GetExtension(request.NIDImgFile.FileName);

                    string filePath = Path.Combine(uploadsFolder, uniqueFileNameNID);

                    // Save the file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.NIDImgFile.CopyToAsync(fileStream);
                    }
                }

                if (request.TinImgFile != null)
                {
                    // Create uploads directory if it doesn't exist
                    string uploadsFolder = Path.Combine(request.WebRootPath!, "uploadsMemberTIN");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate unique filename
                    string guidPart = Guid.NewGuid().ToString("N").Substring(0, 8);
                    uniqueFileNameTIN = nextId + "_" + request.MemberTINNo + "_" + guidPart + Path.GetExtension(request.TinImgFile.FileName);

                    string filePath = Path.Combine(uploadsFolder, uniqueFileNameTIN);

                    // Save the file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.TinImgFile.CopyToAsync(fileStream);
                    }
                }

                if (request.SignatureImgFile != null)
                {
                    // Create uploads directory if it doesn't exist
                    string uploadsFolder = Path.Combine(request.WebRootPath!, "uploadsMemberSign");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate unique filename
                    string guidPart = Guid.NewGuid().ToString("N").Substring(0, 8);
                    uniqueFileNameSign = nextId + "_" + request.MemberNID + "_" + guidPart + Path.GetExtension(request.SignatureImgFile.FileName);

                    string filePath = Path.Combine(uploadsFolder, uniqueFileNameSign);

                    // Save the file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.SignatureImgFile.CopyToAsync(fileStream);
                    }
                }

                if (request.TradeLicenseImgFile != null)
                {
                    // Create uploads directory if it doesn't exist
                    string uploadsFolder = Path.Combine(request.WebRootPath!, "uploadsMemberTrade");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generate unique filename
                    string guidPart = Guid.NewGuid().ToString("N").Substring(0, 8);
                    uniqueFileNameTrade = nextId + "_" + request.MemberTradeLicense + "_" + guidPart + Path.GetExtension(request.TradeLicenseImgFile.FileName);

                    string filePath = Path.Combine(uploadsFolder, uniqueFileNameTrade);

                    // Save the file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.TradeLicenseImgFile.CopyToAsync(fileStream);
                    }
                }

                // Create the entity and add it to the database
                //var entity = new MemberRegistrationInfo
                //{
                //  Id = nextId,

                var lastmembershipNo = await _context.MemberRegistrationInfos.Select(s => s.MemberShipNo).MaxAsync(cancellationToken);
                if (lastmembershipNo != "")
                {
                    lastmembershipNo = (Convert.ToInt32(lastmembershipNo) + 1).ToString("00000000");
                }
                else
                {
                    lastmembershipNo = "00000001";
                }
                memberReg.ApplicationNo = request.ApplicationNo;
                //  memberReg.MemberShipNo = GenerateMembershipNo();
                memberReg.MemberShipNo = lastmembershipNo; // await GenerateMembershipNoAsync();// lastmembershipNo;
                memberReg.IsApproved = false;
                memberReg.PermanentAddress = request.PermanentAddress;
                memberReg.CreatedOn = DateTime.UtcNow;
                memberReg.BusinessStartingDate = request.BusinessStartingDate;
                memberReg.DistrictId = request.DistrictId;
                memberReg.DivisionId = request.DivisionId;
                memberReg.ThanaId = request.ThanaId;

                memberReg.ZoneId = request.ZoneId;
                memberReg.MunicipalityId = request.MunicipalityId;
                memberReg.UnionInfoId = request.UnionInfoId;
                memberReg.WardId = request.WardId;
                memberReg.InstituteNameBengali = request.InstituteNameBengali;
                memberReg.InstituteNameEnglish = request.InstituteNameEnglish;
                memberReg.MemberTINNo = request.MemberTINNo;
                memberReg.MemberNID = request.MemberNID;
                memberReg.Name = request.Name;
                memberReg.SignatureUploadingTime = DateTime.UtcNow;
                memberReg.MemberTradeLicense = request.MemberTradeLicense;
                memberReg.NomineeName = request.NomineeName;
                memberReg.PhoneNo = request.PhoneNo;

                memberReg.NIDImgPath = uniqueFileNameNID;
                memberReg.TinImgPath = uniqueFileNameTIN;
                memberReg.TradeLicenseImgPath = uniqueFileNameTrade;
                memberReg.SignatureImgPath = uniqueFileNameSign;
                memberReg.IsFilled = true;
                memberReg.CreatedBy = user.Id;
                memberReg.CreatedByName = user.UserName;
                memberReg.IsActive = true;
                //  memberReg.ImgPath = uniqueFileNameProfile;
                //  memberReg.PaidTill = DateTime.UtcNow;
                //  memberReg.UserId = checkUserExist?.Id;


                _context.MemberRegistrationInfos.Update(memberReg);

                if (request.ContactDetailReq?.Count > 0)
                {
                    foreach (var item in request.ContactDetailReq)
                    {
                        var contact = new MultipleOwner
                        {
                            Name = item.Name,
                            Phone = item.Phone,
                            Email = item.Phone,
                            MemberId = memberReg.Id,
                        };
                        _context.MultipleOwners.Add(contact);
                    }
                }

                if (await _context.SaveChangesAsync(cancellationToken) > 0)
                {

                    //result.Data.Name = request.Name;
                    //result.Data.UserName = request.UserName;
                    //result.Data.EmailId = request.EmailId;
                    //result.Data.PhoneNo = request.PhoneNo;
                    //result.Data.Id = user.Id;

                    result.HasError = false;
                    result.Messages.Add("Member Registration done successfully!!");

                    return result;
                }
                else
                {
                    result.HasError = true;
                    result.Messages.Add("something wrong");
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add("Something went wrong!!!");
                return result;
            }
            //  }
            return result;
        }
        public async Task<string> GenerateMembershipNoAsync()
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                string query = @"SELECT NEXT VALUE FOR dbo.MembershipNoSequence";
                var data = await connection.QueryAsync<long>(query);
                return data.First().ToString("D8"); // format like 00000001
            }
            //var nextValue = await _context.Database
            //                 .ExecuteSqlRawAsync("SELECT NEXT VALUE FOR dbo.MembershipNoSequence");
            //var result = await _context.da
            //            .sql<int>("SELECT NEXT VALUE FOR dbo.MembershipNoSequence AS Value")
            //            .AsNoTracking()
            //            .FirstAsync();

            //return result.Value.ToString("D8"); // format like 00000001
        }
        public string GenerateMembershipNo()
        {
            // string initials = new string(fullName.Split(' ').Select(w => w[0]).ToArray()).ToUpper();

            string datePart = DateTime.Now.Year.ToString();
            int count = _context.MemberRegistrationInfos.Count(m => m.MemberShipNo!.StartsWith("MEM-" + datePart)) + 1;
            int randomNum = new Random().Next(1000, 9999);
            return $"MEM-{datePart}-{count}-{Guid.NewGuid().ToString("N").Substring(0, 8)}"; // Example: "JS-240304-4567"
        }
    }
}
