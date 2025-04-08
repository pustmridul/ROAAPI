using AutoMapper;
using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Subscription.Model;
using MemApp.Domain.Entities.subscription;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Subscription.Queries
{
    public class GetAllSubscriptionFeeQuery : IRequest<ListResult<SubscriptionFeeDto>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SearchText { get; set; }
    }

    public class GetAllSubscriptionFeeQueryHandler : IRequestHandler<GetAllSubscriptionFeeQuery, ListResult<SubscriptionFeeDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPermissionHandler _permissionHandler;

        public GetAllSubscriptionFeeQueryHandler(IMemDbContext context, IMapper mapper, IPermissionHandler permissionHandler)
        {
            _context = context;
            _mapper = mapper;
            _permissionHandler = permissionHandler;
        }

        public async Task<ListResult<SubscriptionFeeDto>> Handle(GetAllSubscriptionFeeQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<SubscriptionFeeDto>();
            if(!await _permissionHandler.HasRolePermissionAsync(2703))
            {
                result.HasError = true;
                result.Messages?.Add("Permission Denied When Getting Subscription Fee");
                return result;
            }

            var dataList = await _context.SubscriptionFees
                .Include(i=>i.SubscriptionMode)
                .Where(q => q.IsActive)
                .OrderByDescending(q=> q.SubscribedYear).ThenByDescending(q=>q.SubscriptionModId)
                .AsNoTracking()
                .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

            if(dataList.TotalCount > 0)
            {
                result.Data = dataList.Data.Select(s => new SubscriptionFeeDto
                {               
                    Id = s.Id,
                    AbroadFee = s.AbroadFee,
                    LateFee = s.LateFee,
                    PaymentFees=s.SubscriptionFee,
                    SubscribedYear=s.SubscribedYear,
                    PaymentFeesDate=s.StartDate,
                    SubscriptionModName = s.SubscriptionMode.Name,
                    SubscriptionModId = s.SubscriptionModId
                }).ToList();
            }         
            return result;
        }
    }
}
