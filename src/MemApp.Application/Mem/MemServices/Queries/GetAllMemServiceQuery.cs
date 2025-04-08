using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemServices.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetAllMemServiceQuery : IRequest<ListResult<MemServiceDto>>
    {
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }


    public class GetAllMemServiceQueryHandler : IRequestHandler<GetAllMemServiceQuery, ListResult<MemServiceDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        public GetAllMemServiceQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;
        }

        public async Task<ListResult<MemServiceDto>> Handle(GetAllMemServiceQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<MemServiceDto>();
            try
            {
                var data = await _context.MemServices
                    .Include(x => x.ServiceTickets.Where(c=>c.IsActive))
                    .Include(i=>i.ServiceTypes)
                    .Where(q => q.IsActive)
                    .AsNoTracking()
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

                if (data.TotalCount == 0)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.Count = data.TotalCount;
                    result.Data = data.Data.Select(s => new MemServiceDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        ServiceTypeTitle=s.ServiceTypes.Title,
                        ServiceTypeId=s.ServiceTypeId,
                        ServiceTicketCount = s.ServiceTickets.Count()
                    }).ToList();
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
