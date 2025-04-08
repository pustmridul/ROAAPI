using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Restaurant.Reports.Stock.Model
{
    public class RawMaterialStockReportVM
    {
        public string? ShopId { get; set; }
        public DateTime ExpireDate { get; set; }
        public int RawMeterialId { get; set; }
        public decimal CPU { get; set; }
        public decimal RcvQty { get; set; }
        public decimal BonusQty { get; set; }
        public decimal SalesQty { get; set; }
        public decimal SalesRQty { get; set; }
        public decimal SupRetQty { get; set; }
        public decimal DMLQty { get; set; }
        public decimal BAL_QTY { get; set; }
        public int UnitId { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public decimal WoffQty { get; set; }
        public decimal WonQty { get; set; }
        public string? Name { get; set; }
        public string? UnitName { get; set; }
        public string? DepartmentName { get; set; }
        public decimal BalanceValue { get; set; }
    }
}
