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

    public class RawMaterialGroupListQuery : IRequest<RawMaterialGroupListVM>
    {
        //public RawMaterialStockReportCriteria Model { get; set; } = new RawMaterialStockReportCriteria();
    }

    public class RawMaterialGroupListQueryHandler : IRequestHandler<RawMaterialGroupListQuery, RawMaterialGroupListVM>
    {
        private readonly IMemDbContext _context;

        public RawMaterialGroupListQueryHandler(IMemDbContext context)
        {
            _context = context;
        }
        public async Task<RawMaterialGroupListVM> Handle(RawMaterialGroupListQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var result = new RawMaterialGroupListVM();

                var data = await _context.RawMeterialGroups.ToListAsync(cancellationToken);


                if (data.Count == 0)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.DataCount = data.Count;
                    result.DataList = data.Select(s => new RawMeterialGroup
                    {
                        ID = s.ID,
                        GroupName = s.GroupName,
                        GroupID = s.GroupID
                    }).ToList();

                    result.DataList.Insert(0, new RawMeterialGroup { GroupID = "-1", GroupName = "All" });
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
