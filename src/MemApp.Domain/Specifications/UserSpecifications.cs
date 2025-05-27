using MemApp.Domain.Enums;
using MemApp.Domain.Core.Specifications;
using Res.Domain.Entities;

namespace MemApp.Domain.Specifications
{
    public static class UserSpecifications
    {
        public static BaseSpecification<User> GetUserByEmailAndPasswordSpec(string emailId, string password)
        {
            return new BaseSpecification<User>(x => x.EmailId == emailId && x.PasswordHash == password);
        }

        public static BaseSpecification<User> GetAllActiveUsersSpec()
        {
            return new BaseSpecification<User>(x => x.IsActive);
        }
    }
}
