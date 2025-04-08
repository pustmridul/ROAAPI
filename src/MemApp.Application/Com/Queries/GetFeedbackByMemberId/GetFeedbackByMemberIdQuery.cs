using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.com.Queries.GetAllEmergencyInfo
{
    public class GetFeedbackByMemberIdQuery : IRequest<ListResult<FeedbackReq>>
    {
        public int MemberId { get; set; }
    }

    public class GetFeedbackByMemberIdQueryHandler : IRequestHandler<GetFeedbackByMemberIdQuery, ListResult<FeedbackReq>>
    {
        private readonly IMemDbContext _context;
        public GetFeedbackByMemberIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<FeedbackReq>> Handle(GetFeedbackByMemberIdQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<FeedbackReq>();
            try
            {
                var data = await _context.Feedbacks
                    .Include(i=>i.Member)
                    .Include(i => i.Replies)
                    .Where(q=>q.MemberId== request.MemberId)
                    .OrderByDescending(o=>o.Id)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                if (data == null)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.Count = data.Count;
                    result.Data= data.Select(s=> new FeedbackReq
                    {
                        Id = s.Id,                     
                        Status= s.Status,
                        FeedbackDate=s.FeedbackDate,
                        MemberId = s.MemberId,
                        MemberName=s.Member.FullName,
                        MemberShipNo=s.Member.MembershipNo,
                        Message= s.Message,
                        ReplyReqs= s.Replies?.Select(s1 => new ReplyReq
                        {
                            UserId = s1.UserId,
                            UserName = s1.UserName,
                            Message = s1.Message,
                            ReplyDate = s1.ReplyDate,

                        }).ToList()
                    }).ToList();
                   
                }

            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add(ex.Message);

            }

            return result;
        }
    }
}
