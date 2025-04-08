using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Service.Model;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetAllServiceTypeQuery : IRequest<ServiceTypeListVm>
    {
        public int Id { get; set; }
    }

    public class GetAllServiceTypeQueryHandler : IRequestHandler<GetAllServiceTypeQuery, ServiceTypeListVm>
    {
        private readonly IMemDbContext _context;
        public GetAllServiceTypeQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceTypeListVm> Handle(GetAllServiceTypeQuery request, CancellationToken cancellationToken)
        {
            var result = new ServiceTypeListVm();
            var data = await _context.ServiceTypes.Where(q => q.IsActive).ToListAsync(cancellationToken);

            if (data.Count == 0)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.DataCount = data.Count;
                result.DataList = data.Select(s => new ServiceTypeRes
                {
                    Id = s.Id,
                    Title = s.Title,
                    DisplayName = s.DisplayName,
                    Visible = s.IsActive
                }).ToList();
            }

            return result;
        }
    }
}
