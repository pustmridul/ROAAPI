using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.MessageInboxs.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.MessageInboxs.Queries.GetMessageByMemberId;
public class GetMessageByMemberIdQueries : IRequest<ListResult<MessageInboxDto>>
{
    
    public MessageInboxQueryDto Model { get; set; }

}


public class GetMessageByMemberIdQueriesHandler : IRequestHandler<GetMessageByMemberIdQueries, ListResult<MessageInboxDto>>
{
    private readonly IMemDbContext _context;
    public GetMessageByMemberIdQueriesHandler(IMemDbContext context)
    {
        _context = context;
    }

    public async Task<ListResult<MessageInboxDto>> Handle(GetMessageByMemberIdQueries request, CancellationToken cancellationToken)
    {
        var result = new ListResult<MessageInboxDto>();
        var query = _context.MessageInboxs
    .Where(q => q.MemberId == request.Model.MemberId)
    .OrderByDescending(c=>c.Id)
    .AsNoTracking();

        if (request.Model.IsRead != null) // Check if IsRead is not null
        {
            query = query.Where(q => q.IsRead == request.Model.IsRead);
        }

        var data = await query.ToPaginatedListAsync(request.Model.PageNo, request.Model.PageSize, cancellationToken);

        if (data == null)
        {
            result.HasError = true;
            result.Messages.Add("Data Not Found");
        }
        else
        {
            result.HasError = false;
            result.Succeeded = true;

            result.Data = data.Data.Select(s => new MessageInboxDto
            {
                Id = s.Id,
                Message = s.Message,
                MemberId = s.MemberId,
                TypeId = s.TypeId,
                IsRead = s.IsRead,
                CreatedOn = s.CreatedOn,
                Title = s.Title

            }).ToList();

            result.Count = data.TotalCount;   
        }

        return result;
    }
}
