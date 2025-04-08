using MemApp.Application.Models;

namespace MemApp.Application.Interfaces
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        public string Username { get; }
        public string AppId { get; }
        

        public UserProfile Current();
    }
}
