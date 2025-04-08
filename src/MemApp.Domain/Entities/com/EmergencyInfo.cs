using MemApp.Domain.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MemApp.Domain.Entities.com
{
    public class EmergencyInfo : BaseEntity
    {
        public string Name { get; set; }= string.Empty;
        public string? Address { get; set; }
        public string ContactNo { get; set; }=string.Empty;
        public bool IsActive { get; set; }
    }
}
