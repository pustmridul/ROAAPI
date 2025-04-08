using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemServices.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetAllMemServiceOnlyQuery : IRequest<ListResult<MemServiceDto>>
    {
        public string Type { get; set; } = "Service";
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }


    public class GetAllMemServiceOnlyQueryHandler : IRequestHandler<GetAllMemServiceOnlyQuery, ListResult<MemServiceDto>>
    {
        private readonly IMemDbContext _context;
        public GetAllMemServiceOnlyQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<MemServiceDto>> Handle(GetAllMemServiceOnlyQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<MemServiceDto>();
            try
            {
               var dataList = await _context.MemServices
                    .Include(i=>i.ServiceTypes)
                    .Where(q =>  (request.Type.Length>0 ? q.ServiceTypes.Title ==request.Type : true) && q.IsActive)
                    .ToListAsync(cancellationToken);

                var serviceTicket = await _context.ServiceTickets.Select(s => new
                {
                    s.Id,
                    s.ImgFileUrl,
                    s.MemServiceTypeId,
                    s.MemServiceId
                }).Where(q=>q.MemServiceTypeId==7).ToListAsync(cancellationToken);


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
                        ImgFileUrl= serviceTicket?.FirstOrDefault(q=>q.MemServiceId==s.Id)?.ImgFileUrl??"",
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
