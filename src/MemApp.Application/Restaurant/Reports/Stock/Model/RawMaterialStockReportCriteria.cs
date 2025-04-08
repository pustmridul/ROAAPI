using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Restaurant.Reports.Stock.Model
{
    public class RawMaterialStockReportCriteria
    {
        public string? RawmaterialGroup { get; set; }
        public string? RawMaterial { get; set; }
        public string? Department { get; set; }
        public bool WithZero { get; set; }
        public bool isSummary { get; set; }
        public bool isWithNegative { get; set; }
    }
}
