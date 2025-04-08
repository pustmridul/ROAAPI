using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.com.Queries.GetAllEmergencyInfo
{
    public class GetFeedbackIdQuery : IRequest<Result<FeedbackReq>>
    {
        public int Id { get; set; }
    }

    public class GetFeedbackIdQueryHandler : IRequestHandler<GetFeedbackIdQuery, Result<FeedbackReq>>
    {
        private readonly IMemDbContext _context;
        public GetFeedbackIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<Result<FeedbackReq>> Handle(GetFeedbackIdQuery request, CancellationToken cancellationToken)
        {
            var result = new Result<FeedbackReq>();
            try
            {
                var data = await _context.Feedbacks
                    .Include(i => i.Replies)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(q => q.Id == request.Id, cancellationToken);

                if (data == null)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.Data.Id = data.Id;
                    result.Data.MemberShipNo = data.MemberShipNo;
                    result.Data.MemberId = data.MemberId;
                    result.Data.FeedbackDate = data.FeedbackDate;
                    result.Data.Message = data.Message;
                    result.Data.Status = data.Status;
                    result.Data.ReplyReqs = data?.Replies?.Select(s => new ReplyReq
                    {
                        UserId = s.UserId,
                        UserName = s.UserName,
                        Message = s.Message,
                        ReplyDate = s.ReplyDate,

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
