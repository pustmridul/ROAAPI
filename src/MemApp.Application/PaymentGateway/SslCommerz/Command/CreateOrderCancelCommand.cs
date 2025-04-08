using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models;
using MemApp.Application.PaymentGateway.SslCommerz.Model;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.PaymentGateway.SslCommerz.Command
{
    public class CreateOrderCancelCommand:IRequest<Result>
    {
        public SSLCommerzValidatorResponseReq Model { get; set; } = new SSLCommerzValidatorResponseReq();
    }
    public class CreateOrderCancelCommandHandler : IRequestHandler<CreateOrderCancelCommand, Result>
    {
        private readonly IMemDbContext _context;
        public CreateOrderCancelCommandHandler(IMemDbContext context)
        {
            _context= context;
        }
        public async Task<Result> Handle(CreateOrderCancelCommand request, CancellationToken cancellationToken)
        {
            var result = new Result();
            try
            {
                var tobj = await _context.TopUps
                    .Include(i => i.TopUpDetails)
                    .SingleOrDefaultAsync(q => q.Note == request.Model.tran_id, cancellationToken);
                if (tobj != null)
                {
                    if (tobj.Status == "Confirm")
                    {
                        result.HasError = false;
                        result.Messages.Add("Transaction is already completed");
                        return result;
                    }
                    else if (tobj.Status == "Cancel")
                    {
                        result.HasError = false;
                        result.Messages.Add("Transaction is already Cancel");
                        return result;
                    }
                    else if(tobj.Status=="Pending" && request.Model.status == "CANCELLED")
                    {
                        tobj.Status = "Cancel";
                        tobj.IsActive = false;
                        foreach (var d in tobj.TopUpDetails)
                        {
                            d.IsActive = false;
                        }
                        await _context.SaveChangesAsync(cancellationToken);
                        result.HasError = false;
                        result.Messages.Add("Transaction is Canceled");
                    }
                    else if(request.Model.status == "CANCELLED" && tobj.Status== "Cancel")
                    {
                        result.HasError = false;
                        result.Messages.Add("Transaction is already Canceled");
                    }
                    else
                    {
                        result.HasError = true;
                        result.Messages.Add("Transaction is invalid");
                    }
                   
                }
                else
                {
                    result.HasError= true;
                    result.Messages.Add("Data not Found");
                }
                return result;
            }
            catch (Exception ex) { 
             throw new Exception(ex.Message);
            }

        }
    }
}
