using MediatR;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using MemApp.Application.Mem.Service.Model;
using MemApp.Application.Interfaces;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetAllAvailabilityQuery : IRequest<AvailabilityListVm>
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }


    public class GetAllAvailabilityQueryHandler : IRequestHandler<GetAllAvailabilityQuery, AvailabilityListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;

        public GetAllAvailabilityQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler= permissionHandler;
        }

        public async Task<AvailabilityListVm> Handle(GetAllAvailabilityQuery request, CancellationToken cancellationToken)
        {
            
            var result = new AvailabilityListVm();

            if (!await _permissionHandler.HasRolePermissionAsync(2003))
            {
                result.HasError = true;
                result.Messages?.Add("Availability List Permission Denied");
                return result;
            }
            try
            {
                var data = await _context.Availabilities
                    .Where(q => q.IsActive)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                if (data.Count == 0)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.DataCount = data.Count;
                    result.DataList = data.Select(s => new AvailabilityRes
                    {
                        Id = s.Id,
                        Name = s.Name??"",
                        AvailabilityDate = s.AvailabiltyDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        IsLifeTime= s.IsLifeTime
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
