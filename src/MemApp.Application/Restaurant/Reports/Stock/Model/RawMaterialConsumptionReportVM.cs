using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Restaurant.Reports.Stock.Model
{
    public class RawMaterialConsumptionReportVM
    {
        public string? Invoice { get; set; }
        public DateTime SaleDT { get; set; }
        public int RawMaterialId { get; set; }
        public string? RawMaterialName { get; set; }
        public string? GroupName { get; set; }
        public string? UnitName { get; set; }
        public string? SaleItemGroup { get; set; }
        public string? SaleItemName { get; set; }
        public string? SaleItemPrice { get; set; }
        public decimal RawMaterialSQty { get; set; }
        public decimal RawMaterialRQty { get; set; }
        public decimal RawMaterialCPU { get; set; }
        public decimal SaleQty { get; set; }
    }
}
