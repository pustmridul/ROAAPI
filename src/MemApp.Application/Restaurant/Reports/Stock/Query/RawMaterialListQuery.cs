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

    public class RawMaterialListQuery : IRequest<RawMaterialListVM>
    {
        //public RawMaterialStockReportCriteria Model { get; set; } = new RawMaterialStockReportCriteria();
    }

    public class RawMaterialListQueryHandler : IRequestHandler<RawMaterialListQuery, RawMaterialListVM>
    {
        private readonly IMemDbContext _context;

        public RawMaterialListQueryHandler(IMemDbContext context)
        {
            _context = context;
        }
        public async Task<RawMaterialListVM> Handle(RawMaterialListQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var result = new RawMaterialListVM();

                var data = await _context.RawMeterials.ToListAsync(cancellationToken);


                if (data.Count == 0)
                {
                    result.HasError = true;
                    result.Messages?.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.DataCount = data.Count;
                    result.DataList = data.Select(s => new RawMeterial
                    {
                        ID = s.ID,
                        Name = s.Name,
                        GroupID = s.GroupID,
                        UnitTypeID = s.UnitTypeID,
                        CPU = s.CPU,
                        ROL = s.ROL,
                        ReceipeCPU = s.ReceipeCPU,
                        ReceipeUnitTypeId = s.ReceipeUnitTypeId,
                        ConversionRate = s.ConversionRate,
                        IsNonInventoryItem = s.IsNonInventoryItem,
                        IsNonExpireItem = s.IsNonExpireItem,
                        IsNonReceipyItem = s.IsNonReceipyItem,
                        CreatedDate = s.CreatedDate,
                        CreateBy = s.CreateBy,
                        UpdateDate = s.UpdateDate,
                        UpdateBy = s.UpdateBy
                    }).ToList();

                    //result.DataList.Insert(0, new RawMeterial { ID = -1, Name = "All" });
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
