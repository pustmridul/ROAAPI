using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.MemberTypes.Queries
{
    public class GetAllBloodGroupQuery : IRequest<BloodGroupListVm>
    {
    }

    public class GetAllBloodGroupQueryHandler : IRequestHandler<GetAllBloodGroupQuery, BloodGroupListVm>
    {
        private readonly IMemDbContext _context;
        public GetAllBloodGroupQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<BloodGroupListVm> Handle(GetAllBloodGroupQuery request, CancellationToken cancellationToken)
        {
            var result = new BloodGroupListVm();
            var data = await _context.BloodGroups.ToListAsync(cancellationToken);

            if (data.Count==0)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.DataCount= data.Count;
                result.DataList =data.Select(s=> new BloodGroupRes
                {
                    Id = s.Id,
                    Code = s.Code
                }).ToList();
            }

            return result;
        }
    }
}
