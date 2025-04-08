using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Colleges.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Colleges.Queries
{
    public class GetCollegeByIdQuery : IRequest<Result<CollegeDto>>
    {
        public int Id { get; set; }
    }


    public class GetCollegeByIdQueryHandler : IRequestHandler<GetCollegeByIdQuery, Result<CollegeDto>>
    {
        private readonly IMemDbContext _context;
        public GetCollegeByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CollegeDto>> Handle(GetCollegeByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new Result<CollegeDto>();
            var data = await _context.Colleges.FirstOrDefaultAsync(q=>q.Id==request.Id, cancellationToken);
            if (data==null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;

                result.Data = new CollegeDto
                {
                    Id = data.Id,
                    Name = data.Name,
                    Code= data.Code,
                    OldId= data.OldId,
                };
            }

            return result;
        }
    }
}
