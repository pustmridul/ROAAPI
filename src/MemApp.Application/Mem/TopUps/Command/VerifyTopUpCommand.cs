using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.TopUps.Models;
using MemApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.TopUps.Command
{
    public class VerifyTopUpCommand : IRequest<Result>
    {
        public int MemberId { get; set; } 
        public string TrxNo { get; set; }

    }

    public class VerifyTopUpCommandHandler : IRequestHandler<VerifyTopUpCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public VerifyTopUpCommandHandler(IMemDbContext context,ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }
        public async Task<Result> Handle(VerifyTopUpCommand request, CancellationToken cancellation)
        {
            var result = new Result();
            try
            {
                var obj = await _context.TopUps
                .SingleOrDefaultAsync(q => q.MemberId == request.MemberId && q.Note == request.TrxNo && q.IsActive);
                if(obj == null)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found.");
                }
                else
                {
                    obj.Status = "Verify";
                    obj.VerifiedAt = DateTime.Now;
                    obj.VerifiedBy = _currentUserService.UserId;
                    obj.VerifierName = _currentUserService.Username;
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
