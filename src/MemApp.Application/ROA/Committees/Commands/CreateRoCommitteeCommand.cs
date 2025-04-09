using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.Committees.Models;
using MemApp.Application.Mem.Committees.Queries;
using MemApp.Domain.Entities.mem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Res.Domain.Entities.RoaCommittee;
using MemApp.Application.Extensions;
using ResApp.Application.ROA.Committees.Models;
using ResApp.Application.ROA.Committees.Queries;

namespace ResApp.Application.ROA.Committees.Commands
{
    
    public class CreateRoCommitteeCommand : IRequest<Result<RoCommitteeReq>>
    {
        public CommitteeReq Model { get; set; } = new CommitteeReq();
    }

    public class CreateRoCommitteeCommandHandler : IRequestHandler<CreateRoCommitteeCommand, Result<RoCommitteeReq>>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly IUserLogService _userLogService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;

        public CreateRoCommitteeCommandHandler(IMemDbContext context, IMediator mediator, ICurrentUserService currentUserService, IUserLogService userLogService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _userLogService = userLogService;
            _permissionHandler = permissionHandler;
        }
        public async Task<Result<RoCommitteeReq>> Handle(CreateRoCommitteeCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            // var result = new CommitteeVm();
            var result = new Result<RoCommitteeReq>()
            {
                Data = new RoCommitteeReq
                {
                    CommitteeDetails = new List<RoCommitteeDetailReq>()
                }
            };
            try
            {
                var obj = await _context.RoCommittees
               .Include(x => x.CommitteeDetails)
               .SingleOrDefaultAsync(q => q.Id == request.Model.Id, cancellation);

                if (obj == null)
                {
                    //if (!await _permissionHandler.HasRolePermissionAsync(3201))
                    //{
                    //    result.HasError = true;
                    //    result.Messages.Add("Create Committee Permission Denied");
                    //    return result;
                    //}
                    obj = new RoCommittee();
                    obj.IsActive = true;
                    _context.RoCommittees.Add(obj);
                    result.HasError = false;
                    result.Messages.Add("New Committee Created");
                }
                else
                {
                    //if (!await _permissionHandler.HasRolePermissionAsync(3202))
                    //{
                    //    result.HasError = true;
                    //    result.Messages.Add("Update Committee Permission Denied");
                    //    return result;
                    //}
                    result.HasError = false;
                    result.Messages.Add("Committee Updated");
                }

                obj.Title = request.Model.Title;
                obj.CommitteeType = request.Model.CommitteeType;
                obj.CommitteeDate = DateTime.Parse(request.Model.CommitteeDate);
                obj.CommitteeYear = request.Model.CommitteeYear;
               // obj.CommitteeCategoryId = request.Model.CommitteeCategoryId ?? 0;
                obj.CommitteeCategoryId = request.Model.CommitteeCategoryId ;
                obj.DivisionId = request.Model.DivisionId ;
                obj.DistrictId = request.Model.DistrictId ;
                obj.ThanaId = request.Model.ThanaId ;

                await _context.SaveChangesAsync(cancellation);

                List<RoCommitteeDetail> details = new List<RoCommitteeDetail>();
                foreach (var ald in request.Model.CommitteeDetails)
                {
                    var exist = await _context.RoCommitteeDetails.SingleOrDefaultAsync(q => q.Id == ald.Id, cancellation);
                    if (exist == null)
                    {
                        exist = new RoCommitteeDetail()
                        {
                            MemberName = ald.MemberName,
                            CommitteeId = obj.Id,
                            Email = ald.Email,
                            Phone = ald.Phone,
                            ImgFileUrl = ald.ImgFileUrl,
                            Designation = ald.Designation,
                           // IsMasterMember = ald.IsMasterMember,
                            MembershipNo = ald.MemberShipNo,
                        };
                        details.Add(exist);
                    }

                }
                if (obj.CommitteeDetails != null)
                {
                    var tobeDelete = new List<RoCommitteeDetail>();
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
                    _context.RoCommitteeDetails.RemoveRange(tobeDelete);
                }

                _context.RoCommitteeDetails.AddRange(details);

                await _context.SaveChangesAsync(cancellation);


                return await _mediator.Send(new GetRoCommitteeByIdQuery() { Id = obj.Id });
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages?.Add("something wrong" + ex.Message.ToString());
                return result;
            }

        }
    }
}
