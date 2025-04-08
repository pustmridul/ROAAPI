using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Colleges.Models;
using MemApp.Application.Mem.Members.Models;
using MemApp.Application.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Dapper;
using MemApp.Application.Services;

namespace MemApp.Application.Mem.Colleges.Queries
{
    public record ExportMemberListSelectedColumnQuery : IRequest<ExportVm>
    {
        public string QueryString { get; set; }
    }

    public class ExportMemberListSelectedColumnQueryHandler : IRequestHandler<ExportMemberListSelectedColumnQuery, ExportVm>
    {
        private readonly IDapperContext _context;
        private readonly ICsvFileBuilder _fileBuilder;

        public ExportMemberListSelectedColumnQueryHandler(IDapperContext context, ICsvFileBuilder fileBuilder)
        {
            _context = context;
            _fileBuilder = fileBuilder;
        }

        public async Task<ExportVm> Handle(ExportMemberListSelectedColumnQuery request, CancellationToken cancellationToken)
        {
            var result = new List<ViewMemberDto>();

            using (var connection = _context.CreateConnection())
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("SELECT " + request.QueryString + " FROM [dbo].[View_Member]");

                var data = await connection.QueryAsync<dynamic>(sb.ToString());


                

                var vm = new ExportVm(
                "college.csv",
                "text/csv",
                _fileBuilder.BuildCollegesFile(data));

                return vm;

            }

            

           
        }
        public T CreateGenericObject<T>(T value)
        {
            return value;
        }


    }

}
