using MemApp.Domain.Core.Models;

namespace Res.Domain.Entities
{
    public class Thana : BaseEntity
    {
       // public int Id { get; set; }

        public string? EnglishName { get; set; }
        public string? BanglaName { get; set; }
        //public DateTime? CreatedOn { get; set; }

        public bool? IsActive {  get; set; }

        public int? DistrictId { get; set; }
        public District? District { get; set; }

        public virtual ICollection<MemberRegistrationInfo>? MemberRegistrationInfos { get; set; }
    }
}
