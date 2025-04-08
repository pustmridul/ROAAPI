using AutoMapper;
using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using MemApp.Domain.Entities.mem;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;

namespace MemApp.Application.Mem.Subscription.Queries
{
    public class GetMemberSearchQuery : IRequest<MemberResList>
    {
        public MemberSearchReq Model { get; set; } = new MemberSearchReq();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string WebPath { get; set; } = string.Empty;
    }

    public class GetMemberSearchQueryHandler : IRequestHandler<GetMemberSearchQuery, MemberResList>
    {
        private readonly IMemDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetMemberSearchQueryHandler(IMemDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<MemberResList> Handle(GetMemberSearchQuery request, CancellationToken cancellationToken)
        {
            string baseUrl = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host + _httpContextAccessor.HttpContext.Request.PathBase;         

            string membershipNo = !string.IsNullOrEmpty(request.Model?.MemberShipNo) ? request.Model?.MemberShipNo.PadLeft(5,'0') :"";

            var result = new MemberResList();
            var data = await _context.RegisterMembers
            .Include(i => i.Colleges)
            .Include(i => i.MemberProfessions)
            .Include(i => i.BloodGroup)
            .Include(i => i.MemberStatus)
            .Include(i => i.MemberActiveStatus)
                  .Include(i => i.MemberTypes)
                  .Where(q => q.IsActive && q.IsMasterMember==true 
                  && q.CollegeId !=0 
                  && q.MemberProfessionId!=0
                  && q.BloodGroupId !=0 
                  && q.MemberStatusId !=0 
                  && q.MemberActiveStatusId!=0
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
                  .OrderBy(s => s.MembershipNo)
                  .AsNoTracking()
                  .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

         

            if (data.Data.Count == 0)
            {
                result.HasError = true;
                result.Messages?.Add("Data Not Found!");
            }
            else
            {
                result.DataCount = data.TotalCount;               
                result.HasError = false;
                result.DataList = data.Data.Select(s => new MemberRes
                {
                    Id = s.Id,
                    MemberShipNo = s.MembershipNo,
                    Name = s.FullName,
                    CollegeCode = s.Colleges.Code,
                    CollegeName = s.Colleges.Name,
                    CadetName = s.CadetName,
                    CadetNo=s.CadetNo,
                    BatchNo=s.BatchNo,
                    TypeText = s.MemberTypes.Name,
                    StatusText = s.MemberStatus?.Name?? "",
                    ActiveStatusText=s.MemberActiveStatus?.Name,
                    Dbo = s.Dob,
                    BloodGroupText = s.BloodGroup?.Code ?? "",
                    HscYear = s.HscYear,
                    ProfessionText = s.MemberProfessions?.Name ?? "",
                    ImgFileUrl= s.ImgFileUrl==null? baseUrl+"/Members/test.jpg" : baseUrl+ "/"+s.ImgFileUrl,
                    Phone= s.Phone,
                    CardNo= s.CardNo,
                    Email= s.Email,
                    MemberFullId= s.MemberFullId,
                    JoinDate= s.JoinDate,
                    LeaveDate=s.LeaveDate,
                    PermanentAddress=s.PermanentAddress,
                    NID= s.NID,
                    EmergencyContact=s.EmergencyContact
                }).ToList();             
            }
            return result;
        }
    }
}
