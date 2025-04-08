using MediatR;
using MemApp.Application.com.Queries.GetAllFeedback;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Com.Queries
{
    public class GetAllFeedbackCategoryQuery : IRequest<ListResult<FeedbackCategoryReq>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class GetAllFeedbackCategoryQueryHandler : IRequestHandler<GetAllFeedbackCategoryQuery, ListResult<FeedbackCategoryReq>>
    {
        private readonly IMemDbContext _context;
        public GetAllFeedbackCategoryQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<FeedbackCategoryReq>> Handle(GetAllFeedbackCategoryQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<FeedbackCategoryReq>();
            try
            {
                var data = await _context.FeedbackCategories
                    .Include(x => x.RegisterMember)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                if (data.Count == 0)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.Count = data.Count;
                    result.Data = data.Select(s => new FeedbackCategoryReq
                    {
                        Id = s.Id,
                        Name = s.Name, 
                        TaggedDirectorId = s.TaggedDirectorId,
                        TaggedDirectorName = s.RegisterMember.FullName

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
