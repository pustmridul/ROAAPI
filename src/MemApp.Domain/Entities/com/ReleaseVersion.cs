using MemApp.Domain.Core.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MemApp.Domain.Entities.com
{
    public class ReleaseVersion : BaseEntity
    {
        public string ReleaseTitle { get; set; } = string.Empty;
        public string ReleaseType { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public bool IsRequired { get; set; }
        public string AppId { get; set; } = string.Empty;
    }
}
