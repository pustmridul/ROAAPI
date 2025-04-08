using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Com.Queries.GetRolesByUser;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Com.Queries.SaveRolesByUser
{

    public class SaveRolesByUserQuery : IRequest<UsersRoleModelListVm>
    {
        public UsersRoleModelListVm Model { get; set; } = new UsersRoleModelListVm();
       
    }

    public class SaveRolesByUserQueryHandler : IRequestHandler<SaveRolesByUserQuery, UsersRoleModelListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        public SaveRolesByUserQueryHandler(IMemDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<UsersRoleModelListVm> Handle(SaveRolesByUserQuery request, CancellationToken cancellationToken)
        {
            var result = new UsersRoleModelListVm();
            //here single

            if (request.Model.DataList.Count > 0)
            {
                var userRoleList = await _context.UserRoleMaps.Where(q => q.UserId == request.Model.DataList[0].UserId).ToListAsync(cancellationToken);
                var userObj= await _context.Users.SingleOrDefaultAsync(q=>q.Id== request.Model.DataList[0].UserId, cancellationToken);
                foreach (var r in request.Model.DataList)
                {
                    if (r.IsChecked)
                    {
                        var obj = userRoleList.FirstOrDefault(q => q.RoleId == r.RoleId);
                        if (obj == null)
                        {
                            obj = new UserRoleMap()
                            {
                                RoleId = r.RoleId,
                                UserId = r.UserId,
                                IsActive = true
                            };
                            _context.UserRoleMaps.Add(obj);
                            var userLog = new UserLog()
                            {
                                UserId=r.UserId.ToString(),
                                UserName= userObj?.UserName??"",
                                LogDate= DateTime.Now,
                                LogText= "Asign Role :" + r.RoleName
                            };
                            _context.UserLogs.Add(userLog);
                        }
                    }
                    else
                    {
                        var exObj = userRoleList.FirstOrDefault(q => q.RoleId == r.RoleId);
                        if (exObj != null)
                        {
                            exObj.IsActive = false;
                            var userLog = new UserLog()
                            {
                                UserId = r.UserId.ToString(),
                                UserName = userObj?.UserName ?? "",
                                LogDate = DateTime.Now,
                                LogText = "Remove Role :" + r.RoleName
                            };
                            _context.UserLogs.Add(userLog);
                        }
                       
                        ///context.UserRoleMaps.Remove(toDelete);
                    }
                    await _context.SaveChangesAsync(cancellationToken);

                }

                return await _mediator.Send(new GetRolesByUserQuery() { UserId = request.Model.DataList[0].UserId });
            }
            else
            {
                result.HasError = true;
                result.Messages.Add("User Id Not Found");

                return result;
            }
           
        }
    }
}
