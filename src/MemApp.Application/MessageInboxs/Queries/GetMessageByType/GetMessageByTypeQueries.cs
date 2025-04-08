using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.MessageInboxs.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.MessageInboxs.Queries.GetMessageByType;

public class GetMessageByTypeQueries : IRequest<ListResult<MessageInboxDto>>
{
    public int PageSize { get; set; }
    public int PageNo { get; set; }
    public string MessageType { get; set; } = string.Empty;
}


public class GetMessageByTypeQueriesHandler : IRequestHandler<GetMessageByTypeQueries, ListResult<MessageInboxDto>>
{
    private readonly IMemDbContext _context;
    public GetMessageByTypeQueriesHandler(IMemDbContext context)
    {
        _context = context;
    }

    public async Task<ListResult<MessageInboxDto>> Handle(GetMessageByTypeQueries request, CancellationToken cancellationToken)
    {
        var result = new ListResult<MessageInboxDto>();
        var data = await _context.MessageInboxs
                .Where(q => Convert.ToInt32(q.TypeId) == int.Parse(request.MessageType))
                .AsNoTracking()
            .ToPaginatedListAsync(request.PageNo, request.PageSize, cancellationToken);
        if (data == null)
        {
            result.HasError = true;
            result.Messages.Add("Data Not Found");
        }
        else
        {
            result.HasError = false;

            result.Data = data.Data.Select(s => new MessageInboxDto
            {
                Id = s.Id,
                Message = s.Message,
                MemberId = s.MemberId,
                TypeId = s.TypeId,
            }).ToList();
        }

        return result;
    }
}
