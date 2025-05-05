using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Com.Queries.GetAllUsers
{

    public class GetAllUserQuery : IRequest<ListResult<UserDto>>
    {
        public string AppId { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? SearchText { get; set; }
    }

    public class GetAllUserQueryHandler : IRequestHandler<GetAllUserQuery, ListResult<UserDto>>
    {
        private readonly IMemDbContext _context;
        public GetAllUserQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<UserDto>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<UserDto>();
            var data = await _context.Users.Where(q=>q.IsActive && q.AppId==request.AppId 
            && (!string.IsNullOrEmpty(request.SearchText)?q.UserName!.ToLower().Contains(request.SearchText.ToLower()) :true)).OrderByDescending(o=>o.Id)
                .ToPaginatedListAsync(request.PageNo, request.PageSize, cancellationToken);

            var dataCount = await _context.Users.Where(q => q.IsActive && q.AppId == request.AppId
            &&  (!string.IsNullOrEmpty(request.SearchText) ? q.UserName!.ToLower().Contains(request.SearchText.ToLower()) : true)).CountAsync(cancellationToken);

            if (data.TotalCount == 0)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
                result.Count = data.TotalCount; // dataCount;
                result.Data = data.Data.Select(s => new UserDto
                {
                    Id = s.Id,
                    UserName = s.UserName!,
                    Name = s.Name??"",
                    PhoneNo= s.PhoneNo!,
                    EmailId = s.EmailId ?? ""
                }).ToList();
            }

            return result;
        }
    }
}
