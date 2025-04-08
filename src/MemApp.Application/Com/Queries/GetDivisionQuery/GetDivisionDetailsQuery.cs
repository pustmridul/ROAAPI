using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;
using ResApp.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.MemberTypes.Queries
{
    public class GetDivisionDetailsQuery : IRequest<Result<DivisionDto>>
    {
        public int DivisionId {  get; set; }
    }

    public class GetDivisionDetailsQueryHandler : IRequestHandler<GetDivisionDetailsQuery, Result<DivisionDto>>
    {
        private readonly IMemDbContext _context;
        public GetDivisionDetailsQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<Result<DivisionDto>> Handle(GetDivisionDetailsQuery request, CancellationToken cancellationToken)
        {
            var result = new Result<DivisionDto>();
            var data = await _context.Divisions.Where(s=>s.Id==request.DivisionId).FirstOrDefaultAsync(cancellationToken);

            if (data == null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;
               // result.Count= data.Count;
                result.Data =new DivisionDto
                {
                    Id = data.Id,
                    EnglishName = data.EnglishName,
                    BanglaName = data.BanglaName,
                };
            }

            return result;
        }
    }
}
