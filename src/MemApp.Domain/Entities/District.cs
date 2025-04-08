using MemApp.Domain.Core.Models;

namespace Res.Domain.Entities;

public class District :BaseEntity
{
  //  public int Id { get; set; }

    public string? EnglishName { get; set; }
    public string? BanglaName { get; set; }
 //   public DateTime? CreatedOn { get; set; }

    public int? DivisionId { get; set; }  // Foreign key for District
    public Division? Division { get; set; }

    public bool? IsActive {  get; set; }

    public ICollection<Thana>? Thanas { get; set; }


    public virtual ICollection<MemberRegistrationInfo>? MemberRegistrationInfos { get; set; }
}
