using Dapper;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models.DTOs;
using MemApp.Domain.Entities;
using System.Text;

namespace MemApp.Application.Services
{
    public class UserLogService : IUserLogService
    {
        private IDapperContext _context;
        private IMemDbContext _memDbContext;
        public UserLogService(IDapperContext context, IMemDbContext memDbContext)
        {
            _context = context;
            _memDbContext = memDbContext;
        }
        public async Task<bool> InsertLog(UserLogReq model)
        {
            try
            {
                var logObj = new UserLog()
                {
                    UserId = model.UserId,
                    UserName = model.UserName,
                    LogDate = DateTime.Now,
                    LogText = model.LogText,
                };
                _memDbContext.UserLogs.Add(logObj);
                await _memDbContext.SaveAsync();

            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            if (model == null)
            {
                return false;
            }
            else return true;
        }
        public async Task<UserLogListVm> GetAllUsersLog(int pageNo, int pageSize)
        {

            var result = new UserLogListVm();

            int offset = (pageNo - 1) * pageSize;
            using (var connection = _context.CreateConnection())
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(@"SELECT  MessageTemplate As LogText, mu.UserName, FORMAT([TimeStamp], 'yyyy-MM-dd HH:mm:ss') as LogDate,");
                sb.AppendLine(" CAST(CAST(Properties AS XML).value('(/properties/property[@key=\"UserId\"]/text())[1]', 'NVARCHAR(MAX)') AS INT) AS UserId");
                sb.AppendLine("FROM Logs");
                sb.AppendLine("JOIN mem_User mu on CAST(CAST(Properties AS XML).value('(/properties/property[@key=\"UserId\"]/text())[1]', 'NVARCHAR(MAX)') AS INT)=mu.Id");
                sb.AppendLine("ORDER BY Logs.Id desc  OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;");
                var data = await connection.QueryAsync<UserLogReq>(sb.ToString(), new { Offset = offset, PageSize = pageSize });
                sb = new StringBuilder();
                sb.AppendLine("select Count(*) AS UserName from Logs");
                var datacount = await connection.QueryAsync<UserLogReq>(sb.ToString());
                result.DataCount = Convert.ToInt32(datacount.FirstOrDefault()!.UserName);
                result.DataList = data.ToList();
            }
              return result;
        }
        public async Task<UserLogListVm> GetAllUserLogsByUserId(int pageNo, int pageSize, string userId)
        {
            var result = new UserLogListVm();

            int offset = (pageNo - 1) * pageSize;
            using (var connection = _context.CreateConnection())
            {
                StringBuilder sb = new StringBuilder();
               
                sb.AppendLine(@"SELECT  MessageTemplate As LogText, mu.UserName,FORMAT([TimeStamp], 'yyyy-MM-dd HH:mm:ss') as LogDate,");
                sb.AppendLine(" CAST(CAST(Properties AS XML).value('(/properties/property[@key=\"UserId\"]/text())[1]', 'NVARCHAR(MAX)') AS INT) AS UserId");
                sb.AppendLine("FROM Logs");
                sb.AppendLine("JOIN mem_User mu on CAST(CAST(Properties AS XML).value('(/properties/property[@key=\"UserId\"]/text())[1]', 'NVARCHAR(MAX)') AS INT)=mu.Id");
                sb.AppendLine("WHERE CAST(CAST(Properties AS XML).value('(/properties/property[@key=\"UserId\"]/text())[1]', 'NVARCHAR(MAX)') AS INT) =@UserId");
                sb.AppendLine("ORDER BY Logs.Id desc OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;");
   
                var data = await connection.QueryAsync<UserLogReq>(sb.ToString(), new { Offset = offset, PageSize = pageSize , UserId=userId });

                sb= new StringBuilder();
                sb.AppendLine("select Count(*) AS UserName from Logs WHERE CAST(CAST(Properties AS XML).value('(/properties/property[@key=\"UserId\"]/text())[1]', 'NVARCHAR(MAX)') AS INT) =@UserId");
                var datacount= await connection.QueryAsync<UserLogReq>(sb.ToString(), new { UserId = userId });
                result.DataCount = Convert.ToInt32(datacount.FirstOrDefault()!.UserName);
                result.DataList = data.ToList();
            }
           
            return result;
        }

      
    }
}
