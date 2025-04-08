using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemApp.Domain.Entities.mem;

namespace MemApp.Domain.Entities.com
{
    [Table("com_FeedbackCategory")]
    public class FeedbackCategory : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new int Id { get; set; }  // Hides the base Id property
        public string Name { get; set; }
        public int TaggedDirectorId { get; set; }

        [ForeignKey(nameof(TaggedDirectorId))] // Foreign key attribute
        public RegisterMember RegisterMember { get; set; }
    }
}
