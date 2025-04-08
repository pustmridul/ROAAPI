using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.MessageInboxs.Models;
using MemApp.Application.MessageInboxs.Queries.GetMessageByMemberId;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.MessageInboxs.Queries
{
    public class GetTotalReadUnReadMessagesQueries : IRequest<TotalReadUnReadMessagesDto>
    {
        public bool IsRead { get; set; }
        public int MemberId { get; set; }
    }
    public class GetTotalReadUnReadMessagesQueriesHandler : IRequestHandler<GetTotalReadUnReadMessagesQueries, TotalReadUnReadMessagesDto>
    {
        private readonly IMemDbContext _context;
        public GetTotalReadUnReadMessagesQueriesHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<TotalReadUnReadMessagesDto> Handle(GetTotalReadUnReadMessagesQueries request, CancellationToken cancellationToken)
        {
            var result = new TotalReadUnReadMessagesDto();
            var query = _context.MessageInboxs
                        .Where(q => q.MemberId == request.MemberId && q.IsRead == request.IsRead)
                        .AsNoTracking();
                        

            result.HasError = false;
            result.Succeeded = true;
            result.TotalMessageCount = query.Count();

            return result;
        }
    }
}
