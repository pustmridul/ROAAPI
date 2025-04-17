using MemApp.Domain.Core.Models;

namespace Res.Domain.Entities
{
    public class Municipality : BaseEntity
    {
      

        public string? EnglishName { get; set; }
        public string? BanglaName { get; set; }

        public bool? IsActive {  get; set; }

        public int? ThanaId { get; set; }
        public Thana? Thana { get; set; }

        public ICollection<Ward>? Wards { get; set; }

        public virtual ICollection<MemberRegistrationInfo>? MemberRegistrationInfos { get; set; }
    }
}
