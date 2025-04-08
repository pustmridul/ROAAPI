using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetAllAvailabilityDetailsQuery :  IRequest<List<AvailabilityDetailVm>>
    {

    }

    public class GetAllAvailabilityDetailsQueryHandler : IRequestHandler<GetAllAvailabilityDetailsQuery, List<AvailabilityDetailVm>>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler; 
        private readonly IMapper _mapper;

        public GetAllAvailabilityDetailsQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler, IMapper mapper)
        {
            _context = context;
            _permissionHandler = permissionHandler;
            _mapper = mapper;
        }

        public async Task<List<AvailabilityDetailVm>> Handle(GetAllAvailabilityDetailsQuery request, CancellationToken cancellationToken)
        {

           var data = await _context.AvailabilityDetails
                 .Where(q => q.IsActive).ToListAsync(cancellationToken);

            var returnData = _mapper.Map<List<AvailabilityDetailVm>>(data);
           return returnData;
        }
    }
}
