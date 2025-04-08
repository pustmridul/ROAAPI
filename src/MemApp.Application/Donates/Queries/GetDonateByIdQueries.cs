using MediatR;
using MemApp.Application.Donates.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Donates.Queries;
public class GetDonateByIdQueries : IRequest<Result<DonateDto>>
{
    public int Id { get; set; }
}


public class GetDonateByIdQueriesHandler : IRequestHandler<GetDonateByIdQueries, Result<DonateDto>>
{
    private readonly IMemDbContext _context;
    public GetDonateByIdQueriesHandler(IMemDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DonateDto>> Handle(GetDonateByIdQueries request, CancellationToken cancellationToken)
    {
        var result = new Result<DonateDto>();
        var data = await _context.Donates
            .Include(i=>i.RegisterMember)
            .Include(i=>i.Donation)
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
            result.Data.MemberId = data.MemberId;
            result.Data.MemberName = data.RegisterMember?.FullName;
            result.Data.MemberShipNo = data.RegisterMember?.MembershipNo;
            result.Data.DonationText = data.Donation.Title;
            result.Data.DonationId = data.DonationId;
            result.Data.Amount = data.Amount;
            result.Data.DonateDate = data.DonateDate;
            result.Data.DonateNo = data.DonateNo;
            result.Data.Note = data.Note;

        }

        return result;
    }
}
