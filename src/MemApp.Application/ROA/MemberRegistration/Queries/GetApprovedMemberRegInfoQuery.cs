using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Models.DTOs;

namespace ResApp.Application.ROA.MemberRegistration.Queries
{
    public class GetApprovedMemberRegInfoQuery : IRequest<ListResult<MemberRegistrationInfoDto>>
    {
        // public int DivId {  get; set; }
        public string? AppId { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? SearchText { get; set; }
    }

    public class GetApprovedMemberRegInfoQueryHandler : IRequestHandler<GetApprovedMemberRegInfoQuery, ListResult<MemberRegistrationInfoDto>>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public GetApprovedMemberRegInfoQueryHandler(IMemDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ListResult<MemberRegistrationInfoDto>> Handle(GetApprovedMemberRegInfoQuery request, CancellationToken cancellationToken)
        {
            var checkAdmin = _currentUserService.Current().UserName;
            var result = new ListResult<MemberRegistrationInfoDto>();

            if (checkAdmin != "Super Admin")
            {
                result.HasError = true;
                result.Messages.Add("Invalid request!!!");
                return result;
            }
            //var data = await _context.MemberRegistrationInfos.
            //    Where(x => x.IsApproved == true).ToListAsync(cancellationToken);
            //.ToPaginatedListAsync(request.pageNo.GetValueOrDefault(), request.pageSize.GetValueOrDefault(), new CancellationToken());
            // 

            var data = await _context.MemberRegistrationInfos.Where(x => x.IsApproved && x.IsActive &&
           (!string.IsNullOrEmpty(request.SearchText) ? x.Name!.ToLower().Contains(request.SearchText.ToLower()) : true)).OrderByDescending(o => o.Id)
               .ToPaginatedListAsync(request.PageNo, request.PageSize, cancellationToken);


            if (data.TotalCount == 0)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.Count = data.TotalCount;
                result.Data = data.Data.Select(s => new MemberRegistrationInfoDto
                {
                    Id = s.Id,
                    ApplicationNo = s.ApplicationNo,
                    PermanentAddress = s.PermanentAddress,
                    IsApproved = s.IsApproved,
                    BusinessStartingDate = s.BusinessStartingDate,
                    CreatedBy = s.CreatedBy,
                    CreatedByName = s.CreatedByName,
                    CreatedOn = s.CreatedOn,
                    DistrictId = s.DistrictId,
                    InstituteNameBengali = s.InstituteNameBengali,
                    InstituteNameEnglish = s.InstituteNameEnglish,
                    MemberNID = s.MemberNID,
                    MemberTINNo = s.MemberTINNo,
                    MemberTradeLicense = s.MemberTradeLicense,
                    Name = s.Name,
                    NIDImgPath = s.NIDImgPath,
                    NomineeName = s.NomineeName,
                    PhoneNo = s.PhoneNo,
                    SignatureImgPath = s.SignatureImgPath,
                    SignatureUploadingTime = s.SignatureUploadingTime,
                    ThanaId = s.ThanaId,
                    TinImgPath = s.TinImgPath,
                    TradeLicenseImgPath = s.TradeLicenseImgPath,

                    DivisionId = s.DivisionId.GetValueOrDefault(),

                    DivisionName = _context.Divisions.Where(x => x.Id == s.DivisionId).FirstOrDefault()!.EnglishName,
                    DistrictName = _context.Districts.Where(x => x.Id == s.DistrictId).FirstOrDefault()!.EnglishName,
                    ThanaName = _context.Thanas.Where(x => x.Id == s.ThanaId).FirstOrDefault()!.EnglishName,
                }).ToList();

            }

            return result;
        }
    }
}
