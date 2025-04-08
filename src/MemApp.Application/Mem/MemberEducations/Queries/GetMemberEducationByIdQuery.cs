using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemberEducations.Models;
using MemApp.Domain.Entities.mem;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.MemberEducations.Queries
{
    public class GetMemberEducationByIdQuery : IRequest<MemberEducationeVm>
    {
        public int Id { get; set; }
    }


    public class GetMemberEducationByIdQueryHandler : IRequestHandler<GetMemberEducationByIdQuery, MemberEducationeVm>
    {
        private readonly IMemDbContext _context;
        public GetMemberEducationByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<MemberEducationeVm> Handle(GetMemberEducationByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new MemberEducationeVm();
            var data = await _context.MemberEducations.FirstOrDefaultAsync(q=>q.Id==request.Id, cancellationToken);
            if (data==null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;

                result.Data = new MemberEducationeRes
                {
                    Id = data.Id,
                    PrvCusID = data.MemberId.ToString(),
                    Board = data.Board,
                    Exam = data.Exam,
                    Grade = data.Grade,
                    Institution = data.Institution,
                    PassingYear = data.PassingYear
                };
            }

            return result;
        }
    }
}
