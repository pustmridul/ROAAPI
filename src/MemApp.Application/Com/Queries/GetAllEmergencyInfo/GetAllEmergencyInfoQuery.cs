using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.com.Queries.GetAllEmergencyInfo
{
    public class GetAllEmergencyInfoQuery : IRequest<ListResult<EmergencyInfoReq>>
    {
    }

    public class GetAllEmergencyInfoQueryHandler : IRequestHandler<GetAllEmergencyInfoQuery, ListResult<EmergencyInfoReq>>
    {
        private readonly IMemDbContext _context;
        public GetAllEmergencyInfoQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<EmergencyInfoReq>> Handle(GetAllEmergencyInfoQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<EmergencyInfoReq>();
            try
            {
                var data = await _context.EmergencyInfos
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                if (data.Count == 0)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.Count = data.Count;
                    result.Data = data.Select(s => new EmergencyInfoReq
                    {
                        Id = s.Id,
                        Name = s.Name,
                        ContactNo = s.ContactNo,
                        Address = s.Address,
                    }).ToList();
                }

            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add(ex.Message);

            }

            return result;
        }
    }
}
