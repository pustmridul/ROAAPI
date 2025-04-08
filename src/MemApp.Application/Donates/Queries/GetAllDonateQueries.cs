using MediatR;
using MemApp.Application.Donates.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Donates.Queries;

public class GetAllDonateQueries : IRequest<ListResult<DonateDto>>
{
    public int PageSize { get; set; }
    public int PageNo { get; set; }
    public string? SearchText { get; set; }
}


public class GetAllDonateQueriesHandler : IRequestHandler<GetAllDonateQueries, ListResult<DonateDto>>
{
    private readonly IMemDbContext _context;
    public GetAllDonateQueriesHandler(IMemDbContext context)
    {
        _context = context;
    }

    public async Task<ListResult<DonateDto>> Handle(GetAllDonateQueries request, CancellationToken cancellationToken)
    {
        var result = new ListResult<DonateDto>();
        var data = await _context.Donates
            .Include(i=>i.RegisterMember)
            .Include(i=>i.Donation)
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
            result.Count= Convert.ToInt32(data.TotalCount);
            result.Data = data.Data.Select(s => new DonateDto
            {
                Id = s.Id,              
                Amount = s.Amount,
                DonateDate = s.DonateDate,
                DonateNo = s.DonateNo,
                Note = s.Note,
                MemberId = s.MemberId,
                MemberName=s.RegisterMember?.FullName,
                MemberShipNo=s.RegisterMember?.MembershipNo,
                DonationText= s.Donation.Title,
                DonationId = s.DonationId,
            }).ToList();
        }

        return result;
    }
}