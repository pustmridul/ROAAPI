using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Booking.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Booking.Queries
{
    public class GetTramsAndConditionQuery : IRequest<TramsAndConditionVm>
    {
    }


    public class GetTramsAndConditionQueryHandler : IRequestHandler<GetTramsAndConditionQuery, TramsAndConditionVm>
    {
        private readonly IMemDbContext _context;
        public GetTramsAndConditionQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<TramsAndConditionVm> Handle(GetTramsAndConditionQuery request, CancellationToken cancellationToken)
        {
            var result= new TramsAndConditionVm();

            result.Title = "Terms and Conditions (Venue Bookiung)";
            try
            {
                var data = await _context.TramsAndConditions
               .Where(x => x.IsActive && x.TypeId == 1)
               .OrderBy(o => o.SlNo)
               .ToListAsync(cancellationToken);
                if (data != null)
                {
                    foreach (var d in data)
                    {
                        result.Lists.Add(d.Title);
                    }
                }
            }
            catch (Exception ex) {
                throw new Exception(ex.ToString());
            }
            return result;
        }
    }
}

