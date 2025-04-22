using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.Committees.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MemApp.Application.Extensions;
using ResApp.Application.ROA.Committees.Models;
using Microsoft.AspNetCore.Http;

namespace ResApp.Application.ROA.Committees.Queries
{
    
    public class GetRoCommitteeByIdQuery : IRequest<Result<RoCommitteeReq>>
    {
        public int Id { get; set; }
    }


    public class GetRoCommitteeByIdQueryHandler : IRequestHandler<GetRoCommitteeByIdQuery, Result<RoCommitteeReq>>
    {
        private readonly IMemDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly IUserLogService _userLogService;
        //private readonly ICurrentUserService _currentUserService;
        public GetRoCommitteeByIdQueryHandler(IMemDbContext context, 
                                              ICurrentUserService currentUserService, IUserLogService userLogService,  IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            //_currentUserService = currentUserService;
            //_userLogService = userLogService;
        }

        public async Task<Result<RoCommitteeReq>> Handle(GetRoCommitteeByIdQuery request, CancellationToken cancellationToken)
        {
            // var result = new CommitteeVm();
            string baseUrl = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host + _httpContextAccessor.HttpContext.Request.PathBase;
            var result = new Result<RoCommitteeReq>()
            {
                Data= new RoCommitteeReq
                {
                    CommitteeDetails= new List<RoCommitteeDetailReq>()
                }
            };
            var data = await _context.RoCommittees
                .Include(i => i.CommitteeDetails)
                .SingleOrDefaultAsync(q => q.Id == request.Id && q.IsActive, cancellationToken);

            var committeeCategory = await _context.CommitteeCategories.ToListAsync(cancellationToken);

            if (data == null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;

                result.Data = new RoCommitteeReq
                {
                    Id = data.Id,
                    Title = data.Title,
                    CommitteeDate = data.CommitteeDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    CommitteeYear = data.CommitteeYear,
                    CommitteeType = data.CommitteeType,
                    CommitteeCategoryId = data.CommitteeCategoryId,
                    DivisionId=data.DivisionId,
                    DistrictId=data.DistrictId,
                    ZoneId=data.ZoneId,
                    ThanaId=data.ThanaId,
                    MunicipalityId=data.MunicipalityId,
                    UnionInfoId=data.UnionInfoId,
                    WardId=data.WardId,
                    CommitteeCategoryName = data.CommitteeCategoryId > 0 ? committeeCategory!.SingleOrDefault(s => s.Id == data.CommitteeCategoryId)!.Title : "",
                    DivisionName = data.DivisionId != null ? _context.Divisions.SingleOrDefault(x=>x.Id==data.DivisionId)!.EnglishName : "",
                    DistrictName = data.DistrictId != null ? _context.Districts.SingleOrDefault(x => x.Id == data.DistrictId)!.EnglishName : "",
                    ThanaName = data.ThanaId != null ? _context.Thanas.SingleOrDefault(x => x.Id == data.ThanaId)!.EnglishName : "",
                    CommitteeDetails = data.CommitteeDetails.Select(s => new RoCommitteeDetailReq()
                    {
                        MemberName = s.MemberName,
                        Email = s.Email,
                        CommitteeId = s.CommitteeId,
                        Phone = s.Phone,
                       // ImgFileUrl = s.ImgFileUrl,
                        ImgFileUrl = s.ImgFileUrl == null ? baseUrl + "/uploadsMembers/test.png" : baseUrl + "/uploadsMembers/" + s.ImgFileUrl,
                        Designation = s.Designation,
                        MembershipNo = s.MembershipNo,
                      //  IsMasterMember = s.IsMasterMember,
                        Id = s.Id

                    }).ToList(),
                };
            }

            return result;
        }
    }
}
