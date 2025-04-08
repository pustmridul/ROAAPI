using Dapper;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Dashboard.Models;
using MemApp.Application.Mem.Dashboards.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace MemApp.Application.Services
{
    public class DasboardService : IDashboardService
    {
        private readonly IMemDbContext _context;
        private readonly IDapperContext _dapperContext;
        public DasboardService(IMemDbContext context, IDapperContext dapperContext)
        {
            _context = context;
            _dapperContext = dapperContext;
        }

        public async Task<UserConferenceVmList> GetUserConferenceData(int PageNo, int PageSize)
        {
            var data = new UserConferenceVmList();
            var obj = await _context.UserConferences.Select(i => new UserConferenceResp
            {
                AppId = i.AppId ?? "",
                Id = i.Id,
                IpAddress = i.IpAddress ?? "",
                LogInDate = i.LogInDate,
                LogOutDate = i.LogOutDate,
                LogOutStatus = i.LogOutStatus,
                UserName = i.UserName,
                UserId = i.UserId

            }).ToPaginatedListAsync(PageNo, PageSize, new CancellationToken());

            data.DataList = obj.Data;
            data.DataCount = Convert.ToInt32(obj.TotalCount);
            return data;
        }

        public async Task<DashboardListVm> GetDashboardData()
        {
            var data = new DashboardListVm();
            var UpcomingEventList = await _context.ServiceTickets.Where(i => i.EventDate > DateTime.Now && i.IsActive && i.MemServiceTypeId == 6)
                .Select(s => new { s.Id, s.Title }).ToListAsync();

            using (var connection = _dapperContext.CreateConnection())
            {

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("select * from VW_DashboardGrid");


                var dashBoardGridData = await connection
                .QueryAsync<DashboardListVm>(sb.ToString());

                data.TotalMember = dashBoardGridData.FirstOrDefault().TotalMember;
                data.TotalPendingMember = dashBoardGridData.FirstOrDefault().TotalPendingMember;
                data.SubscriptionDueAmount = dashBoardGridData.FirstOrDefault().SubscriptionDueAmount;
            }


            data.UpcomingEvents = UpcomingEventList.Count;
            //  data.ConferenceList = userConference;

            return data;
        }



        public async Task<List<EventTicketSaleDVm>> GetEventTicketSaleInfo()
        {
            try
            {
                using (var connection = _dapperContext.CreateConnection())
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("WITH EventSaleCTE AS ( SELECT COUNT(sed.SaleEventId) AS SaleQty,SUM(sed.TicketPrice) AS TotalAmount," +
                        "sed.TicketPrice,sed.TicketCriteria,sed.EventId,sed.EventTitle,st.EventDate");
                    sb.AppendLine("FROM mem_SaleEventTicketDetail sed");
                    sb.AppendLine("JOIN mem_SaleEventTicket se ON se.Id = sed.SaleEventId");
                    sb.AppendLine("JOIN mem_ServiceTicket st on st.Id = sed.EventId");
                    sb.AppendLine("WHERE se.IsActive = 1 AND sed.IsActive = 1 AND st.IsActive=1 AND (sed.SaleStatus !='Cancel' OR sed.SaleStatus is null)");
                    sb.AppendLine("GROUP BY sed.EventId, sed.EventTitle, sed.TicketCriteria, st.EventDate,sed.TicketPrice)");
                    sb.AppendLine("SELECT *   FROM EventSaleCTE WHERE EventSaleCTE.EventDate >= GETDATE()");
                    sb.AppendLine("ORDER BY EventSaleCTE.EventDate desc");

                    var dataQuery = await connection
                        .QueryAsync<EventTicketSaleDVm>(sb.ToString());

                    var data = dataQuery.ToList();
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<IList<YearlyIncomeVm>> GetYearlyIncomeData()
        {
            try
            {
                using (var connection = _dapperContext.CreateConnection())
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("SELECT   -1*  SUM(Amount) AS Amount,     MONTH(Dates) AS MonthNo,    DATENAME(month, Dates) AS MonthText ");
                    sb.AppendLine("FROM     CustomerLedger");
                    sb.AppendLine("WHERE     YEAR(Dates) = YEAR(GETDATE()) and (TOPUPID is null or TOPUPID='')");
                    sb.AppendLine(" GROUP BY     MONTH(Dates), DATENAME(month, Dates) ORDER BY    MONTH(Dates);");

                    var dataQuery = await connection.QueryAsync<YearlyIncomeVm>(sb.ToString());

                    var data = dataQuery.ToList();
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
