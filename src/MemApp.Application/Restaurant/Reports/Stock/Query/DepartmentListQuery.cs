using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Reports.Common;
using MemApp.Application.Mem.Reports.MemberReport.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MemApp.Application.Services;
using MemApp.Application.Restaurant.Reports.Stock.Model;
using Microsoft.EntityFrameworkCore;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.Restaurant;

namespace MemApp.Application.Restaurant.Reports.Stock.Query
{

    public class DepartmentListQuery : IRequest<DepartmentListVM>
    {
        //public RawMaterialStockReportCriteria Model { get; set; } = new RawMaterialStockReportCriteria();
    }

    public class DepartmentListQueryHandler : IRequestHandler<DepartmentListQuery, DepartmentListVM>
    {
        private readonly IMemDbContext _context;

        public DepartmentListQueryHandler(IMemDbContext context)
        {
            _context = context;
        }
        public async Task<DepartmentListVM> Handle(DepartmentListQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var result = new DepartmentListVM();

                var data = await _context.Departments.ToListAsync(cancellationToken);


                if (data.Count == 0)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.DataCount = data.Count;
                    result.DataList = data.Select(s => new Department
                    {
                        ID = s.ID,
                        DepartmentName = s.DepartmentName,
                        CreatedBy = s.CreatedBy,
                        UpdatedBy = s.UpdatedBy,
                        CreatedDate = s.CreatedDate,
                        UpdatedDate = s.UpdatedDate,
                        PrinterName = s.PrinterName
                    }).ToList();

                    result.DataList.Insert(0, new Department { ID = -1, DepartmentName = "All" });
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching stock report: {ex.Message}", ex);
            }

        }

    }
}
