using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.Models.DTOs
{
    public class ThanaDto
    {
        public int Id { get; set; }

        public string? EnglishName { get; set; }
        public string? BanglaName { get; set; }
        public DateTime? CreatedOn { get; set; }

        public int DistrictId { get; set; }
    }
}
