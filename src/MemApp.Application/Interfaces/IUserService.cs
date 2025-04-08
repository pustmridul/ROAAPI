using MemApp.Application.Models.DTOs;
using System.Data;

namespace MemApp.Application.Interfaces;

public interface IUserService
{
    Task<UserLogListVm> GetAllUserLogs(int pageNo, int pageSize);
    Task<UserLogListVm> GetAllUserLogsByUserId(int pageNo, int pageSize, string UserId);
}