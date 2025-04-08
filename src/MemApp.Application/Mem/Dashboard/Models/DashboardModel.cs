using MemApp.Application.Extensions;
using MemApp.Application.Mem.Dashboard.Models;
using MemApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Dashboards.Models
{
    public class DashboardModel
    {
    }
    public class DashboardReq
    {
        public string Name { get; set; }
        public string? Code { get; set; }

        public int Id { get; set; }
    }
    public class DashboardRes
    {
        public string Name { get; set; }
        public string? Code { get; set; }

        public int Id { get; set; }
    }
    public class DashboardVm: Result
    {
       public DashboardRes Data { get; set; } = new DashboardRes();
    }

    public class DashboardListVm : Result
    {
        public long DataCount { get; set; }
        public List<DashboardRes> DataList { get; set; }= new List<DashboardRes>();
       // public List<UserConferenceResp> ConferenceList { get; set; } = new List<UserConferenceResp>();
        public long TotalMember { get; set; }
        public long TotalPendingMember { get; set; }
        public long UpcomingEvents { get; set; }
        public long SubscriptionDueAmount { get; set; }
    }

    public class EventTicketSaleDVm
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; }
        public DateTime EventDate { get; set; }
        public int SaleQty { get; set; }
        public string TicketCriteria { get; set; }
        public decimal TicketPrice { get; set; }
        public decimal TotalAmount { get; set; }

    }
    public class YearlyIncomeVm
    {
        public string MonthText { get; set; }= string.Empty;
        public int MonthNo { get; set; }
        public decimal Amount { get; set; }
    }


    public class TransactionTypeVm
    {
        public string TransactionType { get; set; }= string.Empty ;
        public decimal Amount { get; set; }
    }

}
