using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.Interfaces
{
    public interface IPasswordNewHash
    {
        string CreateHash(string password, ref string passwordHash, ref string passwordSalt);
        bool ValidatePassword(string password, string passwordHash, string passwordSalt);
        string GetEncryptedPassword(string password);
    }
}
