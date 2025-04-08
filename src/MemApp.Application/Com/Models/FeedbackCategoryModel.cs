using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Com.Models
{
    public class FeedbackCategoryModel
    {
    }

    public class FeedbackCategoryReq
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TaggedDirectorId { get; set; }
        public string TaggedDirectorName { get; set; }
        

    }
}
