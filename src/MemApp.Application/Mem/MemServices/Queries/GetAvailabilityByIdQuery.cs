using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Service.Model;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Queries
{
    public class GetAvailabilityByIdQuery : IRequest<AvailabilityVm>
    {
        public int Id { get; set; }
    }


    public class GetAvailabilityByIdQueryHandler : IRequestHandler<GetAvailabilityByIdQuery, AvailabilityVm>
    {
        private readonly IMemDbContext _context;
        public GetAvailabilityByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<AvailabilityVm> Handle(GetAvailabilityByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new AvailabilityVm();
            try
            {
                var data = await _context.Availabilities
                    .Include(i=>i.AvailabilityDetails.Where(q=>q.IsActive))
                    .SingleOrDefaultAsync(q => q.Id== request.Id && q.IsActive, cancellationToken);

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
                        AvailabilityDetailVms = data.AvailabilityDetails.Select(s => new AvailabilityDetailVm()
                        {
                            AvailabilityId = s.AvailabilityId,
                            IsChecked = s.IsChecked,
                            IsWholeDay = s.IsWholeDay,
                            StartTime = s.StartTime,
                            EndTime = s.EndTime,
                            Title = s.Title,    
                            
                            Id = s.Id
                        }
                        ).ToList(),

                       
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
