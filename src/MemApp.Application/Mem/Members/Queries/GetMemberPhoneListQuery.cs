using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;


namespace MemApp.Application.Mem.Members.Queries
{

    public class GetMemberPhoneListQuery : IRequest<MemberPhoneListVm>
    {
       public MemberSearchReq Model { get; set; }= new MemberSearchReq();
    }

    public class GetMemberPhoneListHandler : IRequestHandler<GetMemberPhoneListQuery, MemberPhoneListVm>
    {
        private readonly IMemDbContext _context;

        public GetMemberPhoneListHandler(IMemDbContext memDbContext)
        {
            _context = memDbContext;
        }

        public async Task<MemberPhoneListVm> Handle(GetMemberPhoneListQuery request, CancellationToken cancellationToken)
        {
            var result = new MemberPhoneListVm();
            try
            {
                string membershipNo = !string.IsNullOrEmpty(request.Model.MemberShipNo) ? request.Model.MemberShipNo.PadLeft(5, '0') : "";

                var data = await _context.RegisterMembers
                 .Include(i => i.Colleges)
                 .Include(i => i.MemberProfessions)
                 .Include(i => i.BloodGroup)
                 .Include(i => i.MemberStatus)
                 .Include(i => i.MemberActiveStatus)
                       .Include(i => i.MemberTypes)
                       .Where(q => q.IsActive && q.IsMasterMember == true
                       && (!string.IsNullOrEmpty(membershipNo) ? q.MembershipNo.Contains(membershipNo) : true)
                       && (!string.IsNullOrEmpty(request.Model.CadetName) ? q.CadetName.Contains(request.Model.CadetName) : true)
                       && (!string.IsNullOrEmpty(request.Model.FullName) ? q.FullName.Contains(request.Model.FullName) : true)
                       && (request.Model.MemberTypeId > 0 ? (q.MemberTypeId == request.Model.MemberTypeId) : true)
                       && (request.Model.MemberActiveStatusId > 0 ? (q.MemberActiveStatusId == request.Model.MemberActiveStatusId) : true)
                       && (request.Model.CollegeId > 0 ? (q.CollegeId == request.Model.CollegeId) : true)
                       && (request.Model.BloodGroupId > 0 ? (q.BloodGroupId == request.Model.BloodGroupId) : true)
                       && (request.Model.MemberProfessionId > 0 ? (q.MemberProfessionId == request.Model.MemberProfessionId) : true)
                       && (!string.IsNullOrEmpty(request.Model.Phone) ? q.Phone.Contains(request.Model.Phone) : true)
                       && (!string.IsNullOrEmpty(request.Model.Email) ? q.Email.Contains(request.Model.Email) : true)
                       && (!string.IsNullOrEmpty(request.Model.BatchNo) ? q.BatchNo.Contains(request.Model.BatchNo) : true)
                       && (!string.IsNullOrEmpty(request.Model.Organaization) ? q.Organaization.Contains(request.Model.Organaization) : true)
                       && (!string.IsNullOrEmpty(request.Model.Designation) ? q.Designation.Contains(request.Model.Designation) : true)
                       && (!string.IsNullOrEmpty(request.Model.Specialization) ? q.Specialization.Contains(request.Model.Specialization) : true)
                       && (!string.IsNullOrEmpty(request.Model.HscYear) ? q.HscYear.Contains(request.Model.HscYear) : true)
                       && (!string.IsNullOrEmpty(request.Model.CadetNo) ? q.CadetNo.Contains(request.Model.CadetNo) : true)
                       && (!string.IsNullOrEmpty(request.Model.memFullId) ? q.MemberFullId.Contains(request.Model.memFullId) : true)
                       )
                       .OrderBy(s => s.MembershipNo).Select(s => new
                       {
                           s.Id,
                           s.FullName,
                           s.MembershipNo,
                           s.Phone,
                           s.Email
                       }).ToListAsync(cancellationToken);

                if (data.Count == 0)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found!");
                }
                else
                {
                    result.DataCount = data.Count;
                    result.HasError = false;
                    result.DataList = data.Select(s => new MemberPhoneReq
                    {
                        Id = s.Id,
                        MemberShipNo = s.MembershipNo ?? "",
                        MemberName = s.FullName,
                        PhoneNo = s.Phone ?? "",
                        MemberEmail = s.Email ?? ""
                    }).ToList();
                }

            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add(ex.Message);
            }
           
            return result;
        }
    }
}
