﻿using MediatR;
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
        public int Id { get; set; }
        public string? Type { get; set; }
        public int? Year { get; set; }
        public int? DivisionId { get; set; }
        public int? DistrictId { get; set; }
        public int? ThanaId { get; set; }
        public int? CategoryId { get; set; }

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

                if (request.Type == "Executive")
                {
                    var query = _context.RoCommittees
                            .Where(x => x.IsActive && x.CommitteeType==request.Type) // Base filters
                            .AsQueryable(); // Start with IQueryable

                    if(request.Year>0)
                    {
                        query = query.Where(x => x.CommitteeYear == request.Year);
                    }
                    query = query.OrderByDescending(o => o.CommitteeYear);

                    //data= await query.
                    //    Include(x=>x.CommitteeDetails).
                    //    ToListAsync(cancellationToken);

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
                            .Where(x => x.IsActive && x.CommitteeType == request.Type) // Base filters
                            .AsQueryable(); // Start with IQueryable

                    if (request.Year > 0)
                    {
                        query = query.Where(x => x.CommitteeYear == request.Year);
                    }

                    if (request.DivisionId != null && request.DistrictId == null && request.ThanaId == null)
                    {
                        // Only Division level
                        query = query.Where(x => x.DivisionId == request.DivisionId && x.DistrictId == null && x.ThanaId == null);
                    }
                    else if (request.DivisionId != null && request.DistrictId != null && request.ThanaId == null)
                    {
                        // Only District level
                        query = query.Where(x => x.DivisionId==request.DivisionId && x.DistrictId == request.DistrictId && x.ThanaId == null);
                    }
                    else if (request.DivisionId != null && request.DistrictId != null && request.ThanaId != null)
                    {
                        // Thana level
                        query = query.Where(x => x.DivisionId == request.DivisionId && x.DistrictId == request.DistrictId && x.ThanaId == request.ThanaId);
                    }

                    query = query.OrderByDescending(o => o.CommitteeYear);

                    data = await query.
                         Include(x => x.CommitteeDetails).
                         ToListAsync(cancellationToken);

                    result.Data = await query.Select(s => new RoCommitteeReq
                    {
                        Id = s.Id,
                        Title = s.Title,
                        CommitteeDate = s.CommitteeDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        IsActive = s.IsActive,
                        CommitteeYear = s.CommitteeYear,
                        CommitteeCategoryId = s.CommitteeCategoryId,
                        CommitteeCategoryName = s.CommitteeCategory!.Title,
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
                result.Messages?.Add(ex.ToString());
            }

            return result;
        }
    }
}
