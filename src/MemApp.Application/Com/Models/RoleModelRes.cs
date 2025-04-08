using MemApp.Application.Extensions;

namespace MemApp.Application.Models.Responses
{
    
    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RoleUsers {  get; set; }
    }


   
}
