using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities
{
    public class Menu:BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? ModuleName { get; set; } = string.Empty;
        public string? Url { get; set; }
        public int DisplayOrder { get; set; }
        public bool Visible { get; set; }
        public string NavIcon { get; set; } = string.Empty; 
        public bool IsActive { get; set; }
        public int? ParentId { get; set; }
        public Menu? Parent {  get; set; }
        public  ICollection<Menu>? Childs { get; set; }
        public virtual ICollection<UserMenuMap> UserMenuMaps { get; set; }
    }
}
