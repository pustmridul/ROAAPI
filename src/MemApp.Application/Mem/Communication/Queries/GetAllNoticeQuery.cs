using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Communication.Models;
using Microsoft.EntityFrameworkCore;
using MemApp.Application.Extensions;
using Microsoft.AspNetCore.Http;

namespace MemApp.Application.Mem.Communication.Queries
{
    public class GetAllNoticeQuery : IRequest<ListResult<NoticeModelDto>>
    {
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class GetAllNoticeQueryHandler : IRequestHandler<GetAllNoticeQuery, ListResult<NoticeModelDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public GetAllNoticeQueryHandler(IMemDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ListResult<NoticeModelDto>> Handle(GetAllNoticeQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<NoticeModelDto>();
            try
            {
                string baseUrl = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host + _httpContextAccessor.HttpContext.Request.PathBase;

                var data = await _context.Notices
                    .AsNoTracking()
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

                if (data.TotalCount == 0)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.Count = Convert.ToInt32(data.TotalCount);
                    result.Data = data.Data.Select(s => new NoticeModelDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        AttachmentType = s.AttachmentType,
                        TextContent = s.TextContent,
                        StartDate = s.StartDate,
                        EndDate = s.EndDate,
                        FileUrl = baseUrl + "/" + s.FileUrl,

                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages?.Add(ex.ToString());
            }

            return result;
        }
    }
}
