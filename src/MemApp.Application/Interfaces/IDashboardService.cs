using MemApp.Application.Mem.Dashboard.Models;
using MemApp.Application.Mem.Dashboards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardListVm> GetDashboardData();
        Task<UserConferenceVmList> GetUserConferenceData(int PageNo, int PageSize);
        Task<List<EventTicketSaleDVm>> GetEventTicketSaleInfo();
        Task<IList<YearlyIncomeVm>> GetYearlyIncomeData();


    }
}
