using MemApp.Application.Interfaces;
using MemApp.Application.Models;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MemApp.WebApi
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public int UserId { get; }
        public string Username { get; }
        public string AppId { get; }
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
            UserId = Convert.ToInt32(httpContextAccessor.HttpContext?.User?.FindFirstValue("uid"));
            Username = httpContextAccessor.HttpContext?.User?.FindFirstValue("user_name") ?? "";
            AppId = httpContextAccessor.HttpContext?.User?.FindFirstValue("AppId") ?? "";
        }

        public UserProfile Current()
        {
            var currentUser = _contextAccessor.HttpContext?.User;


            if (currentUser == null)
                return new UserProfile();

            if (!currentUser.HasClaim(c => c.Type == ClaimTypes.UserData))
                return new UserProfile();

            var userData = currentUser.Claims.Single(c => c.Type == ClaimTypes.UserData).Value;
            return JsonConvert.DeserializeObject<UserProfile>(userData) ?? new UserProfile();

        }

        //public UserProfile Current
        //{
        //    get
        //    {
        //        if (_current != null)
        //            return _current;

        //        var currentUser = _contextAccessor.HttpContext?.User;
        //        if (currentUser == null)
        //            return null;

        //        if (!currentUser.HasClaim(c => c.Type == ClaimTypes.UserData))
        //            return null;

        //        var userData = currentUser.Claims.Single(c => c.Type == ClaimTypes.UserData).Value;
        //        this._current = JsonConvert.DeserializeObject<UserProfile>(userData);

        //        return _current;
        //    }
        //    set => _current = value;
        //}

    }
}

