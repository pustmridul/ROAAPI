using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Committees.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Committees.Queries
{
    public class GetCommitteeByIdQuery : IRequest<CommitteeVm>
    {
        public int Id { get; set; }
    }


    public class GetCommitteeByIdQueryHandler : IRequestHandler<GetCommitteeByIdQuery, CommitteeVm>
    {
        private readonly IMemDbContext _context;
        //private readonly IUserLogService _userLogService;
        //private readonly ICurrentUserService _currentUserService;
        public GetCommitteeByIdQueryHandler(IMemDbContext context, ICurrentUserService currentUserService, IUserLogService userLogService)
        {
            _context = context;
            //_currentUserService = currentUserService;
            //_userLogService = userLogService;
        }

        public async Task<CommitteeVm> Handle(GetCommitteeByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new CommitteeVm();
            var data = await _context.Committees
                .Include(i=>i.CommitteeDetails)
                .SingleOrDefaultAsync(q=>q.Id==request.Id && q.IsActive, cancellationToken);

            var committeeCategory= await _context.CommitteeCategories.ToListAsync(cancellationToken);

            if (data==null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;

                result.Data = new CommitteeReq
                {
                    Id = data.Id,
                    Title = data.Title,
                    CommitteeDate = data.CommitteeDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    CommitteeYear = data.CommitteeYear,
                    CommitteeType = data.CommitteeType,
                    CommitteeCategoryId = data.CommitteeCategoryId,
                    CommitteeCategoryName= data.CommitteeCategoryId>0? committeeCategory.SingleOrDefault(s=>s.Id==data.CommitteeCategoryId).Title : "",
                    CommitteeDetails = data.CommitteeDetails.Select(s=>new CommitteeDetailReq()
                    { 
                        MemberName = s.MemberName,
                        Email = s.Email,
                        CommitteeId=s.CommitteeId,
                        Phone = s.Phone,
                        ImgFileUrl = s.ImgFileUrl,
                        Designation=s.Designation,
                        MemberShipNo=s.MemberShipNo,
                        IsMasterMember=s.IsMasterMember,
                        Id = s.Id                       
                        
                    }).ToList(),
                };
            }

            return result;
        }
    }
}
