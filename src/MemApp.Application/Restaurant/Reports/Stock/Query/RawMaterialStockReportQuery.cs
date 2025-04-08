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

namespace MemApp.Application.Restaurant.Reports.Stock.Query
{

    public class RawMaterialStockReportQuery : IRequest<List<RawMaterialStockReportVM>>
    {
        public RawMaterialStockReportCriteria Model { get; set; } = new RawMaterialStockReportCriteria();
    }

    public class RawMaterialStockReportQueryHandler : IRequestHandler<RawMaterialStockReportQuery, List<RawMaterialStockReportVM>>
    {
        private readonly IDapperContext _context;

        public RawMaterialStockReportQueryHandler(IDapperContext context)
        {
            _context = context;
        }
        public async Task<List<RawMaterialStockReportVM>> Handle(RawMaterialStockReportQuery request, CancellationToken cancellationToken)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@RawmaterialGroup", request.Model.RawmaterialGroup, DbType.String);
                    parameters.Add("@RawMaterial", request.Model.RawMaterial, DbType.String);
                    parameters.Add("@Department", request.Model.Department, DbType.String);

                    string storedProcedure = request.Model.isWithNegative
                        ? "sp_getStockReportOnlyZero"
                        : "sp_getStockReport";

                    if (!request.Model.isWithNegative)
                    {
                        parameters.Add("@WithZero", request.Model.WithZero, DbType.Boolean);
                    }

                    var dataList = (await connection.QueryAsync<RawMaterialStockReportVM>(
                        storedProcedure,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    )).ToList();

                    if (request.Model.isSummary)
                    {
                        return dataList
                            .GroupBy(k => k.Name)
                            .Select(group => new RawMaterialStockReportVM
                            {
                                Name = group.Key,
                                RawMeterialId = group.Max(x => x.RawMeterialId),
                                CPU = group.Max(x => x.CPU),
                                BAL_QTY = group.Sum(x => x.BAL_QTY),
                                UnitName = group.Max(x => x.UnitName),
                                BalanceValue = group.Sum(x => x.BalanceValue)
                            })
                            .ToList();
                    }

                    return dataList;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error fetching stock report: {ex.Message}", ex);
                }
            }
        }

    }
}
