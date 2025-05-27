using MemApp.Domain.Core.Models;
using Res.Domain.Entities;

namespace MemApp.Domain.Entities
{
    public class UserMenuMap : BaseEntity
    {
        public int UserId { get; set; }
        public int MenuId { get; set; }
        public Menu? Menu { get; set; }
        public User? User { get; set; }

    }
}
