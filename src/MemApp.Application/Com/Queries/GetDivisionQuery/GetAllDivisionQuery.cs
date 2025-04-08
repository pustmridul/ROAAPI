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
    public class GetAllDivisionQuery : IRequest<ListResult<DivisionDto>>
    {

    }

    public class GetAllDivisionQueryHandler : IRequestHandler<GetAllDivisionQuery, ListResult<DivisionDto>>
    {
        private readonly IMemDbContext _context;
        public GetAllDivisionQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<ListResult<DivisionDto>> Handle(GetAllDivisionQuery request, CancellationToken cancellationToken)
        {
            var result = new ListResult<DivisionDto>();

            try
            {
                var data = await _context.Divisions.ToListAsync(cancellationToken);

                if (data.Count == 0)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.Count = data.Count;
                    result.Data = data.Select(s => new DivisionDto
                    {
                        Id = s.Id,
                        EnglishName = s.EnglishName,
                        BanglaName = s.BanglaName,
                    }).ToList();
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
