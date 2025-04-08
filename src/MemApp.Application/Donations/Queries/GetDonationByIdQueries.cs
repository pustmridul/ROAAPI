using MediatR;
using MemApp.Application.Donations.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Donations.Queries;
public class GetDonationByIdQueries : IRequest<Result<DonationDto>>
{
    public int Id { get; set; }
}


public class GetDonationByIdQueriesHandler : IRequestHandler<GetDonationByIdQueries, Result<DonationDto>>
{
    private readonly IMemDbContext _context;
    public GetDonationByIdQueriesHandler(IMemDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DonationDto>> Handle(GetDonationByIdQueries request, CancellationToken cancellationToken)
    {
        var result = new Result<DonationDto>();
        var data = await _context.Donations
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
            result.Data.Title = data.Title;
            result.Data.Description = data.Description;
            result.Data.StartDate = data.StartDate;
            result.Data.EndDate = data.EndDate;
            result.Data.IsActive = data.IsActive;
        }

        return result;
    }
}
