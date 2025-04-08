using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities.mem;

public class Donation : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? FileUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsFixed { get; set; }
    public decimal? Amount { get; set; }

    public ICollection<Donate> Donates { get; set; }
}
