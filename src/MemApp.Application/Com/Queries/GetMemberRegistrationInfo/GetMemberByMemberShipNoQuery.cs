using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.Com.Queries.GetMemberRegistrationInfo
{
    public class GetMemberByMemberShipNoQuery : IRequest<Result<MemberRegistrationInfoDto>>
    {
        public string MemberShipNo {  get; set; }
    }

    public class GetMemberByMemberShipNoQueryHandler : IRequestHandler<GetMemberByMemberShipNoQuery, Result<MemberRegistrationInfoDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetMemberByMemberShipNoQueryHandler(IMemDbContext context,   IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<MemberRegistrationInfoDto>> Handle(GetMemberByMemberShipNoQuery request, CancellationToken cancellationToken)
        {
            var result = new Result<MemberRegistrationInfoDto>();
            string baseUrl = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host + _httpContextAccessor.HttpContext.Request.PathBase;
            var data = await _context.MemberRegistrationInfos.
                Where(x=> x.MemberShipNo== request.MemberShipNo).FirstOrDefaultAsync(cancellationToken);

            if (data == null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
               // result.Data = data;
                result.Data = new MemberRegistrationInfoDto
                {
                    Id = data.Id,
                    ApplicationNo = data.ApplicationNo,
                    MemberShipNo=data.MemberShipNo,
                    PermanentAddress = data.PermanentAddress,
                    IsApproved = data.IsApproved,
                    BusinessStartingDate = data.BusinessStartingDate,
                    CreatedBy = data.CreatedBy,
                    CreatedByName = data.CreatedByName,
                    CreatedOn = data.CreatedOn,
                    DistrictId = data.DistrictId,
                    InstituteNameBengali = data.InstituteNameBengali,
                    InstituteNameEnglish = data.InstituteNameEnglish,
                    MemberNID = data.MemberNID,
                    MemberTINNo = data.MemberTINNo,
                    MemberTradeLicense = data.MemberTradeLicense,
                    Name = data.Name,
                    ProfileImgPath=data.ImgPath == null ? baseUrl + "/uploadsMembers/test.png" : baseUrl + "/uploadsMembers/" + data.ImgPath,
                    IsFilled=data.IsFilled,
                    //NIDImgPath = data.NIDImgPath,
                    NIDImgPath = data.NIDImgPath == null ? baseUrl + "/uploadsMemberNID/testNID.png" : baseUrl + "/uploadsMemberNID/" + data.NIDImgPath,
                    NomineeName = data.NomineeName,
                    PhoneNo = data.PhoneNo,
                    SignatureImgPath = data.SignatureImgPath == null ? baseUrl + "/uploadsMemberSign/testSign.jpg" : baseUrl + "/uploadsMemberSign/" + data.SignatureImgPath,
                    SignatureUploadingTime = data.SignatureUploadingTime,
                    ThanaId = data.ThanaId,
                    TinImgPath = data.TinImgPath == null ? baseUrl + "/uploadsMemberTIN/testTin.png" : baseUrl + "/uploadsMemberTIN/" + data.TinImgPath,
                    TradeLicenseImgPath = data.TradeLicenseImgPath == null ? baseUrl + "/uploadsMemberTrade/testTrade.jpg" : baseUrl + "/uploadsMemberTrade/" + data.TradeLicenseImgPath,

                    DivisionId = data.DivisionId.GetValueOrDefault(),

                    DivisionName = _context.Divisions.Where(x => x.Id == data.DivisionId).FirstOrDefault()!.EnglishName,
                    DistrictName = _context.Districts.Where(x => x.Id == data.DistrictId).FirstOrDefault()!.EnglishName,
                    ThanaName = _context.Thanas.Where(x => x.Id == data.ThanaId).FirstOrDefault()!.EnglishName,
                    SubscriptionFee=data.SubscriptionFee.GetValueOrDefault(),
                    MembershipFee =data.MembershipFee.GetValueOrDefault(),
                    SubscriptionStarts = data.SubscriptionStarts,
                    PaidTill = data.PaidTill,
                    MemberFeePaid=await _context.ROAMembershipFeePayments.AnyAsync(x =>x.MemberId==data.Id, cancellationToken: cancellationToken),
                };
                
            }

            return result;
        }
    }
}
