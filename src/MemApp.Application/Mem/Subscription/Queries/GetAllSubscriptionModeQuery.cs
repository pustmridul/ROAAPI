using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Service.Model;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Subscription.Queries
{

    public class GetAllSubscriptionModeQuery : IRequest<ListResult<SubscriptionModDto>>
    {
        public int Id { get; set; }
    }

    public class GetAllSubscriptionModeQueryHandler : IRequestHandler<GetAllSubscriptionModeQuery, ListResult<SubscriptionModDto>>
    {
        private readonly IMemDbContext _context;
        public GetAllSubscriptionModeQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<SubscriptionModDto>> Handle(GetAllSubscriptionModeQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<SubscriptionModDto>();
            var data = await _context.SubscriptionModes.ToListAsync(cancellationToken);

            if (data.Count == 0)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.Count = data.Count;
                result.Data = data.Select(s => new SubscriptionModDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Value = s.Value
                }).ToList();
            }

            return result;
        }
    }
}
