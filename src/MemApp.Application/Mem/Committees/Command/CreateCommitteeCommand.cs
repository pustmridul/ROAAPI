using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Committees.Models;
using MemApp.Application.Mem.Committees.Queries;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Committees.Command
{
    public class CreateCommitteeCommand : IRequest<CommitteeVm>
    {
        public CommitteeReq Model { get; set; } = new CommitteeReq();
    }

    public class CreateCommitteeCommandHandler : IRequestHandler<CreateCommitteeCommand, CommitteeVm>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly IUserLogService _userLogService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;

        public CreateCommitteeCommandHandler(IMemDbContext context, IMediator mediator, ICurrentUserService currentUserService, IUserLogService userLogService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _userLogService = userLogService;
            _permissionHandler = permissionHandler;
        }
        public async Task<CommitteeVm> Handle(CreateCommitteeCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            var result = new CommitteeVm();
            try
            {
                var obj = await _context.Committees
               .Include(x => x.CommitteeDetails)
               .SingleOrDefaultAsync(q => q.Id == request.Model.Id, cancellation);

                if (obj == null)
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(3201))
                    {
                        result.HasError = true;
                        result.Messages.Add("Create Committee Permission Denied");
                        return result;
                    }
                    obj = new Committee();
                    obj.IsActive = true;
                    _context.Committees.Add(obj);
                    result.HasError = false;
                    result.Messages.Add("New Committee Created");
                }
                else
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(3202))
                    {
                        result.HasError = true;
                        result.Messages.Add("Update Committee Permission Denied");
                        return result;
                    }
                    result.HasError = false;
                    result.Messages.Add("Committee Updated");
                }

                obj.Title = request.Model.Title;
                obj.CommitteeType = request.Model.CommitteeType;
                obj.CommitteeDate =DateTime.Parse(request.Model.CommitteeDate);
                obj.CommitteeYear = request.Model.CommitteeYear;
                obj.CommitteeCategoryId = request.Model.CommitteeCategoryId??0;

                await _context.SaveChangesAsync(cancellation);

                List<CommitteeDetail> details = new List<CommitteeDetail>();
                foreach (var ald in request.Model.CommitteeDetails)
                {
                    var exist= await _context.CommitteeDetails.SingleOrDefaultAsync(q=>q.Id==ald.Id, cancellation);
                    if (exist == null)
                    {
                        exist = new CommitteeDetail()
                        {
                            MemberName = ald.MemberName,
                            CommitteeId = obj.Id,
                            Email = ald.Email,
                            Phone = ald.Phone,
                            ImgFileUrl = ald.ImgFileUrl,
                            Designation = ald.Designation,
                            IsMasterMember = ald.IsMasterMember,
                            MemberShipNo = ald.MemberShipNo,
                        };
                        details.Add(exist);
                    }                  
                   
                }
                if(obj.CommitteeDetails != null)
                {
                    var tobeDelete = new List<CommitteeDetail>();
                    foreach (var ad in obj.CommitteeDetails)
                    {
                        var del = request.Model.CommitteeDetails.Where(q => q.Id == ad.Id).FirstOrDefault();
                        if (del == null)
                        {
                            var de = obj.CommitteeDetails.SingleOrDefault(q => q.Id == ad.Id);
                            if (de != null)
                                tobeDelete.Add(de);
                        }
                    }
                    _context.CommitteeDetails.RemoveRange(tobeDelete);
                }
              
                _context.CommitteeDetails.AddRange(details);

                await _context.SaveChangesAsync(cancellation);

               
                return await _mediator.Send(new GetCommitteeByIdQuery() { Id = obj.Id });
            }
            catch   (Exception ex)
            {
                result.HasError = true;
                result.Messages?.Add("something wrong"+ ex.Message.ToString());
                 return result;
            }
            
        }
    }

}
