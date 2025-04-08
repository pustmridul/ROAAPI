using MemApp.Application.Models.DTOs;
using MemApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Interfaces
{
    public interface IUserLogService
    {
        Task<bool> InsertLog(UserLogReq model);
        Task<UserLogListVm> GetAllUserLogsByUserId( int pageNo, int pageSize, string userId);
        Task<UserLogListVm> GetAllUsersLog(int pageNo, int pageSize);     
    }
}
