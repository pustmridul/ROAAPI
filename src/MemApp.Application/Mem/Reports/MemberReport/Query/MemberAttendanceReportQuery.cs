using Dapper;
using MediatR;
using MemApp.Application.App.Models;
using MemApp.Application.Mem.Reports.Common;
using MemApp.Application.Mem.Reports.MemberReport.Model;
using MemApp.Application.Services;
using System.Data;


namespace MemApp.Application.Mem.Reports.MemberReport.Query
{
    public class MemberAttendanceReportQuery:IRequest<List<MemberAttendanceReportVM>>
    {
        public CommonCriteria Model { get; set; }= new CommonCriteria();
    }

    public class MemberAttendanceReportQueryHandler : IRequestHandler<MemberAttendanceReportQuery, List<MemberAttendanceReportVM>>
    {
        private readonly IDapperContext _dapperContext;
        public MemberAttendanceReportQueryHandler( IDapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<List<MemberAttendanceReportVM>> Handle(MemberAttendanceReportQuery request, CancellationToken cancellationToken)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                try
                {  
                    if(request.Model.MembershipNo== null)
                    {
                        request.Model.MembershipNo = "null";
                    }
                    var parameters = new DynamicParameters();
                    parameters.Add("@MembershipNo", request.Model.MembershipNo == "null" ? null : request.Model.MembershipNo, DbType.String, size: 10);
                    parameters.Add("@FromDate", request.Model.FromDate?.Date, DbType.Date);
                    parameters.Add("@ToDate", request.Model.ToDate?.Date, DbType.Date);

                    var dataList = await connection.QueryAsync<MemberAttendanceReportVM>(
                        "SP_MemberAttendance",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );                   
                    var data = dataList.ToList();
                    return data;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
        }
    }
}
