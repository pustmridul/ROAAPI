using MemApp.Domain.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemApp.Domain.Entities.com
{
    public class CreditCard
    {
        public decimal slno { get; set; }
        public string? CardName { get; set; }
        public decimal? BankCommission { get; set; }
    }
}
