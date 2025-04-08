using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Infrastructure.Services
{
    public interface ISubscription
    {
        Task<bool> MonthlyDueGenerator(DateTime syncDate);
    }
}
