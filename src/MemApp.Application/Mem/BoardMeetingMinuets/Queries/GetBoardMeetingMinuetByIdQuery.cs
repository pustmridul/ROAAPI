using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.BoardMeetingMinuets.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace MemApp.Application.Mem.BoardMeetingMinuets.Queries
{
    public class GetBoardMeetingMinuetByIdQuery : IRequest<Result<BoardMeetingMinuetDto>>
    {
        public int Id { get; set; }
        public string WebRootPath { get; set; }
    }


    public class GetBoardMeetingMinuetByIdQueryHandler : IRequestHandler<GetBoardMeetingMinuetByIdQuery, Result<BoardMeetingMinuetDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetBoardMeetingMinuetByIdQueryHandler(IMemDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<BoardMeetingMinuetDto>> Handle(GetBoardMeetingMinuetByIdQuery request, CancellationToken cancellationToken)
        {
            string baseUrl = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host + _httpContextAccessor.HttpContext.Request.PathBase;

            var result = new Result<BoardMeetingMinuetDto>();
            var data = await _context.BoardMeetingMinuets.SingleOrDefaultAsync(q => q.Id == request.Id, cancellationToken);
            if (data == null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;

                result.Data = new BoardMeetingMinuetDto
                {
                    Id = data.Id,
                    Title=data.Title,
                    FileUrl=baseUrl+"/"+ data.FileUrl,
                    MeetingDate= data.MeetingDate,
                    Note=data.Note,
                };
            }

            return result;
        }
    }
}
