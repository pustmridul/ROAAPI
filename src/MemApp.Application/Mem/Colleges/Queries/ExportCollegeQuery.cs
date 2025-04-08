using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Colleges.Models;
using MemApp.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Colleges.Queries
{
    public record ExportCollegesQuery : IRequest<ExportVm>
    {
    }

    public class ExportCollegesQueryHandler : IRequestHandler<ExportCollegesQuery, ExportVm>
    {
        private readonly IMemDbContext _context;
        private readonly ICsvFileBuilder _fileBuilder;

        public ExportCollegesQueryHandler(IMemDbContext context, ICsvFileBuilder fileBuilder)
        {
            _context = context;
            _fileBuilder = fileBuilder;
        }

        public async Task<ExportVm> Handle(ExportCollegesQuery request, CancellationToken cancellationToken)
        {
            var records = await _context.Colleges
                    .Where(t => t.IsActive).Select(s=>new CollegeDto
                    {
                        Code=s.Code,
                        Name=s.Name,
                        Id=s.Id,
                    })
                    .ToListAsync(cancellationToken);

            var vm = new ExportVm(
                "college.csv",
                "text/csv",
                _fileBuilder.BuildCollegesFile(records));

            return vm;
        }
    }

}
