using MemApp.Application.Core.Models;

namespace MemApp.Application.Core.Services
{
    public interface IEmailService
    {
        void SendEmail(Email email);
    }
}