using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.TopUps.Models;
using MemApp.Domain.Entities.com;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.TopUps.Command
{
    public class CreateVerifiedTopUpCommand : IRequest<TopUpVm>
    {
        public TopUpReq Model { get; set; } = new TopUpReq();
    }

    public class CreateVerifiedTopUpCommandHandler : IRequestHandler<CreateVerifiedTopUpCommand, TopUpVm>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        public CreateVerifiedTopUpCommandHandler(IMemDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }
        public async Task<TopUpVm> Handle(CreateVerifiedTopUpCommand request, CancellationToken cancellation)
        {
            var result = new TopUpVm();
            try
            {
                var obj = await _context.TopUps
                .SingleOrDefaultAsync(q => q.Id == request.Model.Id);
                if (obj == null)
                {
                    obj = new TopUp();
                    if (request.Model.OfflineTopUp)
                    {
                        obj.MemberShipNo = request.Model.MemberShipNo;
                        obj.CardNo = request.Model.CardNo;
                      //  obj.PaymentMethodText = request.Model.PaymentMethodText;
                        obj.PaymentMode = request.Model.PaymentMode;
                      //  obj.RegisterMemberId = request.Model.RegisterMemberId;
                        obj.MemberId = request.Model.MemberId;
                      //  obj.TopUpAmmount = request.Model.TopUpAmmount;
                        obj.Status = request.Model.Status;
                        _context.TopUps.Add(obj);
                    }
                    else if (request.Model.OnlineTopUp)
                    {

                    }

                }
                if (await _context.SaveChangesAsync(cancellation) > 0)
                {
                    result.HasError = false;
                    result.Messages.Add("Top Up created.");
                }
                else
                {
                    result.HasError = true;
                    result.Messages.Add("Top Up Failed.");
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
