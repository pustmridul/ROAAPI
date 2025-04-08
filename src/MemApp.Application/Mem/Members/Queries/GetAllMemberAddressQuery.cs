using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemberAddresss.Queries
{
    public class GetAllMemberAddressQuery : IRequest<ListResult<MemberAddressDto>>
    {
        public string AddressType { get; set; }
    }

    public class GetAllMemberAddressQueryHandler : IRequestHandler<GetAllMemberAddressQuery, ListResult<MemberAddressDto>>
    {
        private readonly IMemDbContext _context;
        public GetAllMemberAddressQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<MemberAddressDto>> Handle(GetAllMemberAddressQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<MemberAddressDto>();
            var data = await _context.MemberAddresses.Where(q => q.IsActive && q.Type == request.AddressType).ToListAsync(cancellationToken);

            if (data.Count == 0)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.Count = data.Count;
                result.Data = data.Select(s => new MemberAddressDto
                {
                    Id = s.Id,
                    Title = s.Title
                }).ToList();
            }

            return result;
        }
    }
}
