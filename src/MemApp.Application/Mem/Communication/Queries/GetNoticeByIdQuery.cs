using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Communication.Models;
using MemApp.Application.Mem.MiscItems.Models;
using MemApp.Application.Mem.MiscItems.Queries;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Communication.Queries
{
    public class GetNoticeByIdQuery : IRequest<Result<NoticeModelDto>>
    {
        public int Id { get; set; }
    }
    public class GetNoticeByIdQueryHandler : IRequestHandler<GetNoticeByIdQuery, Result<NoticeModelDto>>
    {
        private readonly IMemDbContext _context;
        public GetNoticeByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<Result<NoticeModelDto>> Handle(GetNoticeByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new Result<NoticeModelDto>();
            var data = await _context.Notices.FirstOrDefaultAsync(q => q.Id == request.Id, cancellationToken);
            if (data == null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;

                result.Data = new NoticeModelDto
                {
                    Id = data.Id,
                    Title = data.Title,
                    AttachmentType = data.AttachmentType,
                    TextContent = data.TextContent,
                    StartDate = data.StartDate,
                    EndDate = data.EndDate,
                    FileUrl = data.FileUrl
                };
            }

            return result;
        }
    }
}
