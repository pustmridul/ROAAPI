using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Committees.Models;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace MemApp.Application.Mem.Committees.Queries
{
    public class GetAllCommitteeQuery : IRequest<CommitteeListVm>
    {
        public string CommitteeType { get; set; }
        public int? CommitteeCategoryId { get; set; }
        public string? CommitteeYear { get; set;}
    }

    public class GetAllCommitteeQueryHandler : IRequestHandler<GetAllCommitteeQuery, CommitteeListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        public GetAllCommitteeQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;
        }

        public async Task<CommitteeListVm> Handle(GetAllCommitteeQuery request, CancellationToken cancellationToken)
        {
            var result = new CommitteeListVm();
            try
            {
                var data = new List<Committee>();

                if (request.CommitteeType == "Executive")
                {
                    data = await _context.Committees
                    .Include(i => i.CommitteeDetails)
                    .Where(q => q.IsActive && q.CommitteeType == request.CommitteeType
                    &&
                    (!(string.IsNullOrEmpty(request.CommitteeYear) || request.CommitteeYear=="0")?
                    q.CommitteeYear == request.CommitteeYear : true)
                    ).OrderByDescending(o=>o.CommitteeYear)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
                }
                else
                {
                    data = await _context.Committees
                   .Include(i => i.CommitteeCategory)
                   .Include(i => i.CommitteeDetails)
                   .Where(q => q.IsActive && q.CommitteeType == request.CommitteeType &&
                   (!string.IsNullOrEmpty(request.CommitteeYear) ?
                   q.CommitteeYear == request.CommitteeYear : true)
                   && (request.CommitteeCategoryId > 0 ? q.CommitteeCategoryId == request.CommitteeCategoryId : true)
                   ).OrderByDescending (o => o.CommitteeYear)
                   .AsNoTracking()
                   .ToListAsync(cancellationToken);
                }

               

                if (data.Count == 0)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.DataCount = data.Count;
                    if(request.CommitteeType == "Executive")
                    {
                        result.DataList = data.Select(s => new CommitteeReq
                        {
                            Id = s.Id,
                            Title = s.Title,
                            CommitteeDate = s.CommitteeDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            IsActive = s.IsActive,
                            CommitteeYear = s.CommitteeYear,
                            CommitteeCategoryId = 0,
                            CommitteeCategoryName = "",
                            CommitteeDetails = s.CommitteeDetails.Select(s => new CommitteeDetailReq
                            {
                                MemberName = s.MemberName,
                                CommitteeId = s.Id,
                                Email = s.Email,
                                ImgFileUrl = s.ImgFileUrl,
                                Phone = s.Phone,
                                Designation = s.Designation,
                                MemberShipNo = s.MemberShipNo,
                                IsMasterMember=s.IsMasterMember
                            }).ToList(),
                        }).ToList();
                    }
                    else
                    {
                        result.DataList = data.Select(s => new CommitteeReq
                        {
                            Id = s.Id,
                            Title = s.Title,
                            CommitteeDate = s.CommitteeDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            IsActive = s.IsActive,
                            CommitteeYear = s.CommitteeYear,
                            CommitteeCategoryId = s.CommitteeCategoryId,
                            CommitteeCategoryName = s.CommitteeCategory?.Title,
                            CommitteeDetails = s.CommitteeDetails.Select(s => new CommitteeDetailReq
                            {
                                MemberName = s.MemberName,
                                CommitteeId = s.Id,
                                Email = s.Email,
                                ImgFileUrl = s.ImgFileUrl,
                                Phone = s.Phone,
                                Designation = s.Designation,
                                MemberShipNo = s.MemberShipNo,
                                IsMasterMember=s.IsMasterMember
                            }).ToList(),
                        }).ToList();
                    }                  
                }
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
