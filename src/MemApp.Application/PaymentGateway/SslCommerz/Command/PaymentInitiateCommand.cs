using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models.DTOs;
using MemApp.Application.PaymentGateway.SslCommerz.Model;
using MemApp.Domain.Entities.com;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ResApp.Application.ROA.SubscriptionPayment.Models;
using ResApp.Application.Com.Commands.RoaSubscriptionVerify;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.PaymentGateway.SslCommerz.Command
{
    public class PaymentInitiateCommand : IRequest<SSLCommerzInitResponse>
    {
        public SSLCommerzInitRequest Model { get; set; } = new SSLCommerzInitRequest();
    }
    public class PaymentInitiateCommandHandler : IRequestHandler<PaymentInitiateCommand, SSLCommerzInitResponse>
    {
        private readonly IMemDbContext _context;
        private readonly SslCommerzPayment _sslCommerzPayment;
        private readonly IPermissionHandler _permissionHandler;
        public PaymentInitiateCommandHandler(IMemDbContext context, IOptions<SslCommerzPayment> sslCommerzPayment, IPermissionHandler permissionHandler)
        {
            _context = context;
            _sslCommerzPayment = sslCommerzPayment.Value;
            _permissionHandler = permissionHandler;
        }

        public async Task<SSLCommerzInitResponse> Handle(PaymentInitiateCommand request, CancellationToken cancellation)
        {
            try
            {
                var result = new SSLCommerzInitResponse();
                if (await _permissionHandler.IsTempMember())
                {
                  //  var result = new SSLCommerzInitResponse();
                    result.HasError = true;
                    result.Messages?.Add("You have no permission to access, please contact with authority.");
                    return result;
                }

                string cardNo = "";
                string memberShipNo = "";

                request.Model.currency = "BDT";
                request.Model.cus_country = "Bangladesh";
                request.Model.cus_city = "Dhaka";

                //var memberObj = await _context.RegisterMembers
                //    .Select(s => new { s.Id, s.Phone, s.Email, s.MembershipNo, s.IsMasterMember, s.CardNo, s.FullName, s.HomeAddress })
                //   .FirstOrDefaultAsync(q => q.Id == request.Model.MemberId, cancellation);

                //if (memberObj != null ? memberObj.IsMasterMember == true : false)
                //{
                //    request.Model.MemberId = memberObj.Id;
                //    cardNo = memberObj.CardNo ?? "";
                //    memberShipNo = memberObj.MembershipNo ??"";
                //    request.Model.cus_name = memberObj.FullName;
                //    request.Model.cus_phone = memberObj.Phone ?? "";
                //    request.Model.cus_email = memberObj.Email??"";
                //    request.Model.cus_add1 = memberObj.HomeAddress?? "";
                //}
                //else
                //{
                //    var smemberObj = await _context.RegisterMembers
                //          .Select(s => new { s.Id, s.Phone, s.Email, s.MembershipNo, s.IsMasterMember, s.MemberId, s.CardNo, s.FullName, s.HomeAddress })
                //     .FirstOrDefaultAsync(q => q.Id == request.Model.MemberId, cancellation);
                //    if (smemberObj != null)
                //    {
                //        request.Model.MemberId = smemberObj.MemberId ?? 0;
                //        cardNo = smemberObj.CardNo ?? "";
                //        memberShipNo = smemberObj.MembershipNo ?? "";
                //        request.Model.cus_name = smemberObj.FullName;
                //        request.Model.cus_phone = smemberObj.Phone ?? "";
                //        request.Model.cus_email = smemberObj.Email ?? "";
                //        request.Model.cus_add1 = smemberObj.HomeAddress ?? "";
                //    }
                //}
              //  var result = new SSLCommerzInitResponse();
                var memberObj = await _context.MemberRegistrationInfos
                    .Select(s => new { s.Id, s.PhoneNo, s.Email, s.MemberShipNo, s.MembershipFee, s.Name, s.PermanentAddress })
                   .FirstOrDefaultAsync(q => q.Id == request.Model.MemberId, cancellation);

                if (memberObj == null)
                {
                    //var result = new SSLCommerzInitResponse();
                    result.HasError = true;
                    result.Messages?.Add("Member Not Found");
                    return result;
                }

                if (string.IsNullOrEmpty(request.Model.PaymentFor!.Trim()))
                {
                    result.HasError = true;
                    result.Messages?.Add("Please specify the Fee");
                    return result;
                }

                

                if (memberObj != null)
                {
                    request.Model.MemberId = memberObj.Id ;
                   // cardNo = memberObj.CardNo ?? "";
                    memberShipNo = memberObj.MemberShipNo ?? "";
                    request.Model.cus_name = memberObj.Name;
                    request.Model.cus_phone = memberObj.PhoneNo ?? "";
                    request.Model.cus_email = memberObj.Email ?? "";
                    request.Model.cus_add1 = memberObj.PermanentAddress ?? "";
                }

                var obj = new TopUp
                {
                    OnlineTopUp = true,
                    Status = "Pending",
                    IsActive = false,
                    MemberShipNo = memberShipNo,
                    CardNo = cardNo,
                    // obj.RegisterMemberId = request.Model.MemberId;
                    MemberId = request.Model.MemberId,
                    TopUpDate = DateTime.Now
                };
                _context.TopUps.Add(obj);

                    obj.TotalAmount = request.Model.total_amount;
                    obj.Note = GenerateUniqueId() +request.Model.MemberId;
                    obj.PaymentMode = "Online";
                    // obj.MonthDetails = months;
                  

                if (request.Model.PaymentFor!.Trim() == "Subscription Fee")
                {
                    var exist = _context.ROAMembershipFeePayments.Any(x => x.MemberId == request.Model.MemberId);
                    if (!exist)
                    {
                        result.HasError = true;
                        result.Messages?.Add("Please pay the membership fee first!!");
                        return result;
                    }
                    var verify = new SubscriptionDetailsVerify(_context);
                    if (!verify.MonthVerify(request.Model.MemberId, request.Model.SubscriptionDetails!))
                    {
                        result.HasError = true;
                        result.Messages?.Add("This month fee has already paid!!");
                        return result;
                    }
                    var checkedList = request.Model.SubscriptionDetails!.Where(q => q.IsChecked).OrderBy(o => o.SubscriptionMonth).ToList();

                    if (checkedList.Sum(x => x.PaymentAmount + x.LateAmount) != request.Model.total_amount)
                    {
                        result.HasError = true;
                        result.Messages?.Add("Wrong amount!");
                        return result;
                    }

                    string months = string.Join(", ", checkedList.Select(p => p.SubscriptionMonth.ToString("yyyy-MM-dd")));

                    var monthDetails = checkedList.Select(x => new PaymentTracking
                    {
                        SubscriptionMonth = x.SubscriptionMonth,
                        PaymentAmount = x.PaymentAmount,
                        LateAmount = x.LateAmount,
                    }).ToList();
                    obj.MonthDetails = JsonConvert.SerializeObject(monthDetails); // Convert to JSON
                    obj.PaymentFor = "Subscription Fee";
                }

                else if (request.Model.PaymentFor!.Trim() == "Membership Fee")
                {
                    var exist = _context.ROAMembershipFeePayments.Any(x => x.MemberId == request.Model.MemberId);
                    if (exist)
                    {
                        result.HasError = true;
                        result.Messages?.Add("Member Fee has been paid already!!");
                        return result;
                    }
                    if(memberObj!.MembershipFee != request.Model.total_amount)
                    {
                        result.HasError = true;
                        result.Messages?.Add("Please set the right amount for Membership Fee!!");
                        return result;
                    }
                    obj.PaymentFor = "Membership Fee";
                }

                else
                {
                    result.HasError = true;
                    result.Messages?.Add("Please specify the Fee");
                    return result;
                }


                if (await _context.SaveChangesAsync(cancellation) > 0)
                {
                    List<TopUpDetail> details = new List<TopUpDetail>();
                    var td = new TopUpDetail()
                    {
                        PaymentMethodId = 1,
                        PaymentMethodText = "SSL Online",
                        Amount = request.Model.total_amount,
                        TrxCardNo = "",
                        MachineNo = "",
                        TrxNo = "",
                        IsActive = false,
                        TopUpId = obj.Id,
                        BankId = 0,
                        BankText = "",
                        CreditCardId = 0,
                        CreditCardText = ""
                    };
                    details.Add(td);

                    _context.TopUpDetails.AddRange(details);
                }
                await _context.SaveChangesAsync(cancellation);
                
                NameValueCollection PostData = new NameValueCollection();
               
                foreach (var item in GetProperties(request.Model))
                {
                    PostData.Add(item.Key, item.Value);
                }
                
                var response = InitiateTransaction(PostData, obj.Note);

                if (string.IsNullOrEmpty(response.GatewayPageURL))
                {
                    return (new SSLCommerzInitResponse
                    {
                        status = "fail",
                        storeLogo = response.storeLogo
                    });
                }
                return (new SSLCommerzInitResponse
                {
                    status = "success",
                    sessionkey = response.sessionkey,
                    GatewayPageURL = response.GatewayPageURL,
                    TrxNo= obj.Note,
                    storeLogo = response.storeLogo
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public SSLCommerzInitResponse InitiateTransaction(NameValueCollection PostData, string transId)
        {
            PostData.Add("store_id", _sslCommerzPayment.store_id);
            PostData.Add("store_passwd", _sslCommerzPayment.store_passwd);
            PostData.Add("tran_id", transId);
            PostData.Add("product_category", "TopUp");
            PostData.Add("product_name", "TopUp");
            PostData.Add("product_profile", "TopUp");
            PostData.Add("success_url", _sslCommerzPayment.success_url);
            PostData.Add("fail_url", _sslCommerzPayment.fail_url);
            PostData.Add("cancel_url", _sslCommerzPayment.cancel_url);
            PostData.Add("shipping_method", "SSL");
            PostData.Add("ship_name", "test");
            PostData.Add("ship_add1", "test");
            PostData.Add("ship_city", "test");
            PostData.Add("ship_state", "test");
            PostData.Add("ship_postcode", "test");
            PostData.Add("ship_country", "test");
            PostData.Add("ipn_url", _sslCommerzPayment.ipn_url);
            string response = SendPost(PostData);
            try
            {
                    SSLCommerzInitResponse resp = JsonConvert.DeserializeObject<SSLCommerzInitResponse>(response);
                    if (resp.status == "SUCCESS")
                    {
                        return resp;
                    }
                    else
                    {
                        throw new Exception("Unable to get data from SSLCommerz. Please contact your manager!");
                    }
                              
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
        }
        private string GenerateUniqueId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= (b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks).ToUpper();
        }
        private static Dictionary<string, string> GetProperties(object obj)
        {
            var props = new Dictionary<string, string>();
            if (obj == null)
                return props;

            var type = obj.GetType();
            foreach (var prop in type.GetProperties())
            {
                var val = prop.GetValue(obj, new object[] { });

                if (val != null)
                {
                    props.Add(prop.Name, val.ToString());
                }
            }

            return props;
        }
        protected string SendPost(NameValueCollection PostData)
        {
            string response = Post(_sslCommerzPayment.SSLCz_URL+_sslCommerzPayment.Submit_URL, PostData);
            return response;
        }
        public static string Post(string uri, NameValueCollection PostData)
        {
            byte[] response = null;
            using (WebClient client = new WebClient())
            {
                response = client.UploadValues(uri, PostData);
            }
            return System.Text.Encoding.UTF8.GetString(response);
        }
    }
}
