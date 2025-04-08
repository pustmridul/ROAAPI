using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Restaurant.Reports.Stock.Model
{
    public class RawMaterialConsumptionReportCriteria
    {
        public string? RawmaterialGroup { get; set; }
        public string? RawMaterial { get; set; }
        public string? Department { get; set; }
        public bool isSummary { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
