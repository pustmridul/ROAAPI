using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.ROA.MunicipalityInfo.Models
{
    public class WardDto
    {
        public int Id { get; set; }

        public string? EnglishName { get; set; }
        public string? BanglaName { get; set; }
        public DateTime? CreatedOn { get; set; }

        public int? ThanaId { get; set; }

        public string? ThanaName { get; set; }

        public int? MunicipalityId { get; set; }

        public string? MunicipalityName { get; set; }

        public int? UnionInfoId { get; set; }

        public string? UnionInfoName { get; set; }
    }
}
