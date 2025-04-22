using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Res.Domain.Entities
{
    public class RoaMemberCategory : BaseEntity
    {
        public int? SeatQuantity {  get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }=true;
    }

   
}
