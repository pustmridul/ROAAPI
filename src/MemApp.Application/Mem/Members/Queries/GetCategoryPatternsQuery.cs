using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Members.Queries
{
    public class GetCategoryPatternsQuery:IRequest<ListResult<CategoryPatternsDto>>
    {

    }
    public class GetAllCategoryPatternsQueryHandler : IRequestHandler<GetCategoryPatternsQuery, ListResult<CategoryPatternsDto>>
    {
        private readonly IMemDbContext _memdbcontext;

        public GetAllCategoryPatternsQueryHandler(IMemDbContext memDbContext)
        {
            _memdbcontext = memDbContext;
        }

        public async Task<ListResult<CategoryPatternsDto>> Handle(GetCategoryPatternsQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<CategoryPatternsDto>();
            var data = await _memdbcontext.CategoryPatterns.ToListAsync(cancellationToken);
            if (data.Count == 0)
            {
                result.HasError= true;
                result.Messages.Add("Data Not Found!");
            }
            else
            {
                result.HasError= false;
                result.Count = data.Count;
                result.Data = data.Select(x => new CategoryPatternsDto { Id =x.Id , Title = x.Title }).ToList();
                result.Messages.Add("Data Retrieve Successfully.");
            }
            return result;
        }

    }
}
