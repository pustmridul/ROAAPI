using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemServices.Queries;
using MemApp.Application.Mem.Service.Model;
using MemApp.Application.Mem.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Services.Queries
{
    public class GetAllServiceQuery : IRequest<ServiceListVm>
    {
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetAllServiceQueryHandler : IRequestHandler<GetAllServiceQuery, ServiceListVm>
    {
        private readonly IMemDbContext _context;
        public GetAllServiceQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceListVm> Handle(GetAllServiceQuery request, CancellationToken cancellationToken)
        {
            var result = new ServiceListVm();
            var data = await _context.ServiceRecords.Where(q => q.IsActive).Include(c=>c.ServiceType).ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

            if (data.TotalCount == 0)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.DataCount = data.TotalCount;
                result.DataList = data.Data.Select(s => new ServiceRes
                {
                    Id = s.Id,
                    Title = s.Title,
                    ServiceTypeId = s.ServiceTypeId,
                    ServiceTypeTitle =  s.ServiceType.Title
                }).ToList();
            }

            return result;
        }
    }
}
