using Hangfire;
using MediatR;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Application.Mem.MemServices.Queries;
using MemApp.Application.MessageInboxs.Commands.MessageGenerateCommand;
using MemApp.Application.MessageInboxs.Models;
using MemApp.Application.Models;
using MemApp.Application.Services;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.ser;
using MemApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;


namespace MemApp.Application.Mem.ServiceTickets.Command
{
    public class CreateServiceTicketCommand : IRequest<ServiceTicketVm>
    {
        public ServiceTicketReq Model { get; set; } = new ServiceTicketReq();
        public string WebRootPath { get; set; }= string.Empty;
    }

    public class ServiceTicketServiceTicketCommandHandler : IRequestHandler<CreateServiceTicketCommand, ServiceTicketVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileService _fileService;
        private readonly IMediator _mediator;
        private readonly IBackgroundJobClientV2 _backgroundJobClientV2;

        public ServiceTicketServiceTicketCommandHandler(IMemDbContext context, IMediator mediator,
            IPermissionHandler permissionHandler, ICurrentUserService currentUserService, IFileService fileService
            , IBackgroundJobClientV2 backgroundJobClientV2)
        {
            _context = context;
            _mediator = mediator;
            _permissionHandler = permissionHandler;
            _currentUserService = currentUserService;
            _fileService = fileService;
            _backgroundJobClientV2 = backgroundJobClientV2;
        }
        public async Task<ServiceTicketVm> Handle(CreateServiceTicketCommand request, CancellationToken cancellation)
        {
            if (_currentUserService?.UserId == null || _currentUserService.UserId == 0)
            {
                throw new UnauthorizedAccessException();
            }
            bool newObj = false;
            var result = new ServiceTicketVm();
            try
            {
               
                var obj = await _context.ServiceTickets
                    .Include(i=>i.ServiceTicketDetails)
                    .Include(i=>i.EventTokens)
                    .Include(i=>i.SerTicketAreaLayouts)
                .SingleOrDefaultAsync(q => q.Id == request.Model.Id);

                if (obj == null)
                {
                    var isservice = await _context.ServiceTickets
                        .Where(q=>q.IsActive && q.MemServiceTypeId==7)
                        .AsNoTracking()
                        .ToListAsync(cancellation);

                    var has= isservice.Any(q=>q.MemServiceId==request.Model.MemServiceId);
                    if (has)
                    {
                        result.HasError= true;
                        result.Messages.Add("This Service already used");
                       return result;
                    }
                   
                    
                    if(!await _permissionHandler.HasRolePermissionAsync(2901))
                    {
                        result.HasError = true;
                        result.Messages?.Add("Service Ticket Create Permission Denied");
                        return result;
                    }
                    obj = new ServiceTicket();
                    obj.IsActive = true;
                    obj.Status = true;
                    obj.MemServiceId = request.Model.MemServiceId;
                    obj.MemServiceTypeId = request.Model.MemServiceTypeId;
                    _context.ServiceTickets.Add(obj);
                    result.HasError = false;
                    result.Messages?.Add("New ServiceTicket created");

                    newObj= true;
                }
                else
                {

                    if (!await _permissionHandler.HasRolePermissionAsync(2902))
                    {
                        result.HasError = true;
                        result.Messages?.Add("Service Ticket Update Permission Denied");
                        return result;
                    }
                    result.HasError = false;
                    result.Messages?.Add("ServiceTicket Updated");
                }

                obj.Title = request.Model.Title;
                obj.VatChargePercent = request.Model.VatChargePercent ??0;
                obj.VatChargeAmount = obj.VatChargePercent * 100;
                obj.ServiceChargePercent = request.Model.ServiceChargePercent ?? 0;
                obj.ServiceChargeAmount = obj.ServiceChargePercent * 100;
                obj.Description = request.Model.Description;


                obj.Location = request.Model.Location;
                obj.StartDate = request.Model.StartDate;
                obj.EndDate = request.Model.EndDate;
                obj.EventDate = request.Model.EventDate;

                obj.PromoCode = request.Model.PromoCode;
               
                obj.AvailabilityId= request.Model.AvailabilityId;
                obj.TicketLimit = request.Model.TicketLimit;
                if (request.Model.formFile!=null)
                {
                    obj.ImgFileUrl = "Tickets" + "/" + request.Model.formFile.FileName;
                }
        

                await _context.SaveChangesAsync(cancellation);

               
                if (request.Model.formFile!=null)
                {
                    var UploadTicketModel = new UploadTicketModel
                    {
                        TicketId = obj.Id,
                        SubDirectory = "Tickets",
                        file = request.Model.formFile
                    };
                    await _fileService.UploadTicketImage(UploadTicketModel, request.WebRootPath);
                }
               


                if (request.Model.ServiceTicketDetailReqs?.Count > 0)
                {
                    obj.HasTicket = true;
                    
                }
                if (request.Model.EventTokenReqs?.Count > 0)
                {
                    obj.HasToken = true;
                }
                if (request.Model.SerTicketAreaLayoutReqs?.Count > 0)
                {
                    obj.HasAreaLayout = true;
                }

                if(request.Model.ServiceTicketDetailReqs?.Count>0)
                foreach (var std in request.Model.ServiceTicketDetailReqs)
                 {
                    var objTicket = await _context.ServiceTicketDetails.SingleOrDefaultAsync(q=>q.Id == std.Id && q.ServiceTicketId== obj.Id
                    && q.ServiceTicketTypeId==std.ServiceTicketTypeId, cancellation);
                    if (objTicket == null)
                    {
                        objTicket = new ServiceTicketDetail()
                        {
                            ServiceTicketId = obj.Id,
                     
                            ServiceTicketTypeId = std.ServiceTicketTypeId,
                        };

                        obj.ServiceTicketDetails.Add(objTicket);             
                    }
                    objTicket.Quantity = std.Quantity;
                    objTicket.MaxQuantity = std.MaxQuantity;
                    objTicket.MinQuantity= std.MinQuantity;
                    objTicket.TicketType = std.TicketType;
                    objTicket.UnitPrice=std.UnitPrice;

                }

                if (request.Model.ServiceTicketAvailabilityReqs?.Count > 0)
                {
                    foreach (var availability in request.Model.ServiceTicketAvailabilityReqs)
                    {
                        var objTicket = await _context.ServiceTicketAvailabilities.SingleOrDefaultAsync(q => q.Id == availability.Id
                        , cancellation);
                        if (objTicket == null)
                        {
                            objTicket = new ServiceTicketAvailability()
                            {
                                ServiceTicketId = obj.Id,
                                DayText = availability.DayText,
                                StartTime = availability.StartTime,
                                EndTime = availability.EndTime,
                                IsWholeDay = availability.IsWholeDay,
                                Qty = availability.Qty,
                                SlotId = availability.SlotId,
                            };

                            obj.ServiceTicketAvailabilities.Add(objTicket);
                        }
                        objTicket.ServiceTicketId = availability.ServiceTicketId;
                        objTicket.DayText = availability.DayText;
                        objTicket.StartTime = availability.StartTime;
                        objTicket.EndTime = availability.EndTime;
                        objTicket.IsWholeDay = availability.IsWholeDay;
                        objTicket.SlotId = availability.SlotId;
                        objTicket.Qty = availability.Qty;
                    }

                    
                    var requestIds = request.Model.ServiceTicketAvailabilityReqs.Select(c => c.Id).ToList();
                    var allIds = _context.ServiceTicketAvailabilities.Where(c=>c.ServiceTicketId== request.Model.Id).AsNoTracking().Select(c => c.Id).ToList();
                    var deletableIds = allIds.Where(c => !requestIds.Contains(c)).ToList();
                    if (deletableIds.Any())
                    {
                        var deletable = _context.ServiceTicketAvailabilities.Where(c => deletableIds.Contains(c.Id));
                        _context.ServiceTicketAvailabilities.RemoveRange(deletable);
                    }
                    obj.HasAvailability= true;
                   
                }
               

                if (request.Model.EventTokenReqs?.Count > 0) 
                foreach (var et in request.Model.EventTokenReqs)
                {
                    var objDetail = await _context.EventTokens.SingleOrDefaultAsync(q => q.Id == et.Id, cancellation);
                    if (objDetail == null)
                    {
                        objDetail = new EventToken()
                        {
                            ServiceTicketId = obj.Id,    
                            TokenCode = et.TokenCode,
                            TokenTitle = et.TokenTitle,
                            IsActive= true
                            
                        };

                        obj.EventTokens.Add(objDetail);
                    }
                }

                if(request.Model.SerTicketAreaLayoutReqs?.Count>0)
                foreach (var sta in request.Model.SerTicketAreaLayoutReqs)
                {
                    var objDetail  = await _context.SerTicketAreaLayouts
                        .SingleOrDefaultAsync(q => q.AreaLayoutId == sta.AreaLayoutId && q.ServiceTicketId== obj.Id 
                    , cancellation);

                    if (objDetail == null)
                    {
                        objDetail = new SerTicketAreaLayout()
                        {
                            ServiceTicketId = obj.Id,
                            AreaLayoutTitle = sta.Title,
                            AreaLayoutId = sta.AreaLayoutId,
                        };

                        obj.SerTicketAreaLayouts.Add(objDetail);
                    }               
                }
              
                var toBeDelete = new List<ServiceTicketDetail>();
                 foreach (var od in obj.ServiceTicketDetails)
                 {
                    if (od.Id != 0)
                    {
                        var has = request.Model.ServiceTicketDetailReqs.Any(q => q.Id == od.Id);
                        if (!has)
                        {
                            toBeDelete.Add(od);
                        }
                    }
                     
                 }
                
                 foreach (var od in obj.EventTokens)
                 {
                    if(od.Id != 0)
                    {
                        var has = request.Model.EventTokenReqs.Any(q => q.Id == od.Id);
                        if (!has)
                        {
                            od.IsActive = false;                    
                        }
                    }
                   
                 }
                 _context.ServiceTicketDetails.RemoveRange(toBeDelete);
                
                var toBeAlDelete = new List<SerTicketAreaLayout>();
                foreach (var od in obj.SerTicketAreaLayouts)
                {
                    if (od.Id != 0)
                    {
                        var has = request.Model.SerTicketAreaLayoutReqs.Any(q => q.Id == od.Id);
                        if (!has)
                        {
                            toBeAlDelete.Add(od);
                        }
                    }

                }
                _context.ServiceTicketDetails.RemoveRange(toBeDelete);
               
                _context.SerTicketAreaLayouts.RemoveRange(toBeAlDelete);
                await _context.SaveChangesAsync(cancellation);
                List<SerTicketAreaLayoutMatrix> SerAreaDetail = new List<SerTicketAreaLayoutMatrix>();
                List<SerTicketAreaLayoutMatrix> tobeMatrixDelete = new List<SerTicketAreaLayoutMatrix>();

                foreach (var std in request.Model.SerTicketAreaLayoutReqs)
                {

                    var areaLayout = obj.SerTicketAreaLayouts.FirstOrDefault(q => q.AreaLayoutId == std.AreaLayoutId);

                    var matrixList = await _context.SerTicketAreaLayoutMatrices.Where(q=>q.SerTicketAreaLayoutId== areaLayout.Id).ToListAsync(cancellation);



                    foreach (var sad in std.AreaLayoutDetails)
                    {
                        var objd= await _context.SerTicketAreaLayoutMatrices
                            .FirstOrDefaultAsync(q=>q.SerTicketAreaLayoutId== areaLayout.Id
                            && q.TableId== sad.TableId
                            && q.NoofChair== sad.NumberOfChair,cancellation
                            );

                        if (objd == null)
                        {
                            objd = new SerTicketAreaLayoutMatrix()
                            {
                                SerTicketAreaLayoutId = areaLayout.Id,
                                NoofChair = sad.NumberOfChair,
                                TableId = sad.TableId,
                                TableTitle = sad.TableName,
                            };
                            SerAreaDetail.Add(objd);
                        }                      
                    }

                    foreach(var smd in matrixList)
                    {
                        if (smd.Id != 0)
                        {
                            var has = std.AreaLayoutDetails.Any(q => q.Id == smd.Id);
                            if (!has)
                            {
                                tobeMatrixDelete.Add(smd);
                            }
                        }
                    }
                  
                }
                _context.SerTicketAreaLayoutMatrices.AddRange(SerAreaDetail);

                _context.SerTicketAreaLayoutMatrices.RemoveRange(tobeMatrixDelete);

                await _context.SaveChangesAsync(cancellation);



                //if (memberObj != null)
                //{
                //    var curentBalance = await _memLedgerService.GetCurrentBalance(memberObj.Id.ToString());
                //    string message = "";
                //    if (memberObj.Phone != null)
                //    {
                //        message = "Dear CCCL Member, Tk. " + Math.Round(obj.TotalAmount, 2) + " has been deducted from your wallet, current balance is Tk. " + Math.Round(curentBalance, 2) + ", Thanks.";


                //        result.Data.SmsText = message;
                //        result.Data.PhoneNo = memberObj.Phone;



                //    }


                //    var messageObj = new MessageInboxCreateDto
                //    {
                //        MemberId = obj.MemberId,
                //        Message = message,
                //        TypeId = MessageInboxTypeEnum.EventTicketSale,
                //        IsRead = false,
                //        IsAllMember = false,

                //    };
                //    _backgroundJobClientV2.Enqueue(() => ProcessMessage(new MessageGenerateCommand() { Model = messageObj }));
                //}
                if (request.Model.Id==null || request.Model.Id == 0)
                {
                    var memService = await _context.MemServices.FirstOrDefaultAsync(c => c.Id == request.Model.MemServiceId);
                    var messageObj = new MessageInboxCreateDto
                    {
                        Message = $"Event Launch: Dear Member, New event is launching. Event Name: {memService.Title}. Event Date: {request.Model.EventDate?.Date.ToString("dd MMMM yyyy")}",
                        TypeId = MessageInboxTypeEnum.EventLaunch,
                        IsAllMember = true

                    };
                    _backgroundJobClientV2.Enqueue(() => ProcessMessage(new MessageGenerateCommand() { Model = messageObj }));
                }
                




                return await _mediator.Send(new GetServiceTicketByIdQuery()
                {
                    Id = obj.Id
                });

            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages?.Add("Exception" + ex.Message);
            }
            return result;
        }
        private string GetMemorySizeString(long byteCount)
        {
            string[] sizeSuffixes = { "B", "KB", "MB", "GB", "TB" };
            int suffixIndex = 0;
            double size = byteCount;

            while (size >= 1024 && suffixIndex < sizeSuffixes.Length - 1)
            {
                size /= 1024;
                suffixIndex++;
            }

            return $"{size:0.##} {sizeSuffixes[suffixIndex]}";
        }
        public async Task ProcessMessage(MessageGenerateCommand command)
        {
            await _mediator.Send(command);
        }

    }


   

}