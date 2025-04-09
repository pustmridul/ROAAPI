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

namespace ResApp.Application.ROA.Committees.Queries
{
    
    public class GetRoCommitteeByIdQuery : IRequest<Result<RoCommitteeReq>>
    {
        public int Id { get; set; }
    }


    public class GetRoCommitteeByIdQueryHandler : IRequestHandler<GetRoCommitteeByIdQuery, Result<RoCommitteeReq>>
    {
        private readonly IMemDbContext _context;
        //private readonly IUserLogService _userLogService;
        //private readonly ICurrentUserService _currentUserService;
        public GetRoCommitteeByIdQueryHandler(IMemDbContext context, ICurrentUserService currentUserService, IUserLogService userLogService)
        {
            _context = context;
            //_currentUserService = currentUserService;
            //_userLogService = userLogService;
        }

        public async Task<Result<RoCommitteeReq>> Handle(GetRoCommitteeByIdQuery request, CancellationToken cancellationToken)
        {
            // var result = new CommitteeVm();
            var result = new Result<RoCommitteeReq>()
            {
                Data= new RoCommitteeReq
                {
                    CommitteeDetails= new List<RoCommitteeDetailReq>()
                }
            };
            var data = await _context.Committees
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
                    CommitteeCategoryName = data.CommitteeCategoryId > 0 ? committeeCategory!.SingleOrDefault(s => s.Id == data.CommitteeCategoryId)!.Title : "",
                    CommitteeDetails = data.CommitteeDetails.Select(s => new RoCommitteeDetailReq()
                    {
                        MemberName = s.MemberName,
                        Email = s.Email,
                        CommitteeId = s.CommitteeId,
                        Phone = s.Phone,
                        ImgFileUrl = s.ImgFileUrl,
                        Designation = s.Designation,
                        MembershipNo = s.MemberShipNo,
                        IsMasterMember = s.IsMasterMember,
                        Id = s.Id

                    }).ToList(),
                };
            }

            return result;
        }
    }
}
