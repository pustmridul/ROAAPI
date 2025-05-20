using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Models.DTOs;
using Res.Domain.Entities;
using Microsoft.AspNetCore.Http;
using ResApp.Application.ROA.RoaSubcription;
using ResApp.Application.ROA.RoaSubcription.Command;


namespace ResApp.Application.Com.Commands.MemberRegistration.UpdateMemberInfo
{
    public class UpdateMemberInfoCommand : IRequest<Result<MemberRegistrationInfoDto>>
    {
        public int Id { get; set; }
        public string? UserName {  get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? LastModifiedBy { get; set; }
        public string? LastModifiedByName { get; set; }

        public DateTime? LastModifiedOn { get; set; }

        public string? ApplicationNo { get; set; }

        public string? Name { get; set; }
        //public string? FatherName { get; set; }
        //public string? MotherName { get; set; }
        //public string? SpouseName { get; set; }
        //public string? HusbandName { get; set; }
        public string? NomineeName { get; set; }
        public string? InstituteNameBengali { get; set; }
        public string? InstituteNameEnglish { get; set; }
        public string? PhoneNo { get; set; }
        public string? PermanentAddress { get; set; }
        public string? MemberNID { get; set; }
        public string? MemberTINNo { get; set; }
        public string? MemberTradeLicense { get; set; }
        public DateTime? BusinessStartingDate { get; set; }
        public string? SignatureImgPath { get; set; }
        public string? NIDImgPath { get; set; }
        public string? TradeLicenseImgPath { get; set; }
        public string? TinImgPath { get; set; }

        public bool IsApproved { get; set; }

      //  public virtual User? ApprovedBy { get; set; }

        public DateTime? ApproveTime { get; set; }
        public DateTime? SignatureUploadingTime { get; set; }

        public int? DivisionId { get; set; }
        public int? DistrictId { get; set; }
        public int? ThanaId { get; set; }

        public int? ZoneId { get; set; }
        public int? MunicipalityId { get; set; }
        public int? UnionInfoId { get; set; }
        public int? WardId { get; set; }
        public string? WebRootPath { get; set; }

        public IFormFile? SignatureImgFile { get; init; }
        public IFormFile? ProfileImg { get; init; }
        public IFormFile? NIDImgFile { get; set; }
        public IFormFile? TradeLicenseImgFile { get; set; }
        public IFormFile? TinImgFile { get; set; }
        public decimal SubscriptionFee { get; set; }
        public DateTime? SubscriptionStarts { get; set; }

        public List<ContactDetailReq>? ContactDetailReq { get; set; } = new List<ContactDetailReq>();
    }

    public class UpdateMemberInfoCommandHandler : IRequestHandler<UpdateMemberInfoCommand, Result<MemberRegistrationInfoDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;       
        private readonly IPermissionHandler _permissionHandler;

      //  private readonly IWebHostEnvironment _hostingEnv;
        public UpdateMemberInfoCommandHandler(IMemDbContext context, IMediator mediator, IPermissionHandler permissionHandler)
        {
            _context = context;
            _mediator = mediator;            
            _permissionHandler = permissionHandler;
        }

        public async Task<Result<MemberRegistrationInfoDto>> Handle(UpdateMemberInfoCommand request, CancellationToken cancellationToken)
        {
            var result = new Result<MemberRegistrationInfoDto>();

            var checkUserNIDExist = await _context.MemberRegistrationInfos
               .Where(q => q.MemberNID == request.MemberNID ).ToListAsync();

            var checkUserExist = await _context.MemberRegistrationInfos
               .FirstOrDefaultAsync(q =>  q.Id == request.Id);

            if (checkUserExist == null)
            {
                result.HasError = true;
                result.Messages.Add("Member does not exist!!!");
                return result;
            }

            //if (checkUserNIDExist.Count >1)
            //{
            //    result.HasError = true;
            //    result.Messages.Add("NID exist!!!");
            //    return result;
            //}
            //if (request.NIDImgFile == null || request.SignatureImgFile == null || request.TinImgFile == null || request.TradeLicenseImgFile == null)
            //{
            //    result.HasError = true;
            //    result.Messages.Add("Please upload all required files!!!");
            //    return result;
            //}

            //string[] allowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };

            //var files = new Dictionary<string, string>
            //{
            //    { "NID Image", request.NIDImgFile.FileName },
            //    { "Signature Image", request.SignatureImgFile.FileName },
            //    { "Trade License", request.TradeLicenseImgFile.FileName },
            //    { "TIN Document", request.TinImgFile.FileName }
            //};

            //var invalidFiles = files.Where(file => !allowedExtensions.Contains(Path.GetExtension(file.Value).ToLower()))
            //                        .Select(file => file.Key)
            //                        .ToList();

            //if (invalidFiles.Any())
            //{
            //    result.HasError = true;
            //    result.Messages.Add($"Invalid file type detected for: {string.Join(", ", invalidFiles)}. Only Image (.jpg, .jpeg, .png) or PDF (.pdf) files are allowed.");
            //    return result;
            //}
          //  string ext = Path.GetExtension(request.NIDImgFile!.FileName);
            //string extSign = Path.GetExtension(request.SignatureImgFile!.FileName);
            //string extTrade = Path.GetExtension(request.TradeLicenseImgFile!.FileName);
            //string extTin = Path.GetExtension(request.TinImgFile!.FileName);

            //if (ext != ".pdf" && ext != ".jpg" && ext != ".jpeg" && ext != ".png")
            //{
            //    result.HasError = true;
            //    result.Messages.Add("Only Image or pdf file will be accepted!!!");
            //    return result;
            //}

            if (checkUserExist != null)
            {
                try
                {
                    var maxId = _context.MemberRegistrationInfos
               .OrderByDescending(s => s.Id)
               .Select(s => s.Id)
               .FirstOrDefault();

                    string nextId;
                    if (maxId == 0)
                    {
                        nextId = "0000001";
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

                        string ext = Path.GetExtension(request.ProfileImg!.FileName);

                        if (ext != ".pdf" && ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                        {
                            result.HasError = true;
                            result.Messages.Add("Only Image or pdf file will be accepted!!!");
                            return result;
                        }
                        // Create uploads directory if it doesn't exist
                        string uploadsFolder = Path.Combine(request.WebRootPath!, "uploadsMembers");

                        // Generate unique filename
                        string guidPart = Guid.NewGuid().ToString("N").Substring(0, 8);
                        uniqueFileNameProfile = nextId + "_" + checkUserExist.MemberShipNo + "_" + guidPart + Path.GetExtension(request.ProfileImg.FileName);

                        string filePath = Path.Combine(uploadsFolder, uniqueFileNameProfile);

                        string oldFilePath = Path.Combine(uploadsFolder, checkUserExist.ImgPath!);

                        // Check if the file exists
                        if (File.Exists(oldFilePath))
                        {
                            // Delete the file
                            System.IO.File.Delete(oldFilePath);
                            //  return Ok(new { message = "File deleted successfully." });
                        }
                        else
                        {
                            //  return NotFound(new { message = "File not found." });

                            result.HasError = true;
                            result.Messages.Add("Something went wrong!!!");
                        }

                        // Save the file
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await request.ProfileImg.CopyToAsync(fileStream);
                        }

                        checkUserExist.ImgPath = uniqueFileNameProfile;
                    }

                    if (request.NIDImgFile != null)
                    {

                        string ext = Path.GetExtension(request.NIDImgFile!.FileName);

                        if (ext != ".pdf" && ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                        {
                            result.HasError = true;
                            result.Messages.Add("Only Image or pdf file will be accepted!!!");
                            return result;
                        }
                        // Create uploads directory if it doesn't exist
                        string uploadsFolder = Path.Combine(request.WebRootPath!, "uploadsMemberNID");
                       
                        // Generate unique filename
                        string guidPart = Guid.NewGuid().ToString("N").Substring(0, 8);
                        uniqueFileNameNID = nextId + "_" + request.MemberNID + "_" + guidPart + Path.GetExtension(request.NIDImgFile.FileName);

                        string filePath = Path.Combine(uploadsFolder, uniqueFileNameNID);

                        string oldFilePath= Path.Combine(uploadsFolder, checkUserExist.NIDImgPath!);

                        // Check if the file exists
                        if (File.Exists(oldFilePath))
                        {
                            // Delete the file
                            System.IO.File.Delete(oldFilePath);
                          //  return Ok(new { message = "File deleted successfully." });
                        }
                        else
                        {
                            //  return NotFound(new { message = "File not found." });

                            result.HasError = true;
                            result.Messages.Add("Something went wrong!!!");
                        }

                        // Save the file
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await request.NIDImgFile.CopyToAsync(fileStream);
                        }

                        checkUserExist.NIDImgPath = uniqueFileNameNID;
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

                        string oldFilePath = Path.Combine(uploadsFolder, checkUserExist.TinImgPath!);

                        // Check if the file exists
                        if (File.Exists(oldFilePath))
                        {
                            // Delete the file
                            System.IO.File.Delete(oldFilePath);
                            //  return Ok(new { message = "File deleted successfully." });
                        }
                        else
                        {
                            //  return NotFound(new { message = "File not found." });

                            result.HasError = true;
                            result.Messages.Add("Something went wrong!!!");
                        }

                        // Save the file
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await request.TinImgFile.CopyToAsync(fileStream);
                        }

                        checkUserExist.TinImgPath = uniqueFileNameTIN;
                    }

                    if (request.SignatureImgFile != null)
                    {

                        string extSign = Path.GetExtension(request.SignatureImgFile!.FileName);

                        if (extSign != ".pdf" && extSign != ".jpg" && extSign != ".jpeg" && extSign != ".png")
                        {
                            result.HasError = true;
                            result.Messages.Add("Only Image or pdf file will be accepted!!!");
                            return result;
                        }
                        // Create uploads directory if it doesn't exist
                        string uploadsFolder = Path.Combine(request.WebRootPath!, "uploadsMemberSign");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Generate unique filename
                        string guidPart = Guid.NewGuid().ToString("N").Substring(0, 8);
                        uniqueFileNameSign = nextId + "_" + request!.MemberNID + "_" + guidPart + Path.GetExtension(request.SignatureImgFile.FileName);

                        string filePath = Path.Combine(uploadsFolder, uniqueFileNameSign);

                        string oldFilePath = Path.Combine(uploadsFolder, checkUserExist.SignatureImgPath!);

                        // Check if the file exists
                        if (File.Exists(oldFilePath))
                        {
                            // Delete the file
                            System.IO.File.Delete(oldFilePath);
                            //  return Ok(new { message = "File deleted successfully." });
                        }
                        else
                        {
                            //  return NotFound(new { message = "File not found." });

                            result.HasError = true;
                            result.Messages.Add("Something went wrong!!!");
                        }

                        // Save the file
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await request.SignatureImgFile.CopyToAsync(fileStream);
                        }

                        checkUserExist.SignatureImgPath = uniqueFileNameSign;
                        checkUserExist.SignatureUploadingTime=DateTime.UtcNow;
                    }

                    if (request.TradeLicenseImgFile != null)
                    {

                        string extSign = Path.GetExtension(request.TradeLicenseImgFile!.FileName);

                        if (extSign != ".pdf" && extSign != ".jpg" && extSign != ".jpeg" && extSign != ".png")
                        {
                            result.HasError = true;
                            result.Messages.Add("Only Image or pdf file will be accepted!!!");
                            return result;
                        }
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

                        string oldFilePath = Path.Combine(uploadsFolder, checkUserExist.TradeLicenseImgPath!);

                        // Check if the file exists
                        if (File.Exists(oldFilePath))
                        {
                            // Delete the file
                            System.IO.File.Delete(oldFilePath);
                            //  return Ok(new { message = "File deleted successfully." });
                        }
                        else
                        {
                            //  return NotFound(new { message = "File not found." });

                            result.HasError = true;
                            result.Messages.Add("Something went wrong!!!");
                        }


                        // Save the file
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await request.TradeLicenseImgFile.CopyToAsync(fileStream);
                        }

                        checkUserExist.TradeLicenseImgPath = uniqueFileNameTrade;
                    }

                    checkUserExist.ApplicationNo = request.ApplicationNo;
                 
                    checkUserExist.PermanentAddress = request.PermanentAddress;
                   
                    checkUserExist.BusinessStartingDate = request.BusinessStartingDate;
                    checkUserExist.DistrictId = request.DistrictId;
                    checkUserExist.DivisionId = request.DivisionId;
                    checkUserExist.ThanaId = request.ThanaId;

                    checkUserExist.ZoneId = request.ZoneId;
                    checkUserExist.MunicipalityId = request.MunicipalityId;
                    checkUserExist.UnionInfoId = request.UnionInfoId;
                    checkUserExist.WardId = request.WardId;

                    checkUserExist.InstituteNameBengali = request.InstituteNameBengali;
                    checkUserExist.InstituteNameEnglish = request.InstituteNameEnglish;
                    checkUserExist.MemberTINNo = request.MemberTINNo;
                    checkUserExist.MemberNID = request.MemberNID;
                    checkUserExist.Name = request.Name;
                 
                    checkUserExist.MemberTradeLicense = request.MemberTradeLicense;
                    checkUserExist.NomineeName = request.NomineeName;
                    checkUserExist.PhoneNo = request.PhoneNo;
                    checkUserExist.IsApproved=request.IsApproved;
                    checkUserExist.SubscriptionFee = request.SubscriptionFee;
                   


                    if(checkUserExist.SubscriptionStarts ==null && checkUserExist.PaidTill ==null && request.SubscriptionStarts != null)
                    {
                        //checkUserExist.SubscriptionStarts = request.SubscriptionStarts;
                        //// Subtract one month to go to the previous month
                        //DateTime previousMonth = request.SubscriptionStarts.GetValueOrDefault().AddMonths(-1);

                        //// Get the last day of that month
                        //int lastDay = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);
                        ////var paidTill= request.SubscriptionStarts.AddMonths(-1);
                        //checkUserExist.PaidTill = new DateTime(previousMonth.Year, previousMonth.Month, lastDay);


                    }

                   
               
                    

                    _context.MemberRegistrationInfos.Update(checkUserExist);

                    if (request.ContactDetailReq?.Count > 0)
                    {
                        var existingOwners = await _context.MultipleOwners
                                            .Where(x => x.MemberId == checkUserExist.Id)
                                            .ToListAsync(cancellationToken);

                        var incomingIds = request.ContactDetailReq
                                            .Where(x => x.Id > 0)
                                            .Select(x => x.Id)
                                            .ToList();

                        foreach (var item in request.ContactDetailReq)
                        {
                            var exist= await _context.MultipleOwners.FirstOrDefaultAsync(x=>x.Id == item.Id && x.MemberId==checkUserExist.Id,cancellationToken);
                            if(exist != null)
                            {
                                exist.Name = item.Name;
                                exist.Email = item.Email;
                                exist.Phone = item.Phone;

                                _context.MultipleOwners.Update(exist);
                            }
                            else
                            {
                                var contact = new MultipleOwner
                                {
                                    MemberId=checkUserExist.Id,
                                };

                                if(item.Name != "null" && item.Name !="undefined" )
                                {
                                    contact.Name = item.Name;
                                }
                                if (item.Email != "null" && item.Email != "undefined")
                                {
                                    contact.Email = item.Email;
                                }
                                if (item.Phone != "null" && item.Phone != "undefined")
                                {
                                    contact.Phone = item.Phone;
                                }

                                _context.MultipleOwners.Add(contact);
                            }
                            
                        }

                        // 3. Remove items not in the incoming request
                        var toRemove = existingOwners
                            .Where(x => !incomingIds.Contains(x.Id))
                            .ToList();

                        _context.MultipleOwners.RemoveRange(toRemove);
                    }
                    else
                    {
                        var existingOwners = await _context.MultipleOwners
                                            .Where(x => x.MemberId == checkUserExist.Id)
                                            .ToListAsync(cancellationToken);
                        _context.MultipleOwners.RemoveRange(existingOwners);
                    }


                    if (await _context.SaveChangesAsync(cancellationToken) > 0)
                    {
                        //var checkExist = _context.RoSubscriptionDueTemps
                        //                .AsNoTracking()
                        //                .Any(x => x.MemberId == checkUserExist.Id);
                        //if (!checkExist)
                        //{

                        //    await _mediator.Send(new RSubscriptionDueByMemberCommand() { MemberId = checkUserExist.Id });
                        //}
                     
                        result.HasError = false;
                        result.Messages.Add("Member Registration Updated successfully!!");

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
            }
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
