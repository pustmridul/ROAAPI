using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Subscriptions.Command
{
    public class DeleteSubscriptionFeeCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }


    public class DeleteSubscriptionCommandHandler : IRequestHandler<DeleteSubscriptionFeeCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;
        public DeleteSubscriptionCommandHandler(IMemDbContext context, IPermissionHandler permissionHandler, ICurrentUserService currentUserService)
        {
            _context = context;
            _permissionHandler = permissionHandler;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(DeleteSubscriptionFeeCommand request, CancellationToken cancellationToken)
        {
            var result = new Result();
            if(! await _permissionHandler.HasRolePermissionAsync(2704))
            {
                result.HasError = true;
                result.Messages.Add("Delete Subscription Permission Denied");
                return result;
            }
            var data = await _context.SubscriptionFees.SingleOrDefaultAsync(q=>q.Id==request.Id && q.IsActive,cancellationToken);


            if (data==null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
               
                    data.IsActive = false;
               
                await _context.SaveChangesAsync(cancellationToken);
                result.HasError = false;
                result.Messages.Add("Delete Success");
            }

            return result;
        }
    }
}
