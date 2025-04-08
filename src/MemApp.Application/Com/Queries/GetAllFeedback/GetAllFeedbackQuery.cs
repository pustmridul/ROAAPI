using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Domain.Entities.com;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.com.Queries.GetAllFeedback
{
    public class GetAllFeedbackQuery : IRequest<ListResult<FeedbackReq>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int FeedbackCategoryId { get; set; }

    }

    public class GetAllFeedbackQueryHandler : IRequestHandler<GetAllFeedbackQuery, ListResult<FeedbackReq>>
    {
        private readonly IMemDbContext _context;
        public GetAllFeedbackQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<FeedbackReq>> Handle(GetAllFeedbackQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<FeedbackReq>();
            try
            {
                var data = await _context.Feedbacks
                    .Include(c => c.Member)
                    .Include(c => c.Replies)
                    .Include(c => c.FeedbackCategory)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                if (request.FeedbackCategoryId != 0)
                {
                    data = data.Where(c => c.FeedbackCategoryId == request.FeedbackCategoryId).ToList();
                }

                if (data.Count == 0)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.Count = data.Count;
                    result.Data = data.Select(s => new FeedbackReq
                    {
                        Id = s.Id,
                        Message = s.Message,
                        FeedbackDate = s.FeedbackDate,
                        MemberId = s.MemberId,
                        MemberShipNo = s.MemberShipNo,
                        MemberName = s.Member.FullName,
                        Status = s.Status,
                        FeedbackCategoryName = s.FeedbackCategory?.Name,
                        ReplyReqs = s.Replies?.Select(s1 => new ReplyReq
                        {
                            Id = s1.Id,
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
