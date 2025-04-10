using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.Colleges.Models;
using Microsoft.EntityFrameworkCore;
using Res.Domain.Entities.RoaCommittee;
using MemApp.Application.Mem.Committees.Models;
using MemApp.Application.Extensions;

namespace ResApp.Application.ROA.CommitteeCategory.Command
{

    public class CreateCommitteeCatCommand : IRequest<Result<CommitteeCatReq>>
    {
        //  public CommitteeCatReq Model { get; set; } = new CommitteeCatReq();
        public string Title { get; set; } = string.Empty;
        public int Id { get; set; }
    }

    public class CreateCommitteeCatCommandHandler : IRequestHandler<CreateCommitteeCatCommand, Result<CommitteeCatReq>>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        public CreateCommitteeCatCommandHandler(IMemDbContext context, IMediator mediator, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
        }
        public async Task<Result<CommitteeCatReq>> Handle(CreateCommitteeCatCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }

            var result = new Result<CommitteeCatReq>()
            {
                Data = new CommitteeCatReq()
            };

            var obj = await _context.RoCommitteeCategories
                .SingleOrDefaultAsync(q => q.Id == request.Id);
            if (obj == null)
            {

                if (!await _permissionHandler.HasRolePermissionAsync(3401))
                {
                    result.HasError = true;
                    result.Messages.Add("Committee Category Create Permission Denied");
                    return result;
                }

                obj = new RoCommitteeCategory();
                obj.IsActive = true;
                _context.RoCommitteeCategories.Add(obj);
                result.HasError = false;
                result.Messages.Add("New Committee Category Created");
            }
            else
            {
                if (!await _permissionHandler.HasRolePermissionAsync(3402))
                {
                    result.HasError = true;
                    result.Messages.Add("Committee Category Update Permission Denied");
                    return result;
                }
                result.HasError = false;
                result.Messages.Add("Committee Category Updated");
            }

            obj.Title = request.Title;

            if (await _context.SaveChangesAsync(cancellation) > 0)
            {

                result.Data.Title = obj.Title;
                result.Data.Id = obj.Id;
            }
            else
            {
                result.HasError = true;
                result.Messages.Add("something wrong");
            }
            return result;
        }
    }
}
