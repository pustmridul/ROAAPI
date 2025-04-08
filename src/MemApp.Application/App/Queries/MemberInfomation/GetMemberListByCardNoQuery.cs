using MediatR;
using MemApp.Application.App.Models;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.App.Queries.MemberInfomation
{
    public class GetMemberListByCardNoQuery : IRequest<MemberInfoListVm>
    {
        public string CardNo { get; set; }
    }

    public class GetMemberListByCardNoQueryHandler : IRequestHandler<GetMemberListByCardNoQuery, MemberInfoListVm>
    {
        private readonly IMemDbContext _context;
        public GetMemberListByCardNoQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<MemberInfoListVm> Handle(GetMemberListByCardNoQuery request, CancellationToken cancellationToken)
        {
            var result = new MemberInfoListVm();

            //if(!await _permissionHandler.HasRolePermissionAsync())

            var data = await _context.RegisterMembers.Where(x=>x.CardNo==request.CardNo).ToListAsync(cancellationToken);
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
                    Email=s.Email,
                    FullName= s.FullName,
                    Phone= s.Phone,
                }).ToList();
            }
            return result;
        }
    }
}
