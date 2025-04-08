using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;


namespace MemApp.Application.Mem.Members.Queries
{
    public class GetMemberShipFeeByIdQuery : IRequest<Result<MemberShipFeeDto>>
    {
        public int Id { get; set; }
    }


    public class GetMemberShipFeeByIdQueryHandler : IRequestHandler<GetMemberShipFeeByIdQuery, Result<MemberShipFeeDto>>
    {
        private readonly IMemDbContext _context;
        public GetMemberShipFeeByIdQueryHandler(IMemDbContext context)
        {
            _context = context;

        }

        public async Task<Result<MemberShipFeeDto>> Handle(GetMemberShipFeeByIdQuery request, CancellationToken cancellationToken)
        {                                                   

            var result = new Result<MemberShipFeeDto>();
            var data = await _context.MemberShipFees
                .Include(i=>i.MemberType)
                .SingleOrDefaultAsync(q => q.Id == request.Id && q.IsActive, cancellationToken);

            if (data == null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;

                result.Data = new MemberShipFeeDto
                {
                    Id = data.Id,
                    Title = data.Title,
                    Amount = data.Amount,
                    DisplayName = data.DisplayName,
                    IsChecked = false
                };
            }

            return result;
        }
    }
}
