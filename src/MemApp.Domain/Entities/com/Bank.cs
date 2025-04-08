using MemApp.Domain.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemApp.Domain.Entities.com
{
    public class Bank 
    {
        public decimal slno { get; set; }
        public string? BANKNAME { get; set; }
    }
}
