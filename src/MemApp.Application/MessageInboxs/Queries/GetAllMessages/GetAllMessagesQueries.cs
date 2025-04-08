using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.MessageInboxs.Models;

namespace MemApp.Application.MessageInboxs.Queries.GetAllMessages;


public class GetAllMessagesQuery : IRequest<ListResult<MessageInboxDto>>
{
    public int PageSize { get; set; }
    public int PageNo { get; set; }
}


public class GetAllMessagesQueryHandler : IRequestHandler<GetAllMessagesQuery, ListResult<MessageInboxDto>>
{
    private readonly IMemDbContext _context;
    public GetAllMessagesQueryHandler(IMemDbContext context)
    {
        _context = context;
    }

    public async Task<ListResult<MessageInboxDto>> Handle(GetAllMessagesQuery request, CancellationToken cancellationToken)
    {
        var result = new ListResult<MessageInboxDto>();
        var data = await _context.MessageInboxs
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
                IsRead = s.IsRead
            }).ToList();
        }

        return result;
    }
}
