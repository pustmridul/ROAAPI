using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Committees.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Committees.Queries
{
    public class ExportSubCommitteeInfoQuery : IRequest<ExportCommitteeInfo>
    {
        public int Id { get; set; }
    }

    public class ExportSubCommitteeInfoQueryHandler : IRequestHandler<ExportSubCommitteeInfoQuery, ExportCommitteeInfo>
    {
        private readonly IMemDbContext _context;
        //private readonly IUserLogService _userLogService;
        //private readonly ICurrentUserService _currentUserService;
        public ExportSubCommitteeInfoQueryHandler(IMemDbContext context, ICurrentUserService currentUserService, IUserLogService userLogService)
        {
            _context = context;
            //_currentUserService = currentUserService;
            //_userLogService = userLogService;
        }

        public async Task<ExportCommitteeInfo> Handle(ExportSubCommitteeInfoQuery request, CancellationToken cancellationToken)
        {
            var result = new ExportCommitteeInfo();
            var data = await _context.Committees           
                .SingleOrDefaultAsync(q=>q.Id==request.Id && q.IsActive, cancellationToken);

            var committeeCategory= await _context.CommitteeCategories.ToListAsync(cancellationToken);

            if (data==null)
            {
               
            }
            else
            {
                result.Title= data.Title;
                result.CommitteeYear=data.CommitteeYear;
                result.CommitteeDate = data.CommitteeDate.ToString("yyyy-mm-dd");
                result.CommitteeCategoryName = data.CommitteeCategoryId > 0 ? committeeCategory.SingleOrDefault(s => s.Id == data.CommitteeCategoryId)!.Title : "";         
            }

            return result;
        }
    }
}
