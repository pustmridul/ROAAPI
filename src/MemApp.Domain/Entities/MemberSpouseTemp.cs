using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities
{
    [Table("Member_spouse")]
    public class MemberSpouseTemp
    {
        [Key]
        public int Id { get; set; }
        public string? member_full_id { get; set; }
        public string? MemberShipNo { get; set; }
        public int? MemberId { get; set; }
        public string? status { get; set; }
        public string? anniversary { get; set; }
        public string? spouse { get; set; }
        public string? spouse_occupation { get; set; }
        public string? child1 { get; set; }
        public string? child1_name { get; set; }
        public string? child1_date_of_birth { get; set; }
        public string? child2 { get; set; }
        public string? child2_name { get; set; }
        public DateTime? child2_date_of_birth { get; set; }
    }
}
