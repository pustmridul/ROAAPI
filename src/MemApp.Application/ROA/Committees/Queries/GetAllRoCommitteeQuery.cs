using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using ResApp.Application.ROA.Committees.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Models.DTOs;
using MemApp.Application.Mem.Committees.Models;
using MemApp.Domain.Entities.mem;
using Res.Domain.Entities.RoaCommittee;

namespace ResApp.Application.ROA.Committees.Queries
{
   
    public class GetAllRoCommitteeQuery : IRequest<ListResult<RoCommitteeReq>>
    {
        public CommitteeSearchParam Model { get; set; } = new CommitteeSearchParam();

    }


    public class GetAllCommitteeQueryHandler : IRequestHandler<GetAllRoCommitteeQuery, ListResult<RoCommitteeReq>>
    {
        private readonly IMemDbContext _context;
        //private readonly IUserLogService _userLogService;
        //private readonly ICurrentUserService _currentUserService;
        public GetAllCommitteeQueryHandler(IMemDbContext context, ICurrentUserService currentUserService, IUserLogService userLogService)
        {
            _context = context;
            //_currentUserService = currentUserService;
            //_userLogService = userLogService;
        }

        public async Task<ListResult<RoCommitteeReq>> Handle(GetAllRoCommitteeQuery request, CancellationToken cancellationToken)
        {
            // var result = new CommitteeVm();
            var result = new ListResult<RoCommitteeReq>()
            {
                Data=new List<RoCommitteeReq>()
            };
            try
            {
                var data = new List<RoCommittee>();

                if (request.Model.Type == "Executive")
                {
                    var query = _context.RoCommittees
                            .Where(x => x.IsActive && x.CommitteeType==request.Model.Type) // Base filters
                            .AsQueryable(); // Start with IQueryable

                    if(request.Model.Year>0)
                    {
                        query = query.Where(x => x.CommitteeYear == request.Model.Year);
                    }
                    query = query.OrderByDescending(o => o.CommitteeYear);

                    //data= await query.
                    //    Include(x=>x.CommitteeDetails).
                    //    ToListAsync(cancellationToken);

                    // Apply pagination
                    //var data = await query.ToPaginatedListAsync(
                    //    request.Model.PageNo.GetValueOrDefault(),
                    //    request.Model.PageSize.GetValueOrDefault(),
                    //    cancellationToken
                    //);

                    result.Data = await query.Select(s => new RoCommitteeReq
                    {
                        Id = s.Id,
                        Title = s.Title,
                        CommitteeDate = s.CommitteeDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        IsActive = s.IsActive,
                        CommitteeYear = s.CommitteeYear,
                        CommitteeCategoryId = 0,
                        CommitteeCategoryName = "",
                        CommitteeDetails = s.CommitteeDetails.Select(s => new RoCommitteeDetailReq
                        {
                            MemberName = s.MemberName,
                            CommitteeId = s.Id,
                            Email = s.Email,
                            ImgFileUrl = s.ImgFileUrl,
                            Phone = s.Phone,
                            Designation = s.Designation,
                            MembershipNo = s.MembershipNo,

                        }).ToList(),
                    }).ToListAsync(cancellationToken);

                    if (result.Data.Count == 0)
                    {
                        result.HasError = true;
                        result.Messages?.Add("Data Not Found");
                    }

                    result.HasError = false;
                    result.Count = data.Count;
                    return result;

                    //data = await _context.RoCommittees
                    //.Include(i => i.CommitteeDetails)
                    //.Where(q => q.IsActive && q.CommitteeType == request.Type)
                    //.OrderByDescending(o => o.CommitteeYear)
                    //.AsNoTracking()
                    //.ToListAsync(cancellationToken);
                }
                else
                {
                    var query = _context.RoCommittees
                            .Where(x => x.IsActive && x.CommitteeType == request.Model.Type) // Base filters
                            .AsQueryable(); // Start with IQueryable

                    if (request.Model.Year > 0)
                    {
                        query = query.Where(x => x.CommitteeYear == request.Model.Year);
                    }
                    if(request.Model.DivisionId != null || request.Model.DistrictId != null || request.Model.ZoneId != null
                        || request.Model.ThanaId != null || request.Model.MunicipalityId != null || request.Model.UnionInfoId != null || request.Model.WardId != null)
                    {
                        query = query.Where(x => x.DivisionId == request.Model.DivisionId && x.DistrictId == request.Model.DistrictId
                                                && x.ZoneId == request.Model.ZoneId && x.ThanaId == request.Model.ThanaId && x.MunicipalityId == request.Model.MunicipalityId
                                                && x.UnionInfoId == request.Model.UnionInfoId && x.WardId == request.Model.WardId);
                    }

                  
                    //if (request.Model.DivisionId != null && request.Model.DistrictId == null && request.Model.ZoneId==null 
                    //    && request.Model.ThanaId == null && request.Model.MunicipalityId == null && request.Model.UnionInfoId == null && request.Model.WardId == null)
                    //{
                    //    // Only Division level
                    //    query = query.Where(x => x.DivisionId == request.Model.DivisionId && x.DistrictId == null 
                    //                              && x.ZoneId==null && x.ThanaId == null && x.MunicipalityId==null 
                    //                              && x.UnionInfoId==null && x.WardId==null);
                    //}
                    //else if (request.Model.DivisionId != null && request.Model.DistrictId != null && request.Model.ZoneId==null 
                    //    && request.Model.ThanaId == null && request.Model.MunicipalityId == null && request.Model.UnionInfoId == null && request.Model.WardId == null)
                    //{
                    //    // Only District level
                    //    query = query.Where(x => x.DivisionId==request.Model.DivisionId && x.DistrictId == request.Model.DistrictId 
                    //                                && x.ZoneId == null && x.ThanaId == null && x.MunicipalityId == null
                    //                                && x.UnionInfoId == null && x.WardId == null);
                    //}
                    //else if (request.Model.DivisionId != null && request.Model.DistrictId != null && request.Model.ZoneId != null
                    //    && request.Model.ThanaId == null && request.Model.MunicipalityId == null && request.Model.UnionInfoId == null && request.Model.WardId == null)
                    //{
                    //    // Only Zone level
                    //    query = query.Where(x => x.DivisionId == request.Model.DivisionId && x.DistrictId == request.Model.DistrictId && x.ZoneId == request.Model.ZoneId
                    //                                    && x.ThanaId == null && x.MunicipalityId == null
                    //                                    && x.UnionInfoId == null && x.WardId == null);
                    //}
                    //else if (request.Model.DivisionId != null && request.Model.DistrictId != null && request.Model.ZoneId == null && request.Model.ThanaId != null
                    //                                    && request.Model.MunicipalityId == null && request.Model.UnionInfoId == null && request.Model.WardId == null)
                    //{
                    //    // Only Thana level
                    //    query = query.Where(x => x.DivisionId == request.Model.DivisionId && x.DistrictId == request.Model.DistrictId && x.ZoneId == request.Model.ZoneId && x.ThanaId == request.Model.ThanaId
                    //                                     && x.MunicipalityId == null && x.UnionInfoId == null && x.WardId == null);
                    //}

                    //else if (request.Model.DivisionId != null && request.Model.DistrictId != null && request.Model.ZoneId == null && request.Model.ThanaId != null
                    //                                    && request.Model.MunicipalityId != null && request.Model.UnionInfoId == null && request.Model.WardId == null)
                    //{
                    //    // Only Municipality level
                    //    query = query.Where(x => x.DivisionId == request.Model.DivisionId && x.DistrictId == request.Model.DistrictId && x.ZoneId == request.Model.ZoneId && x.ThanaId == request.Model.ThanaId
                    //                                     && x.MunicipalityId == request.Model.MunicipalityId && x.UnionInfoId == null && x.WardId == null);
                    //}

                    //else if (request.Model.DivisionId != null && request.Model.DistrictId != null && request.Model.ZoneId == null && request.Model.ThanaId != null
                    //                                    && request.Model.MunicipalityId == null && request.Model.UnionInfoId != null && request.Model.WardId == null)
                    //{
                    //    // Only Union level
                    //    query = query.Where(x => x.DivisionId == request.Model.DivisionId && x.DistrictId == request.Model.DistrictId && x.ZoneId == request.Model.ZoneId && x.ThanaId == request.Model.ThanaId
                    //                                     && x.MunicipalityId == null && x.UnionInfoId == request.Model.UnionInfoId && x.WardId == null);
                    //}

                    //else if (request.Model.DivisionId != null && request.Model.DistrictId != null && request.Model.ZoneId == null && request.Model.ThanaId != null
                    //                                   && request.Model.MunicipalityId == null && request.Model.UnionInfoId != null && request.Model.WardId != null)
                    //{
                    //    // Only Ward level
                    //    query = query.Where(x => x.DivisionId == request.Model.DivisionId && x.DistrictId == request.Model.DistrictId && x.ZoneId == request.Model.ZoneId && x.ThanaId == request.Model.ThanaId
                    //                                     && x.MunicipalityId == null && x.UnionInfoId == request.Model.UnionInfoId && x.WardId == request.Model.WardId);
                    //}

                    query = query.
                        Include(x=>x.CommitteeDetails).
                        OrderByDescending(o => o.CommitteeYear);

                    //data = await query.
                    //     Include(x => x.CommitteeDetails).
                    //     ToListAsync(cancellationToken);

                    // Apply pagination
                    var paginatedResult = await query.ToPaginatedListAsync(
                        request.Model.PageNo.GetValueOrDefault(),
                        request.Model.PageSize.GetValueOrDefault(),
                        cancellationToken
                    );
                    result.Count = paginatedResult.TotalCount;
                    // result.Data = await query.Select(s => new RoCommitteeReq
                    result.Data = paginatedResult.Data.Select(s => new RoCommitteeReq
                    {
                        Id = s.Id,
                        Title = s.Title,
                        CommitteeDate = s.CommitteeDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        IsActive = s.IsActive,
                        CommitteeYear = s.CommitteeYear,
                        CommitteeCategoryId = s.CommitteeCategoryId,
                        // CommitteeCategoryName = s.CommitteeCategory!.Title,
                        CommitteeCategoryName = s.CommitteeCategoryId != null ? _context.RoCommitteeCategories.FirstOrDefault(x => x.Id == s.CommitteeCategoryId)?.Title : "",
                        DivisionName = s.DivisionId != null ? _context.Divisions.FirstOrDefault(x => x.Id == s.DivisionId)?.EnglishName : "",
                        DistrictName = s.DistrictId != null ? _context.Districts.FirstOrDefault(x => x.Id == s.DistrictId)?.EnglishName : "",
                        ZoneName = s.ZoneId != null ? _context.ZoneInfos.FirstOrDefault(x => x.Id == s.ZoneId)?.EnglishName : "",
                        ThanaName = s.ThanaId != null ? _context.Thanas.FirstOrDefault(x => x.Id == s.ThanaId)?.EnglishName : "",
                        MunicipalityName = s.MunicipalityId != null ? _context.Municipalities.FirstOrDefault(x => x.Id == s.MunicipalityId)?.EnglishName : "",
                        UnionName = s.UnionInfoId != null ? _context.UnionInfos.FirstOrDefault(x => x.Id == s.UnionInfoId)?.EnglishName : "",
                        WardName = s.WardId != null ? _context.Wards.FirstOrDefault(x => x.Id == s.WardId)?.EnglishName : "",
                        CommitteeDetails = s.CommitteeDetails.Select(s => new RoCommitteeDetailReq
                        {
                            MemberName = s.MemberName,
                            CommitteeId = s.Id,
                            Email = s.Email,
                            ImgFileUrl = s.ImgFileUrl,
                            Phone = s.Phone,
                            Designation = s.Designation,
                            MembershipNo = s.MembershipNo,

                        }).ToList(),
                    }).ToList();

                    // query = query.OrderByDescending(o => o.CommitteeYear);
                    // data = await _context.RoCommittees
                    //.Include(i => i.CommitteeCategory)
                    //.Include(i => i.CommitteeDetails)
                    //.Where(q => q.IsActive && q.CommitteeType == request.Type)
                    ////&&
                    ////(!string.IsNullOrEmpty(request.Year) ?
                    ////q.CommitteeYear == request.CommitteeYear : true)
                    ////&& (request.CommitteeCategoryId > 0 ? q.CommitteeCategoryId == request.CommitteeCategoryId : true)
                    ////)
                    // .OrderByDescending(o => o.CommitteeYear)
                    //.AsNoTracking()
                    //.ToListAsync(cancellationToken);
                }



                //if (data.Count == 0)
                //{
                //    result.HasError = true;
                //    result.Messages?.Add("Data Not Found");
                //}
                //else
                //{
                //    result.HasError = false;
                //    result.Count = data.Count;
                //    if (request.Type == "Executive")
                //    {
                //        result.Data = data.Select(s => new RoCommitteeReq
                //        {
                //            Id = s.Id,
                //            Title = s.Title,
                //            CommitteeDate = s.CommitteeDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                //            IsActive = s.IsActive,
                //            CommitteeYear = s.CommitteeYear,
                //            CommitteeCategoryId = 0,
                //            CommitteeCategoryName = "",
                //            CommitteeDetails = s.CommitteeDetails.Select(s => new RoCommitteeDetailReq
                //            {
                //                MemberName = s.MemberName,
                //                CommitteeId = s.Id,
                //                Email = s.Email,
                //                ImgFileUrl = s.ImgFileUrl,
                //                Phone = s.Phone,
                //                Designation = s.Designation,
                //                MembershipNo = s.MembershipNo,
                               
                //            }).ToList(),
                //        }).ToList();
                //    }
                //    else
                //    {
                //        result.Data = data.Select(s => new RoCommitteeReq
                //        {
                //            Id = s.Id,
                //            Title = s.Title,
                //            CommitteeDate = s.CommitteeDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                //            IsActive = s.IsActive,
                //            CommitteeYear = s.CommitteeYear,
                //            CommitteeCategoryId = s.CommitteeCategoryId,
                //            CommitteeCategoryName = s.CommitteeCategory?.Title,
                //            CommitteeDetails = s.CommitteeDetails.Select(s => new RoCommitteeDetailReq
                //            {
                //                MemberName = s.MemberName,
                //                CommitteeId = s.Id,
                //                Email = s.Email,
                //                ImgFileUrl = s.ImgFileUrl,
                //                Phone = s.Phone,
                //                Designation = s.Designation,
                //                MembershipNo = s.MembershipNo,
                              
                //            }).ToList(),
                //        }).ToList();
                //    }
                //}
            }
            catch (Exception ex)
            {
                result.HasError = true;
             //   result.Messages?.Add(ex.ToString());
                result.Messages?.Add("Something went wrong");
            }

            return result;
        }
    }
}
