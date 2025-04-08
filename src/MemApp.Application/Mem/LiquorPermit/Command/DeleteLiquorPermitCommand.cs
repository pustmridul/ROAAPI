using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Domain.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Communication.Command
{
    public class DeleteLiquorPermitCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }
    public class DeleteLiquorPermitCommandHandler : IRequestHandler<DeleteLiquorPermitCommand, Result>
    {
        private readonly IMemDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IHostingEnvironment _hostingEnvironment;


        public DeleteLiquorPermitCommandHandler(IHostingEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor, IMemDbContext context, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _currentUserService = currentUserService;
            _permissionHandler = permissionHandler;
            _httpContextAccessor = httpContextAccessor;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<Result> Handle(DeleteLiquorPermitCommand request, CancellationToken cancellationToken)
        {
            //if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            //{
            //    throw new UnauthorizedAccessException();
            //}
            var result = new Result();
            try
            {
                string baseUrl = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host + _httpContextAccessor.HttpContext.Request.PathBase;

                var data = await _context.LiquorPermits.SingleOrDefaultAsync(q => q.Id == request.Id, cancellationToken);

                if (data == null)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    var existingFile = await _context.FileInformations.Where(q => q.OperationTypeId == (int)OperationTypeEnum.LiquorPermit && q.OperationId == data.Id).FirstOrDefaultAsync();
                    if (existingFile != null)
                    {
                        if (System.IO.File.Exists(existingFile.FileUrl))
                        {
                            System.IO.File.Delete(existingFile.FileUrl);
                        }
                        _context.FileInformations.Remove(existingFile);
                    }
                    _context.LiquorPermits.Remove(data);
                    await _context.SaveChangesAsync(cancellationToken);
                    result.HasError = false;
                    result.Messages.Add("Delete Success");
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add("Error" + ex.Message.ToString());
            }

            return result;
        }
    }
}
