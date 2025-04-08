using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.com
{
    public class TramsAndCondition : BaseEntity
    {
        public string Title { get; set; }
        public int TypeId { get; set; }
        public bool IsActive { get; set; }
        public int SlNo { get; set; }
    }
}
