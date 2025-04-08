using Dapper;
using MediatR;
using MemApp.Application.Restaurant.Reports.Stock.Model;
using MemApp.Application.Services;
using System.Data;

namespace MemApp.Application.Restaurant.Reports.Stock.Query
{

    public class RawMaterialConsumptionReportQuery : IRequest<List<RawMaterialConsumptionReportVM>>
    {
        public RawMaterialConsumptionReportCriteria Model { get; set; } = new RawMaterialConsumptionReportCriteria();
    }

    public class RawMaterialConsumptionReportQueryHandler : IRequestHandler<RawMaterialConsumptionReportQuery, List<RawMaterialConsumptionReportVM>>
    {
        private readonly IDapperContext _context;

        public RawMaterialConsumptionReportQueryHandler(IDapperContext context)
        {
            _context = context;
        }
        public async Task<List<RawMaterialConsumptionReportVM>> Handle(RawMaterialConsumptionReportQuery request, CancellationToken cancellationToken)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@RawMaterialGroupId", request.Model.RawmaterialGroup, DbType.String);
                    parameters.Add("@RawMaterialId", request.Model.RawMaterial, DbType.String);
                    parameters.Add("@sDT", request.Model.FromDate.Value.ToShortDateString(), DbType.String);
                    parameters.Add("@eDT", request.Model.ToDate.Value.ToShortDateString(), DbType.String);
                    parameters.Add("@type", "", DbType.String);
                    parameters.Add("@Department", request.Model.Department, DbType.String);

                    string storedProcedure = "GetRawMaterialConsumtion";

                    var dataList = (await connection.QueryAsync<RawMaterialConsumptionReportVM>(
                        storedProcedure,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    )).ToList();

                    if (request.Model.isSummary)
                    {
                        return dataList
                                      .GroupBy(row => new
                                      {
                                          row.SaleItemName,
                                          row.RawMaterialId,
                                          row.RawMaterialName,
                                          row.UnitName,
                                          row.SaleItemPrice
                                      })
                                      .Select(group => new RawMaterialConsumptionReportVM
                                      {
                                          SaleItemName = group.Key.SaleItemName,
                                          RawMaterialId = group.Key.RawMaterialId,
                                          RawMaterialName = group.Key.RawMaterialName,
                                          UnitName = group.Key.UnitName,
                                          SaleQty = group.Sum(row => row.SaleQty),
                                          SaleItemPrice = group.Key.SaleItemPrice,
                                          RawMaterialCPU = group.Sum(row => row.RawMaterialCPU), 
                                          RawMaterialSQty = group.Sum(row => row.RawMaterialSQty)
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
