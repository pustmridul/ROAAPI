using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Service.Model;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.ser;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.MemServices.Command
{
    public class CreateAvailabilityCommand : IRequest<AvailabilityVm>
    {
        public AvailabilityRes Model { get; set; } = new AvailabilityRes();
    }

    public class CreateAvailabilityCommandHandler : IRequestHandler<CreateAvailabilityCommand, AvailabilityVm>
    {
        private readonly IMemDbContext _context;
        private readonly IMediator _mediator;
        private readonly IUserLogService _userLogService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPermissionHandler _permissionHandler;

        public CreateAvailabilityCommandHandler(IMemDbContext context, IMediator mediator, IUserLogService userLogService, ICurrentUserService currentUserService, IPermissionHandler permissionHandler)
        {
            _context = context;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _userLogService = userLogService;
            _permissionHandler = permissionHandler;
        }
        public async Task<AvailabilityVm> Handle(CreateAvailabilityCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            bool isNewObj = false;
            var result = new AvailabilityVm();
            
            try
            {
                var obj = await _context.Availabilities
                    .Include(i=>i.AvailabilityDetails)
                .SingleOrDefaultAsync(q => q.Id == request.Model.Id, cancellation);

                if (obj == null)
                {
                    if (!await _permissionHandler.HasRolePermissionAsync(2001))
                    {
                        result.HasError = true;
                        result.Messages.Add("Availability Save Permission Denied");
                        return result;
                    }
                    isNewObj = true;
                    obj = new Availability();
                    obj.IsActive = true;
                   
                    if(request.Model.IsLifeTime)
                    {
                        obj.IsLifeTime = request.Model.IsLifeTime;
                    }
                    else
                    {
                        obj.IsLifeTime = false;
                    }
                    obj.AvailabiltyDate = DateTime.Parse(request.Model.AvailabilityDate);
                    obj.IsLifeTime= request.Model.IsLifeTime;
                    _context.Availabilities.Add(obj);
                    result.HasError = false;
                    result.Messages.Add("New Availability created");
                }
                else
                {

                    if (!await _permissionHandler.HasRolePermissionAsync(2002))
                    {
                        result.HasError = true;
                        result.Messages.Add("Availability Update Permission Denied");
                        return result;
                    }

                    result.HasError = false;
                    result.Messages.Add("Availability Updated");
                }

                if (isNewObj)
                {
                    if(await _context.SaveChangesAsync(cancellation) > 0)
                    {
                        foreach (var ad in request.Model.AvailabilityDetailVms)
                        {
                            var objDetail = new AvailabilityDetail()
                            {
                                AvailabilityId = obj.Id,
                                StartTime = DateTime.Parse(ad.StartTime).ToString(),
                                EndTime = DateTime.Parse(ad.EndTime).ToString(),
                                IsActive = true,
                                IsWholeDay = ad.IsWholeDay,
                                IsChecked = ad.IsChecked,
                                Title = ad.Title
                            };
                            _context.AvailabilityDetails.Add(objDetail);
                        }
                    }
                    else
                    {
                        result.HasError = true;
                        result.Messages.Add("opoos!!!!!");
                    }
                 
                }
                else
                {
                    foreach(var ad in request.Model.AvailabilityDetailVms)
                    {
                        var objDetail = obj.AvailabilityDetails.SingleOrDefault(q => q.AvailabilityId == obj.Id && q.IsActive && q.Id== ad.Id);

                        if(objDetail == null)
                        {
                            objDetail = new AvailabilityDetail()
                            {
                                AvailabilityId = obj.Id,         
                                IsActive = true,
                           
                            };
                            _context.AvailabilityDetails.Add(objDetail);
                        }
                        objDetail.StartTime = DateTime.Parse(ad.StartTime).ToString();
                        objDetail.EndTime = DateTime.Parse(ad.EndTime).ToString();
                        objDetail.IsWholeDay= ad.IsWholeDay;
                        objDetail.IsChecked= ad.IsChecked;
                        objDetail.Title = ad.Title;

                    }
                    foreach(var ad in obj.AvailabilityDetails)
                    {
                        var exist = request.Model.AvailabilityDetailVms.SingleOrDefault(q => q.Id == ad.Id);
                        if(exist == null)
                        {
                            ad.IsActive = false;
                        }
                    }
                }

                obj.Name = request.Model.Name;
                if (await _context.SaveChangesAsync(cancellation) > 0)
                {
                    result.Data.Id = obj.Id;
                }
                else
                {
                    result.HasError = true;
                    result.Messages.Add("Nothing Updated");
                }
               
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add("Exception" + ex.Message);       
            }
            return result;
        }
    }

}