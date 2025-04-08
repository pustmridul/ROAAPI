using MediatR;
using MemApp.Application.Donations.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Donations.Queries;


public class GetAllDonationQueries : IRequest<ListResult<DonationDto>>
{
    public int PageSize { get; set; }
    public int PageNo { get; set; }
    public string? SearchText { get; set; }
}


public class GetAllDonationQueriesHandler : IRequestHandler<GetAllDonationQueries, ListResult<DonationDto>>
{
    private readonly IMemDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetAllDonationQueriesHandler(
        IMemDbContext context,
        IHttpContextAccessor httpContextAccessor
        )
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ListResult<DonationDto>> Handle(GetAllDonationQueries request, CancellationToken cancellationToken)
    {
        string baseUrl = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host + _httpContextAccessor.HttpContext.Request.PathBase+"/";

        var result = new ListResult<DonationDto>();
        var data = await _context.Donations
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
            result.Count = Convert.ToInt32( data.TotalCount);
            result.Data = data.Data.Select(s => new DonationDto
            {
                Id = s.Id,
                Description = s.Description,
                Title = s.Title,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                IsActive = s.IsActive,
                IsFixed = s.IsFixed,
                FileUrl = s.FileUrl,
                Amount = s.Amount,
            }).ToList();
        }

        return result;
    }
}
