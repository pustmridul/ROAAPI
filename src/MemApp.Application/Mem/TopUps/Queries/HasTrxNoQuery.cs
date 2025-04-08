using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.TopUps.Models;
using MemApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.TopUps.Queries
{
    public class HasTrxNoQuery : IRequest<Result>
    {
        public string TrxNo { get; set; }
    }

    public class HasTrxNoQueryHandler : IRequestHandler<HasTrxNoQuery, Result>
    {
        private readonly IMemDbContext _context;

        public HasTrxNoQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(HasTrxNoQuery request, CancellationToken cancellationToken)
        {
            var result = new Result();
            try
            {
                var data = await _context.TopUpDetails.Where(q => q.IsActive && q.TrxNo == request.TrxNo).ToListAsync(cancellationToken);
                    
                if (data.Count<= 0)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.Messages.Add("Trx Found");
                   
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
