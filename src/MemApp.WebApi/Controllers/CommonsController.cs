using MediatR;
using MemApp.Application.com.Queries.GetAllBanks;
using MemApp.Application.com.Queries.GetAllCreditCards;
using MemApp.Application.com.Queries.GetAllPaymentMethods;
using MemApp.Application.Com.Commands;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Mem.Members.Queries;
using MemApp.Application.Mem.MemberTypes.Queries;
using MemApp.Application.Models.DTOs;
using MemApp.Domain.Entities;
using MemApp.Domain.Entities.mem;
using MemApp.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Net.Mail;
using System.Net;
using Hangfire;

namespace MemApp.WebApi.Controllers
{

    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommonsController : ApiControllerBase
    {
        private readonly MemDbContext _dbContext;
        private readonly IBroadcastHandler _broadcastHandler;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IBackgroundJobClientV2 _backgroundJobClient;
        public CommonsController(
            MemDbContext context,
            IBroadcastHandler broadcastHandler,
            IWebHostEnvironment webHostEnvironment, ISender sender, IBackgroundJobClientV2 backgroundJobClient) : base(sender)
        {
            _dbContext = context;
            _broadcastHandler = broadcastHandler;
            _webHostEnvironment = webHostEnvironment;
            _backgroundJobClient = backgroundJobClient;
        }
        //[HttpGet]
        //public async Task<IActionResult> SyncDueSubscription(DateTime date, string quaterNo)
        //{
        //    return Ok(await _subscription.IsDueSubscriptionSync(date, quaterNo));
        //}
        //[HttpGet]
        //public async Task<IActionResult> SyncLateFeeApply(DateTime date)
        //{
        //    return Ok(await _subscription.IsLateFeesApply(date));
        //}
        [HttpGet]
        public async Task<BloodGroupListVm> GetAllBloodGroup()
        {
            return await Mediator.Send(new GetAllBloodGroupQuery());
        }

        [HttpGet]
        public async Task<PaymentMethodListVm> GetAllPaymentMethod()
        {
            return await Mediator.Send(new GetAllPaymentMethodQuery());
        }
        [HttpGet]
        public async Task<BankListVm> GetAllBank()
        {
            return await Mediator.Send(new GetAllBankQuery());
        }
        [HttpGet]
        public async Task<CreditCardListVm> GetAllCreditCard()
        {
            return await Mediator.Send(new GetAllCreditCardQuery());
        }

        [HttpGet]
        public async Task<IActionResult> MemberSpouseSync()
        {
            var datalist = await _dbContext.MemberSpouseTemps.Where(q => q.MemberId != null).ToListAsync();

            foreach (var item in datalist)
            {
                if (item.child1_name != null)
                {
                    var objMemChild = new MemberChildren()
                    {
                        CadetNo = "",
                        ContactName = item.child1_name,
                        Dob = item.child1_date_of_birth == null ? null : DateTime.Parse(item.child1_date_of_birth),
                        Gender = item.child1 ?? "",
                        RegisterMemberId = item.MemberId ?? 0,
                        IsActive = true
                    };
                    _dbContext.MemberChildrens.Add(objMemChild);
                }
                if (item.child2_name != null)
                {
                    var objMemChild = new MemberChildren()
                    {
                        CadetNo = "",
                        ContactName = item.child2_name ?? "",
                        Dob = item.child2_date_of_birth == null ? null : item.child2_date_of_birth,
                        Gender = item.child1 ?? "",
                        RegisterMemberId = item.MemberId ?? 0,
                        IsActive = true
                    };
                    _dbContext.MemberChildrens.Add(objMemChild);
                }

                if (item.spouse != null)
                {
                    var obj = await _dbContext.RegisterMembers
                   .SingleOrDefaultAsync(q => q.Id == item.MemberId);
                    if (obj == null)
                    {

                    }
                    else
                    {
                        obj.Spouse = item.spouse;
                        obj.SpouseOccupation = item.spouse_occupation;
                        obj.Anniversary = item.anniversary == null ? null : DateTime.Parse(item.anniversary);

                        var spouseObj = await _dbContext.RegisterMembers.SingleOrDefaultAsync(q => q.MemberId == obj.Id && q.IsMasterMember == false && q.IsActive);
                        if (spouseObj == null)
                        {
                            spouseObj = obj;
                            spouseObj.IsMasterMember = false;
                            spouseObj.MemberId = obj.Id;
                            spouseObj.Id = 0;

                            _dbContext.RegisterMembers.Add(spouseObj);
                        }
                        spouseObj.FullName = item.spouse ?? obj.FullName;
                        spouseObj.PrvCusID = spouseObj.Id.ToString();
                        spouseObj.CardNo = obj.CardNo + "-01";
                        spouseObj.Spouse = item.spouse ?? "";
                        spouseObj.SpouseOccupation = item.spouse_occupation;
                        spouseObj.Anniversary = item.anniversary == null ? null : DateTime.Parse(item.anniversary);


                    }
                }
                await _dbContext.SaveAsync();
            }

            return Ok();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendSms(CustomSmsReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await _broadcastHandler.SendSms(model.PhoneNo, model.Message, model.LanType);
            SmsLog sms = new SmsLog()
            {
                PhoneNo = model.PhoneNo,
                Message = model.Message,
                SmsDate = DateTime.Now,
                MemberName = "",
                MemberShipNo = "",
                Status = result ? "Ok" : "No"
            };
            await _broadcastHandler.SaveSmsLog(sms);

            return Ok(result);
        }
        //[Authorize]
        //[HttpGet]
        //public async Task<IActionResult> SendEmail(string to, string subject, string body)
        //{
        //    var result =await _broadcastHandler.SendEmail(to, subject, body);
        //    return Ok(result);
        //}
        //[Authorize]
        //[HttpGet]
        //public async Task<IActionResult> SendEmailWithAttachment(string to, string subject, string body, string fileName)
        //{
        //    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "EmailedFiles", fileName);

        //    var result = await _broadcastHandler.SendEmailWithAttachmentAsync(to, subject, body,filePath);
        //    return Ok(result);
        //}
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendBulkSms(BulkSmsReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = new Result();
            foreach (var item in model.SmsReqList)
            {
                if (await _broadcastHandler.SendSms(item.PhoneNo, model.Message, model.LanType))
                {
                    SmsLog sms = new SmsLog()
                    {
                        PhoneNo = item.PhoneNo,
                        Message = model.Message,
                        SmsDate = DateTime.Now,
                        MemberName = item.MemberName,
                        MemberShipNo = item.MemberShipNo,
                        Status = "Ok"
                    };
                    await _broadcastHandler.SaveSmsLog(sms);
                }
                else
                {
                    SmsLog sms = new SmsLog()
                    {
                        PhoneNo = item.PhoneNo,
                        Message = model.Message,
                        SmsDate = DateTime.Now,
                        MemberName = item.MemberName,
                        MemberShipNo = item.MemberShipNo,
                        Status = "No"
                    };
                    await _broadcastHandler.SaveSmsLog(sms);
                }
            }
            result.HasError = false;
            result.Messages.Add("Sms Process completed");
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SmsToAllMember(CustomSmsReq model)
        {
            var result = new List<string>();
            var data = await Mediator.Send(new GetMemberPhoneListQuery());
            foreach (var item in data.DataList)
            {
                if (await _broadcastHandler.SendSms(item.PhoneNo, model.Message, model.LanType))
                {
                    result.Add("Success :  " + item.MemberShipNo + " Phone No : " + item.PhoneNo);
                }
                else
                {
                    result.Add("Fail : " + item.MemberShipNo + " Phone No : " + item.PhoneNo);
                }
            }
            return Ok(result);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendEmail(CustomSmsReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await _broadcastHandler.SendEmail(model.EmailId, model.EmailSubject, model.Message);
            SmsLog sms = new SmsLog()
            {
                PhoneNo = model.PhoneNo,
                Message = model.Message,
                SmsDate = DateTime.Now,
                MemberName = "",
                MemberShipNo = "",
                Status = result ? "Ok" : "No"
            };
            await _broadcastHandler.SaveSmsLog(sms);

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public  IActionResult ExcelFileProcess([FromForm] EmailModel model)
        {
            if (model.EmailList == null || !model.EmailList.Any())
                return BadRequest("No emails provided.");
           
             _broadcastHandler.SendEmailByAttachment(model);
           // _backgroundJobClient.Enqueue<IBroadcastHandler>(e => e.);
            var result = new Result();
            result.HasError = false;
            result.Messages.Add("Email sendings...");
            return Ok(result);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendBulkEmail(BulkSmsReq model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = new Result();
            foreach (var item in model.SmsReqList)
            {
                await _broadcastHandler.SendEmail(item.EmailId, model.EmailSubject, model.Message);
            }
            result.HasError = false;
            result.Messages.Add("Email Process completed");
            return Ok(result);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EmailToAllMember(CustomSmsReq model)
        {
            var result = new List<string>();
            var data = await Mediator.Send(new GetMemberPhoneListQuery());
            foreach (var item in data.DataList)
            {
                if (await _broadcastHandler.SendEmail(item.MemberEmail, model.EmailSubject, model.Message))
                {
                    result.Add("Success :  " + item.MemberShipNo + " Email: " + item.MemberEmail);
                }
                else
                {
                    result.Add("Fail : " + item.MemberShipNo + " Email: " + item.MemberEmail);
                }
            }
            return Ok(result);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GetSmsLog(SmsLogSearchReq model)
        {
            var result = await Mediator.Send(new GetAllSmsLogQuery() { Model = model });
            return Ok(result);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ReleaseVersionSave(ReleaseVersionReq model)
        {
            if (model == null) return BadRequest();

            var result = await Mediator.Send(new CreateReleaseVersionCommand() { Model = model });
            return Ok(result);
        }
        // [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetReleaseVersion(string appId, string releaseType)
        {
            var result = await Mediator.Send(new GetReleaseVersionQuery() { AppId = appId, ReleaseType = releaseType });

            return Ok(result);
        }
    }
}
