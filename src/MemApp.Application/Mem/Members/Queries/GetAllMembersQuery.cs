using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Members.Queries
{
    public class GetAllMembersQuery : IRequest<MemberResList>
    {
        public string? SearchText { get; set; }
        public string? webRootPath { get; set; }
     
    }

    public class GetAllMembersQueryHandler : IRequestHandler<GetAllMembersQuery, MemberResList>
    {
        private readonly IMemDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetAllMembersQueryHandler(IMemDbContext memDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _context = memDbContext;
            _httpContextAccessor = httpContextAccessor;
        }
          
        public async Task<MemberResList> Handle(GetAllMembersQuery request, CancellationToken cancellationToken)
        {
            var result = new MemberResList();         
            string membershipNo = !string.IsNullOrEmpty(request.SearchText) ? request.SearchText.PadLeft(5, '0') : "";
            string baseUrl = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host + _httpContextAccessor.HttpContext.Request.PathBase;


            result.DataList = await _context.RegisterMembers.Include(i=>i.MemberTypes).Select(s => new MemberRes
            {
                Name = s.FullName,
                Phone = s.Phone,
                Email = s.Email,
                Designation = s.Designation,
                MemberShipNo = s.MembershipNo,
                IsActive = s.IsActive,
                IsMasterMember = s.IsMasterMember,
                ImgFileUrl =  s.ImgFileUrl,
                TypeText= s.MemberTypes.Name             
            }).Where(q => q.IsActive && q.MemberShipNo == membershipNo
            ).AsNoTracking()
            .ToListAsync(cancellationToken);
            result.DataCount= result.DataList.Count;
            return result;
        }
    }
}
