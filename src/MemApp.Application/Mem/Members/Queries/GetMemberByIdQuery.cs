using AutoMapper.Execution;
using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace MemApp.Application.Mem.MemberStatuss.Queries
{
    public class GetMemberByIdQuery : IRequest<RegisterMemberVm>
    {
        public int Id { get; set; }
        public string WebRootPath { get; set; }
    }

    public class GetMemberByIdQueryHandler : IRequestHandler<GetMemberByIdQuery, RegisterMemberVm>
    {
        private readonly IMemDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetMemberByIdQueryHandler(IMemDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<RegisterMemberVm> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken)
        {
            string baseUrl = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host + _httpContextAccessor.HttpContext.Request.PathBase;
            var result = new RegisterMemberVm();
            var spousedata = new SpouseRegistrationRes();

            var data = await _context.RegisterMembers
                .Include(i=>i.MemberTypes)
                .Include(i=>i.MemberStatus)
                .Include(i=>i.MemberActiveStatus)
                .Include(x => x.Colleges)
                .Include(i=>i.BloodGroup)
                .Include(i=>i.MemberProfessions)
                .Include(i=>i.MemberChildrens.Where(q=>q.IsActive))
                .Include(i=>i.MemberFeesMaps.Where(q => q.IsActive))
                .AsNoTracking()
                .SingleOrDefaultAsync(q=>q.Id==request.Id && q.IsMasterMember==true && q.IsActive, cancellationToken);

            if (data==null)
            {
                result.HasError = true;
                result.Messages?.Add("Data Not Found");
            }
            else
            {
               
                var sData = await _context.RegisterMembers
                .Include(i => i.MemberTypes)
                .Include(i => i.MemberStatus)
                .Include(i => i.MemberActiveStatus)
                .Include(x => x.Colleges)
                .Include(i => i.BloodGroup)
                .Include(i => i.MemberProfessions)
                .Include(i => i.MemberChildrens.Where(q => q.IsActive))
                .Include(i => i.MemberFeesMaps.Where(q => q.IsActive))
                .AsNoTracking()
                .SingleOrDefaultAsync(q => q.MemberId == request.Id && q.IsMasterMember==false && q.IsActive, cancellationToken);

                if (sData == null)
                {                   
                }
                else {
                    spousedata.Id = sData.Id;
                    spousedata.CardNo = sData.CardNo;
                    spousedata.FullName = sData.FullName;
                    spousedata.Spouse = sData.Spouse;
                    spousedata.MemberShipNo = sData.MembershipNo ?? "";
                    spousedata.Anniversary = sData?.Anniversary == null ? "" : sData.Anniversary.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    spousedata.SpouseOccupation = sData?.SpouseOccupation;
                    spousedata.ImgFileUrl = baseUrl + "/" + sData?.ImgFileUrl;
                }

                result.HasError = false;

                result.Data = new RegisterMemberRes
                {
                    ImgFileUrl = baseUrl + "/" + data.ImgFileUrl,
                    Id = data.Id,
                    FullName = data.FullName,
                    MemberTypeId = data.MemberTypeId,
                    MemberTypeText = data.MemberTypes.Name,
                    MemberStatusId = data.MemberStatusId,
                    MemberStatusText = data.MemberStatus?.Name,
                    MemberActiveStatusId = data.MemberActiveStatusId,
                    MemberActiveStatusText = data.MemberActiveStatus?.Name,
                    CadetNo = data.CadetNo ?? "",
                    CollegeId = data.CollegeId,
                    CollegeCode = data.Colleges.Code,
                    CollegeName = data.Colleges.Name,
                    HscYear = data.HscYear,
                    BloodGroupId = data.BloodGroupId,
                    BloodGroupText = data?.BloodGroup?.Code ?? "",
                    Dob = data?.Dob==null? "": data.Dob.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    MemberProfessionId = data?.MemberProfessionId,
                    CadetName = data?.CadetName,
                    BatchNo = data?.BatchNo ??"",
                    OfficeAddress = data?.OfficeAddress,
                    IsMasterMember = data?.IsMasterMember,
                    HomeAddress = data?.HomeAddress,
                    PostalAddress = data?.Address,
                    MemberProfessionText = data?.MemberProfessions?.Name ??"",
                    Organaization = data?.Organaization,
                    Designation = data?.Designation,
                    Specialization = data?.Specialization,
                    Phone = data?.Phone,
                    Email = data?.Email,
                    MemberFullId = data?.MemberFullId ?? "",
                    MemberShipNo = data?.MembershipNo??"",
                    CreditLimit = data?.CreditLimit,
                    PaidTill = data?.PaidTill == null ? "" : data.PaidTill.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                   
                    CardNo = data?.CardNo,
                    QBCusName = data?.QBCusName,
                    Spouse = data?.Spouse,
                    SpouseOccupation = data?.SpouseOccupation,
                    Anniversary = data?.Anniversary == null ? "" : data.Anniversary.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
               
                    SpouseData = spousedata,
                    HasSubscription = data?.MemberTypes.IsSubscribed ?? false,
                    Nok = data?.Nok,
                    ActiveStatusDate = data?.ActiveStatusDate == null ? "" : data.ActiveStatusDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                   

                    JoinDate = data?.JoinDate == null ? "" : data.JoinDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    LeaveDate = data?.LeaveDate == null ? "" : data.LeaveDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                   
                    PermanentAddress = data?.PermanentAddress,
                    NID = data?.NID,
                    EmergencyContact = data?.EmergencyContact,
                
                    CurrentBalance = await _context.MemLedgers.Where(q => q.PrvCusID == data.PrvCusID).SumAsync(s => s.Amount) ?? 0,
                    DeviceId = data?.DeviceId,
                    ClubJoinDate = data?.ClubJoinDate == null ? "" : data.ClubJoinDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                 
                    MemberchildrenReqs = data?.MemberChildrens.Where(q=>q.IsActive).Select(s => new MemberChildrenReq
                    {
                        CadetNo = s.CadetNo,
                        ContactName = s.ContactName,
                        Phone = s.Phone,
                        Dob = s?.Dob == null ? "" : s.Dob.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        Gender = s.Gender

                    }).ToList(),

                    MemberFeesMapRess = data?.MemberFeesMaps.Where(q => q.IsActive).Select(s => new MemberFeesMapRes
                    {
                        MemberFeesTitle = s.MemberFeesTitle,
                        MemberShipFeeId = s.MemberShipFeeId,
                        Amount = s.Amount,
                        IsChecked = true
                    }).ToList()

                };
            }

            return result;
        }
    }
}
