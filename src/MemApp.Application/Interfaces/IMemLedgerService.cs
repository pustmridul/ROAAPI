using MemApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Interfaces
{
    public interface IMemLedgerService
    {
        Task<bool> CreateMemLedger(MemLedgerVm model);
        Task<bool> CreateBulkMemLedger(List<MemLedgerVm> models);

        Task<decimal> GetCurrentBalance(string memberId);

        Task<bool> AutoSubscriptionPayment(string SubscriptionYear,string QuaterNo);

    }
}
