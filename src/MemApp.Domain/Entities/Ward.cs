using MemApp.Domain.Core.Models;

namespace Res.Domain.Entities
{
    public class Ward : BaseEntity
    {
        // public int Id { get; set; }

        public string? Sequence { get; set; }
        public string? EnglishName { get; set; }
        public string? BanglaName { get; set; }
        //public DateTime? CreatedOn { get; set; }

        public bool? IsActive {  get; set; }

        public int? ThanaId { get; set; }
        public Thana? Thana { get; set; }

        public int? MunicipalityId { get; set; }
        public Municipality? Municipality { get; set; }

        public int? UnionInfoId { get; set; }
        public UnionInfo? UnionInfo { get; set; }

        //public int? UnionId { get; set; }
        //public Union? Union { get; set; }

        public virtual ICollection<MemberRegistrationInfo>? MemberRegistrationInfos { get; set; }
    }
}
