using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using System.Text;
using MemApp.Application.Services;
using Dapper;
using MemApp.Application.Mem.MemServices.Models;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetAllServiceTicketQuery : IRequest<ServiceTicketListVm>
    {
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }


    public class GetAllServiceTicketQueryHandler : IRequestHandler<GetAllServiceTicketQuery, ServiceTicketListVm>
    {
        private readonly IMemDbContext _context;
        public GetAllServiceTicketQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceTicketListVm> Handle(GetAllServiceTicketQuery request, CancellationToken cancellationToken)
        {
            var result = new ServiceTicketListVm();
            try
            {
                var data = await _context.ServiceTickets.Where(q => q.IsActive).ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

                if (data.TotalCount == 0)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.DataCount = data.TotalCount;
                    result.DataList = data.Data.Select(s => new ServiceTicketReq
                    {
                        Id = s.Id,
                        Title = s.Title
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
