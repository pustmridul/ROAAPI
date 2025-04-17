using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ResApp.Application.ROA.MunicipalityInfo.Command
{
    
    public class DeleteMunicipalityCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }

    public class DeleteMunicipalityCommandHandler : IRequestHandler<DeleteMunicipalityCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public DeleteMunicipalityCommandHandler(IMemDbContext context, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }

        public async Task<Result> Handle(DeleteMunicipalityCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new Result();
            if (!await _permissionHandler.HasRolePermissionAsync(3504))
            {
                result.HasError = true;
                result.Messages.Add("Delete Municipality Info Denied");
                return result;
            }

            try
            {
                var data = await _context.Municipalities
                    .SingleOrDefaultAsync(q => q.Id == request.Id && q.IsActive == true, cancellationToken);

                if (data == null)
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

            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add("Error" + ex.Message.ToString());
            }
            return result;
        }
    }
}
