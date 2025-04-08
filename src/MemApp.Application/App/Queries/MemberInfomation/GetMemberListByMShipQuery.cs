using MediatR;
using MemApp.Application.App.Models;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.App.Queries.MemberInfomation
{
    public class GetMemberListByMShipQuery : IRequest<MemberInfoListVm>
    {
        public string MemberShipNo { get; set; }
    }

    public class GetMemberListByMShipQueryHandler : IRequestHandler<GetMemberListByMShipQuery, MemberInfoListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        public GetMemberListByMShipQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;
        }

        public async Task<MemberInfoListVm> Handle(GetMemberListByMShipQuery request, CancellationToken cancellationToken)
        {
            var result = new MemberInfoListVm();

            //if(!await _permissionHandler.HasRolePermissionAsync())

            var data = await _context.RegisterMembers.Where(x=>x.MembershipNo==request.MemberShipNo).ToListAsync(cancellationToken);
            if (data.Count == 0)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.DataCount = data.Count;
                result.DataList = data.Select(s => new MemberInfoRes
                {
                    Id= s.Id,
                    MemberShipNo = s.MembershipNo,
                    CardNo = s.CardNo,
                    ImgFileUrl = s.ImgFileUrl,
                    FullName= s.FullName,
                    Phone= s.Phone,
                }).ToList();
            }
            return result;
        }
    }
}
