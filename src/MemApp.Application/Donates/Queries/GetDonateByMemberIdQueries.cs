using MediatR;
using MemApp.Application.Donates.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Donates.Queries;
public class GetDonateByMemberIdQueries : IRequest<ListResult<DonateDto>>
{
    public int MemberId { get; set; }
}


public class GetDonateByMemberIdQueriesHandler : IRequestHandler<GetDonateByMemberIdQueries, ListResult<DonateDto>>
{
    private readonly IMemDbContext _context;
    public GetDonateByMemberIdQueriesHandler(IMemDbContext context)
    {
        _context = context;
    }

    public async Task<ListResult<DonateDto>> Handle(GetDonateByMemberIdQueries request, CancellationToken cancellationToken)
    {
        var result = new ListResult<DonateDto>();
        var data = await _context.Donates
            .Include(i=>i.RegisterMember)
            .Include(i=>i.Donation)
            .Where(q=>q.MemberId== request.MemberId)
            .AsNoTracking()
            .ToListAsync( cancellationToken);

        if (data == null)
        {
            result.HasError = true;
            result.Messages.Add("Data Not Found");
        }
        else
        {
            result.HasError = false;

            result.Data = data.Select(s => new DonateDto
            {
                Id = s.Id,
                Amount = s.Amount,
                DonateDate = s.DonateDate,
                DonateNo = s.DonateNo,
                Note = s.Note,
                MemberId = s.MemberId,
                MemberName = s.RegisterMember?.FullName,
                MemberShipNo = s.RegisterMember?.MembershipNo,
                DonationText = s.Donation.Title,
                DonationId = s.DonationId,
            }).ToList();

        }

        return result;
    }
}
