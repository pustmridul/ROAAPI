using MemApp.Application.Core.Repositories;
using MemApp.Application.Interfaces;
using MemApp.Application.Models.DTOs;

namespace MemApp.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserLogService _userLogService;

    public UserService(IUnitOfWork unitOfWork, IUserLogService userLogService)
    {
        _unitOfWork = unitOfWork;
        _userLogService = userLogService;
    }

    public async Task<UserLogListVm> GetAllUserLogs(int pageNo, int pageSize)
    {
        return await _userLogService.GetAllUsersLog(pageNo, pageSize);
    }

    public async Task<UserLogListVm> GetAllUserLogsByUserId(int pageNo, int pageSize, string UserId)
    {
        return await _userLogService.GetAllUserLogsByUserId(pageNo, pageSize, UserId);
    }
}