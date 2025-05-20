using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Models.DTOs;

namespace ResApp.Application.ROA.MemberRegistration.Queries
{
    public class GetPendingMemberRegInfoQuery : IRequest<ListResult<MemberRegistrationInfoDto>>
    {
        // public int DivId {  get; set; }
        //public int? pageSize { get; set; } = 10;
        //public int? pageNo { get; set; } = 1;
        public MemberSearchParam Model { get; set; } = new MemberSearchParam();
    }

    public class GetPendingMemberRegInfoQueryHandler : IRequestHandler<GetPendingMemberRegInfoQuery, ListResult<MemberRegistrationInfoDto>>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetPendingMemberRegInfoQueryHandler(IMemDbContext context, ICurrentUserService currentUserService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _currentUserService = currentUserService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ListResult<MemberRegistrationInfoDto>> Handle(GetPendingMemberRegInfoQuery request, CancellationToken cancellationToken)
        {
            var checkAdmin = _currentUserService.Current().UserName;
            var result = new ListResult<MemberRegistrationInfoDto>();

            string baseUrl = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host + _httpContextAccessor.HttpContext.Request.PathBase;

            if (checkAdmin != "Super Admin")
            {
                result.HasError = true;
                result.Messages.Add("Invalid request!!!");
                return result;
            }
            //var data = await _context.MemberRegistrationInfos
            //    //Where(x => x.IsApproved == false)
            //    .ToListAsync(cancellationToken);
            //.ToPaginatedListAsync(request.pageNo.GetValueOrDefault(), request.pageSize.GetValueOrDefault(), new CancellationToken());
            // 

            try
            {
                var query = _context.MemberRegistrationInfos
                             .Where(x => x.IsActive && x.IsFilled && x.IsApproved == false) // Base filters
                             .AsQueryable(); // Start with IQueryable

                if (!string.IsNullOrEmpty(request.Model.SearchText))
                {
                    string searchText = request.Model.SearchText.ToLower();
                    query = query.Where(x => x.Name!.ToLower().Contains(searchText));
                }

                if (!string.IsNullOrEmpty(request.Model.MemberShipNo))
                {
                    string searchText = request.Model.MemberShipNo.ToLower();
                    query = query.Where(x => x.MemberShipNo!.ToLower().Contains(searchText));
                }

                if (!string.IsNullOrEmpty(request.Model.Name))
                {
                    string searchText = request.Model.Name.ToLower();
                    query = query.Where(x => x.Name!.ToLower().Contains(searchText));
                }

                if (!string.IsNullOrEmpty(request.Model.Email))
                {
                    string searchText = request.Model.Email.ToLower();
                    query = query.Where(x => x.Email!.ToLower().Contains(searchText));
                }

                if (!string.IsNullOrEmpty(request.Model.ApplicationNo))
                {
                    query = query.Where(x => x.ApplicationNo == request.Model.ApplicationNo);
                }

                if (!string.IsNullOrEmpty(request.Model.NomineeName))
                {
                    query = query.Where(x => x.NomineeName == request.Model.NomineeName);
                }

                if (!string.IsNullOrEmpty(request.Model.InstituteNameBengali))
                {
                    query = query.Where(x => x.InstituteNameBengali == request.Model.InstituteNameBengali);
                }

                if (!string.IsNullOrEmpty(request.Model.InstituteNameEnglish))
                {
                    query = query.Where(x => x.InstituteNameEnglish == request.Model.InstituteNameEnglish);
                }

                if (!string.IsNullOrEmpty(request.Model.PhoneNo))
                {
                    query = query.Where(x => x.PhoneNo == request.Model.PhoneNo);
                }

                if (!string.IsNullOrEmpty(request.Model.MemberNID))
                {
                    query = query.Where(x => x.MemberNID == request.Model.MemberNID);
                }

                if (!string.IsNullOrEmpty(request.Model.MemberTINNo))
                {
                    query = query.Where(x => x.MemberTINNo == request.Model.MemberTINNo);
                }

                if (!string.IsNullOrEmpty(request.Model.MemberTradeLicense))
                {
                    query = query.Where(x => x.MemberTradeLicense == request.Model.MemberTradeLicense);
                }

                if (request.Model.BusinessStartingDate.HasValue)
                {
                    query = query.Where(x => x.BusinessStartingDate == request.Model.BusinessStartingDate);
                }

                if (request.Model.DivisionId.HasValue)
                {
                    query = query.Where(x => x.DivisionId == request.Model.DivisionId);
                }

                if (request.Model.DistrictId.HasValue)
                {
                    query = query.Where(x => x.DistrictId == request.Model.DistrictId);
                }

                if (request.Model.ThanaId.HasValue)
                {
                    query = query.Where(x => x.ThanaId == request.Model.ThanaId);
                }

                //if (request.Model.IsApproved !=null)
                //{
                //    query = query.Where(x => x.IsApproved == request.Model.IsApproved);
                //}

                //query = query.Where(x => x.IsActive && x.IsFilled);

                // Apply ordering
                query = query.OrderByDescending(o => o.Id);

                // Apply pagination
                var data = await query.ToPaginatedListAsync(
                    request.Model.PageNo.GetValueOrDefault(),
                    request.Model.PageSize.GetValueOrDefault(),
                    cancellationToken
                );


                //        var data = await _context.MemberRegistrationInfos.Where(x => x.IsActive && x.IsFilled &&
                //(!string.IsNullOrEmpty(request.Model.SearchText) ? x.Name!.ToLower().Contains(request.Model.SearchText.ToLower()) : true)).OrderByDescending(o => o.Id)
                //    .ToPaginatedListAsync(request.Model.PageNo.GetValueOrDefault(), request.Model.PageSize.GetValueOrDefault(), cancellationToken);

                if (data.Data.Count == 0)
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
                        ProfileImgPath = s.ImgPath == null ? baseUrl + "/uploadsMembers/test.png" : baseUrl + "/uploadsMembers/" + s.ImgPath,
                        NIDImgPath = s.NIDImgPath == null ? baseUrl + "/uploadsMemberNID/testNID.png" : baseUrl + "/uploadsMemberNID/" + s.NIDImgPath,
                        NomineeName = s.NomineeName,
                        PhoneNo = s.PhoneNo,
                        // ImgFileUrl = s.ImgFileUrl == null ? baseUrl + "/Members/test.jpg" : baseUrl + "/" + s.ImgFileUrl,
                        SignatureImgPath = s.SignatureImgPath == null ? baseUrl + "/uploadsMemberSign/testSign.jpg" : baseUrl + "/uploadsMemberSign/" + s.SignatureImgPath,
                        SignatureUploadingTime = s.SignatureUploadingTime,
                        ThanaId = s.ThanaId,
                        TinImgPath = s.TinImgPath == null ? baseUrl + "/uploadsMemberTIN/testTin.png" : baseUrl + "/uploadsMemberTIN/" + s.TinImgPath,
                        TradeLicenseImgPath = s.TradeLicenseImgPath == null ? baseUrl + "/uploadsMemberTrade/testTrade.jpg" : baseUrl + "/uploadsMemberTrade/" + s.TradeLicenseImgPath,

                        DivisionId = s.DivisionId.GetValueOrDefault(),

                        DivisionName = _context.Divisions.Where(x => x.Id == s.DivisionId).FirstOrDefault()!.EnglishName,
                        DistrictName = _context.Districts.Where(x => x.Id == s.DistrictId).FirstOrDefault()!.EnglishName,
                        ThanaName = _context.Thanas.Where(x => x.Id == s.ThanaId).FirstOrDefault()!.EnglishName,
                        SubscriptionFee = s.SubscriptionFee.GetValueOrDefault(),
                        MembershipFee = s.MembershipFee.GetValueOrDefault(),
                    }).ToList();

                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add("Something went wrong!!!!");
            }



            return result;
        }
    }
}
