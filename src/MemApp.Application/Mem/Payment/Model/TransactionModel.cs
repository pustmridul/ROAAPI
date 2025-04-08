
using MemApp.Application.Extensions;
using MemApp.Application.Models;

namespace MemApp.Application.Mem.Payment.Model
{
    public class TransactionModel
    {
    }

    public class YearlyExpense
    {
        public decimal ExpenseAmount { get; set; }
    }
    public class YearlyExpenseList : Result
    {
        public List<decimal> YearlyExpenses { get; set; } = new List<decimal>();
    }
}
