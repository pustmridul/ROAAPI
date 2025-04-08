using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;


namespace MemApp.Application.Mem.MemberStatuss.Queries
{
    public class GetMemberByMemberCardNoQuery : IRequest<RegisterMemberVm>
    {
        public string CardNo { get; set; }
    }


    public class GetMemberByMemberCardNoQueryHandler : IRequestHandler<GetMemberByMemberCardNoQuery, RegisterMemberVm>
    {
        private readonly IMemDbContext _context;
        public GetMemberByMemberCardNoQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<RegisterMemberVm> Handle(GetMemberByMemberCardNoQuery request, CancellationToken cancellationToken)
        {
            var result = new RegisterMemberVm();
            var data = await _context.RegisterMembers
                .Include(i=>i.MemberTypes)
                .Include(i=>i.MemberStatus)
                .Include(i=>i.MemberActiveStatus)
                .Include(x => x.Colleges)
                .Include(i=>i.BloodGroup)
                .Include(i=>i.MemberProfessions)
                .Include(i=>i.MemberChildrens.Where(q => q.IsActive))
                .Include(i=>i.MemberFeesMaps.Where(q=>q.IsActive))
                .AsNoTracking()
                .SingleOrDefaultAsync(q=>q.CardNo==request.CardNo && q.IsActive, cancellationToken);

            if (data==null)
            {
                result.HasError = true;
                result.Messages?.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;

                result.Data = new RegisterMemberRes
                {
                    Id = data.Id,
                    FullName = data.FullName,
                    MemberTypeId = data.MemberTypeId,
                    MemberTypeText = data.MemberTypes.Name,
                    MemberStatusId = data.MemberStatusId,
                    MemberStatusText = data.MemberStatus.Name,
                    MemberActiveStatusId = data.MemberActiveStatusId,
                    MemberActiveStatusText = data.MemberActiveStatus.Name,
                    CadetNo = data.CadetNo??"",
                    CollegeId = data.CollegeId,
                    CollegeCode = data.Colleges.Code,
                    CollegeName = data.Colleges.Name,
                    HscYear = data.HscYear,
                    BloodGroupId = data.BloodGroupId,
                    BloodGroupText = data?.BloodGroup?.Code ?? "",
                    Dob = data?.Dob.ToString(),
                    MemberProfessionId = data?.MemberProfessionId,
                    CadetName= data?.CadetName,
                    BatchNo= data?.BatchNo??"",
                    OfficeAddress= data?.OfficeAddress,
                    ImgFileUrl = data?.ImgFileUrl,
                    HomeAddress = data?.HomeAddress,
                    PostalAddress= data?.Address,
                    MemberProfessionText = data?.MemberProfessions?.Name ?? "",
                    Organaization = data?.Organaization,
                    Designation = data?.Designation,
                    Specialization = data?.Specialization,
                    Phone = data?.Phone,
                    Email = data?.Email,       
                    MemberFullId = data?.MemberFullId ?? "",
                    MemberShipNo= data?.MembershipNo,
                    PaidTill= data?.PaidTill.ToString(),
                    CardNo= data?.CardNo,
                    Spouse = data?.Spouse,
                    SpouseOccupation = data?.SpouseOccupation,
                    Anniversary = data?.Anniversary.ToString(),

                    JoinDate = data?.JoinDate.ToString(),
                    LeaveDate = data?.LeaveDate.ToString(),
                    PermanentAddress = data.PermanentAddress,
                    NID = data.NID,
                    EmergencyContact = data.EmergencyContact,

                    CurrentBalance = await _context.MemLedgers.Where(q => q.PrvCusID == data.PrvCusID).SumAsync(s => s.Amount) ?? 0,
                    MemberchildrenReqs = data.MemberChildrens.Where(q => q.IsActive).Select(s => new MemberChildrenReq
                    {
                        CadetNo = s.CadetNo,
                        ContactName = s.ContactName,
                        Phone = s.Phone,
                        Dob = s?.Dob == null ? "" : s.Dob.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        Gender = s.Gender

                    }).ToList(),

                    MemberFeesMapRess = data.MemberFeesMaps.Where(q=>q.IsActive).Select(s => new MemberFeesMapRes
                    {
                        MemberFeesTitle = s.MemberFeesTitle,
                        MemberShipFeeId = s.MemberShipFeeId,
                        Amount = s.Amount,
                        IsChecked=true
                    }).ToList()

                };
            }

            return result;
        }
    }
}
