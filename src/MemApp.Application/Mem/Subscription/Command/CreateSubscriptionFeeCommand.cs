using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Subscription.Model;
using MemApp.Domain.Entities.subscription;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Subscription.Command
{
    public class CreateSubscriptionFeeCommand : IRequest<Result<SubscriptionFeeDto>>
    {
        public SubscriptionFeeReq Model { get; set; } = new SubscriptionFeeReq();
    }

    public class CreateSubscriptionFeeCommandHandler : IRequestHandler<CreateSubscriptionFeeCommand, Result<SubscriptionFeeDto>>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        public CreateSubscriptionFeeCommandHandler(IMemDbContext context, IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;
        }
        public async Task<Result<SubscriptionFeeDto>> Handle(CreateSubscriptionFeeCommand request, CancellationToken cancellation)
        {
            var result = new Result<SubscriptionFeeDto>();
            if(!await _permissionHandler.HasRolePermissionAsync(2701))
            {
                result.HasError = true;
                result.Messages.Add("Subscription Fee Create Permission Denied");
                return result;
            }

            try
            {
                var obj = await _context.SubscriptionFees
                    .SingleOrDefaultAsync(q => q.Id == request.Model.Id && q.IsActive, cancellation);

                if (obj == null)
                {
                    var sobj = await _context.SubscriptionFees
                       .Where(q => q.SubscribedYear == request.Model.SubscribedYear
                       && q.SubscriptionModId == request.Model.SubscriptionModId && q.IsActive).ToListAsync(cancellation);
                    if (sobj.Count > 0)
                    {
                        result.HasError = true;
                        result.Messages.Add("Already Added");
                        return result;
                    }

                    obj = new SubscriptionFees()
                    {
                        IsActive = true,
                        SubscribedYear = request.Model.SubscribedYear,
                        SubscriptionModId = request.Model.SubscriptionModId,                                           
                    };

                    var q = request.Model.SubscriptionModId;
                    if (q == 2)
                    {
                        obj.Title = "Q1";
                        obj.StartDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 01, 01);
                        obj.EndDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 03, 31);

                    }
                    else if (q == 3)
                    {
                        obj.Title = "Q2";
                        obj.StartDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 04, 01);
                        obj.EndDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 06, 30);
                    }
                    else if (q == 4)
                    {
                        obj.Title = "Q3";
                        obj.StartDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 07, 01);
                        obj.EndDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 09, 30);
                    }
                    else if (q == 5)
                    {
                        obj.Title = "Q4";
                        obj.StartDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 10, 01);
                        obj.EndDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 12, 31);
                    }
                    else
                    {
                        obj.Title = "";
                        obj.StartDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 01, 01);
                        obj.EndDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 12, 31);
                    }

                    result.HasError = false;
                    result.Messages.Add("add new Subscription Fee Success");
                    _context.SubscriptionFees.Add(obj);

                }
                else
                {
                   

                    result.HasError = false;
                    result.Messages.Add("Update new Subscription Fee Success");
                }
                obj.SubscriptionFee = request.Model.SubscriptionFee;
                obj.LateFee = request.Model.LateFee;
                obj.AbroadFee = request.Model.AbroadFee;

                await _context.SaveChangesAsync(cancellation);
            }
            catch(Exception ex)
            {
                result.HasError = true;
                result.Messages.Add("Error " + ex.Message);
            }

            //if(request.Model.CommandType == "Generate")
            //{
            //    int startYear = Convert.ToInt32(!string.IsNullOrEmpty(request.Model.SubscribedYear) ? request.Model.SubscribedYear : "0");
            //    int endYear = Convert.ToInt32(!string.IsNullOrEmpty(request.Model.SubscribedEndYear) ? request.Model.SubscribedEndYear : "0");

            //    if (startYear < endYear)
            //    {
            //        List<SubscriptionFees> objList = new List<SubscriptionFees>();
            //        for (int i = startYear; i < endYear; i++)
            //        {                        
            //            foreach (var q in request.Model.SubscriptionModIds)
            //            {
            //                var sobj = new SubscriptionFees()
            //                {
            //                    IsActive = true,
            //                    SubscribedYear = i.ToString(),
            //                    SubscriptionModId = q,
            //                    Title= q.ToString(),
            //                    StartDate = DateTime.Now,
            //                    EndDate = DateTime.Now,

            //                    SubscriptionFee = request.Model.SubscriptionFee,
            //                    LateFee = request.Model.LateFee,
            //                    AbroadFee = request.Model.AbroadFee
            //                };
            //                objList.Add(sobj);
            //            }
                       
            //        }
            //        _context.SubscriptionFees.AddRange(objList);
            //       if( await _context.SaveChangesAsync(cancellation) > 0 )
            //       {
            //            result.HasError = false;
            //            result.Messages.Add("Success");
            //       }
            //    }
            //}
            //else
            //{            
            //    var obj = await _context.SubscriptionFees.Where
            //  (q => q.SubscribedYear == request.Model.SubscribedYear && q.IsActive).ToListAsync(cancellation);

            //    if (obj.Count == 0)
            //    {
            //        List<SubscriptionFees> objList = new List<SubscriptionFees>();

            //        foreach (var q in request.Model.SubscriptionModIds)
            //        {
            //            var sobj = new SubscriptionFees()
            //            {
            //                IsActive = true,
            //                SubscribedYear = request.Model.SubscribedYear,
            //                SubscriptionModId = q,
            //                SubscriptionFee = request.Model.SubscriptionFee,
            //                LateFee = request.Model.LateFee,
            //                AbroadFee = request.Model.AbroadFee,                         
                         
            //            };
            //            if (q == 2)
            //            {
            //                sobj.Title = "Q1";
            //                sobj.StartDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 01, 01);
            //                sobj.EndDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 03, 31);

            //            }
            //            else if (q == 3)
            //            {
            //                sobj.Title = "Q2";
            //                sobj.StartDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 04, 01);
            //                sobj.EndDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 06, 30);
            //            }
            //            else if (q == 4)
            //            {
            //                sobj.Title = "Q3";
            //                sobj.StartDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 07, 01);
            //                sobj.EndDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 09, 30);
            //            }
            //            else if(q==5)
            //            {
            //                sobj.Title = "Q4";
            //                sobj.StartDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 10, 01);
            //                sobj.EndDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 12, 31);
            //            }
            //            else
            //            {
            //                sobj.Title = "";
            //                sobj.StartDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 01, 01);
            //                sobj.EndDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 12, 31);
            //            }
            //            objList.Add(sobj);
            //        }
            //        _context.SubscriptionFees.AddRange(objList);

            //        await _context.SaveChangesAsync(cancellation);

            //    }
            //    else
            //    {
            //        foreach (var q in request.Model.SubscriptionModIds)
            //        {
            //            var sobj = obj.SingleOrDefault(q1 => q1.SubscriptionModId == q);
            //            if (sobj == null)
            //            {
            //                sobj = new SubscriptionFees()
            //                {
            //                    IsActive = true,
            //                    SubscribedYear = request.Model.SubscribedYear,
            //                    SubscriptionModId = q,
            //                    SubscriptionFee = request.Model.SubscriptionFee,
            //                    LateFee = request.Model.LateFee,
            //                    AbroadFee = request.Model.AbroadFee
            //                };
            //                _context.SubscriptionFees.Add(sobj);
            //            }
            //            if (q == 2)
            //            {
            //                sobj.Title = "Q1";
            //                sobj.StartDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 01, 01);
            //                sobj.EndDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 03, 31);

            //            }
            //            else if (q == 3)
            //            {
            //                sobj.Title = "Q2";
            //                sobj.StartDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 04, 01);
            //                sobj.EndDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 06, 30);
            //            }
            //            else if (q == 4)
            //            {
            //                sobj.Title = "Q3";
            //                sobj.StartDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 07, 01);
            //                sobj.EndDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 09, 30);
            //            }
            //            else if (q == 5)
            //            {
            //                sobj.Title = "Q4";
            //                sobj.StartDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 10, 01);
            //                sobj.EndDate = new DateTime(Convert.ToInt32(request.Model.SubscribedYear), 12, 31);
            //            }
            //            sobj.SubscriptionFee = request.Model.SubscriptionFee;
            //            sobj.LateFee = request.Model.LateFee;
            //            sobj.AbroadFee = request.Model.AbroadFee;

            //        }

            //        if (await _context.SaveChangesAsync(cancellation) > 0)
            //        {
            //            result.Data.SubscribedYear = request.Model.SubscribedYear;
            //            result.Data.LateFee = request.Model?.LateFee;
            //            result.Data.AbroadFee = request?.Model?.AbroadFee;
            //            result.HasError = false;
            //            result.Messages.Add("Data Saved");
            //        }
            //        else
            //        {
            //            result.HasError = true;
            //            result.Messages.Add("something wrong");
            //        }

            //    }
            //}
                    
            return result;
        }

    }
}
