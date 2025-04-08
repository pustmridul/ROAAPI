using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Application.Mem.Service.Model;
using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetAvailabilityByDateQuery : IRequest<AvailabilityVm>
    {
        public DateTime AvailabilityDate  { get; set; }
        public int ServiceId { get; set; }
        public int ServiceTypeId { get; set; }
    }


    public class GetAvailabilityByDateQueryHandler : IRequestHandler<GetAvailabilityByDateQuery, AvailabilityVm>
    {
        private readonly IMemDbContext _context;
        public GetAvailabilityByDateQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<AvailabilityVm> Handle(GetAvailabilityByDateQuery request, CancellationToken cancellationToken)
        {
            var result = new AvailabilityVm();
            try
            {
                var data = await _context.Availabilities
                    .Include(i=>i.AvailabilityDetails)
                    .FirstOrDefaultAsync(q =>
                    q.IsActive 
                    && q.AvailabiltyDate.Date == request.AvailabilityDate.Date ,cancellationToken);

                if (data == null)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.Data = new AvailabilityRes
                    {
                        Id = data.Id,
                        AvailabilityDate = data.AvailabiltyDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        Name = data.Name,
                        IsLifeTime = data.IsLifeTime,
                        AvailabilityDetailVms = data.AvailabilityDetails.Select(s => new AvailabilityDetailVm
                        {
                            StartTime = s.StartTime,
                            EndTime = s.EndTime,
                            IsChecked = s.IsChecked,
                            IsWholeDay = s.IsWholeDay,
                            Title = s.Title,
                            AvailabilityId = s.AvailabilityId,
                            Id = data.Id,
                        }).ToList()
                       
                    };
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
