using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Sales.Command
{
    public class VerifySaleCommand : IRequest<Result>
    {
        public int Id { get; set; } 
    }

    public class VerifySaleCommandHandler : IRequestHandler<VerifySaleCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public VerifySaleCommandHandler(IMemDbContext context,ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }
        public async Task<Result> Handle(VerifySaleCommand request, CancellationToken cancellation)
        {
            var result = new Result();
            try
            {
                var obj = await _context.SaleMasters
                .SingleOrDefaultAsync(q => q.Id == request.Id && q.InvoiceStatus!="Verify" && q.IsActive);
                if(obj == null)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found.");
                }
                else
                {
                    obj.InvoiceStatus = "Verify";                 
                }

                if (await _context.SaveChangesAsync(cancellation) > 0)
                {
                    result.HasError = false;
                    result.Messages.Add("Top Up Verify.");
                }
                else
                {
                    result.HasError = true;
                    result.Messages.Add("Top Up Verify Failed.");
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add("Exception" + ex.Message);
            }
            return result;
        }
    }
}
