using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.AreaLayouts.Models;
using MemApp.Application.Mem.AreaLayouts.Queries;
using MemApp.Application.Mem.BoardMeetingMinuets.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.BoardMeetingMinuets.Queries
{
    public class GetAllBoardMeetingMinuetQuery : IRequest<ListResult<BoardMeetingMinuetDto>>
    {
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string WebRootPath { get; set; } = string.Empty;
    }


    public class GetAllBoardMeetingMinuetQueryHandler : IRequestHandler<GetAllBoardMeetingMinuetQuery, ListResult<BoardMeetingMinuetDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPermissionHandler _permissionHandler;
        public GetAllBoardMeetingMinuetQueryHandler(IMemDbContext context, IHttpContextAccessor httpContextAccessor, IPermissionHandler permissionHandler)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _permissionHandler = permissionHandler;
        }

        public async Task<ListResult<BoardMeetingMinuetDto>> Handle(GetAllBoardMeetingMinuetQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<BoardMeetingMinuetDto>();
            if(!await _permissionHandler.HasRolePermissionAsync(3503))
            {
                result.HasError = true;
                result.Messages?.Add("Board Meeting List Permission Denied");
                return result;
            }
            string baseUrl = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host + _httpContextAccessor.HttpContext.Request.PathBase;
            try
            {
                var data = await _context.BoardMeetingMinuets
                    .Where(q => q.IsActive)
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

                if (data.TotalCount == 0)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.Count = data.TotalCount;
                    result.Data = data.Data.Select(s => new BoardMeetingMinuetDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        FileUrl = baseUrl + "/"+ s.FileUrl,
                        MeetingDate = s.MeetingDate,
                        Note=s.Note,
                        IsActive = s.IsActive,
                        FileName=s.FileName
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


