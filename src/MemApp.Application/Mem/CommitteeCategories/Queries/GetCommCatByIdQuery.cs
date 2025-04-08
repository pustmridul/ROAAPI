using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Colleges.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Colleges.Queries
{
    public class GetCommCatByIdQuery : IRequest<CommCatVm>
    {
        public int Id { get; set; }
    }


    public class GetCommCatByIdQueryHandler : IRequestHandler<GetCommCatByIdQuery, CommCatVm>
    {
        private readonly IMemDbContext _context;
        public GetCommCatByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<CommCatVm> Handle(GetCommCatByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new CommCatVm();
            var data = await _context.CommitteeCategories
                .SingleOrDefaultAsync(q=>q.Id==request.Id, cancellationToken);

            if (data==null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;

                result.Data = new CommCatReq
                {
                    Id = data.Id,
                    Title = data.Title
                };
            }

            return result;
        }
    }
}
