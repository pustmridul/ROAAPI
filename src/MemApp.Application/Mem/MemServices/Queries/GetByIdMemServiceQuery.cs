using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemServices.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetMemServiceByIdQuery : IRequest<ListResult<MemServiceDto>>
    {
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }


    public class GetMemServiceByIdQueryHandler : IRequestHandler<GetMemServiceByIdQuery, ListResult<MemServiceDto>>
    {
        private readonly IMemDbContext _context;
        public GetMemServiceByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<MemServiceDto>> Handle(GetMemServiceByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<MemServiceDto>();
            try
            {
               var dataList = await _context.MemServices
                    .Include(i=>i.ServiceTypes)
                    .Where(q =>  (request.Id>0 ? q.ServiceTypeId ==request.Id: true) && q.IsActive)
                    .ToListAsync(cancellationToken);

                if (dataList == null)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.Data = dataList.Select(s => new MemServiceDto
                    {
                        Id =s.Id,
                        ServiceTypeTitle = s.ServiceTypes.Title,
                        Title = s.Title,
                        ServiceTypeId = s.ServiceTypes.Id
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add(ex.ToString());
            }
            
            return result;
        }
    }
}
