using MemApp.Application.Extensions;
using MemApp.Application.Models;
using Microsoft.AspNetCore.Http;
using ResApp.Application.Com.Commands.ROAPayment.Models;
using System.Text.Json.Serialization;

namespace MemApp.Application.PaymentGateway.SslCommerz.Model
{
    public class SslCommerzModel
    {
     
    }
    public class OrderValidationReq
    {
        public string tran_id { get; set; } = string.Empty;
        public string ammount { get; set; } = string.Empty;
        public string currency { get; set; } = string.Empty;
        public HttpRequest request { get; set; } 
    }
   
    public class SSLCommerzValidatorResponse
    {
        public string? status { get; set; }
        public string tran_date { get; set; } = string.Empty;
        public string? tran_id { get; set; }
        public string? val_id { get; set; }
        public string?   amount { get; set; }
        public string? store_amount { get; set; }
        public string? currency { get; set; }
        public string? bank_tran_id { get; set; }
        public string? card_type { get; set; }
        public string? card_no { get; set; }
        public string? store_id { get; set; }
        public string? verify_sign { get; set; }
        public string? verify_key { get; set; }
        public string? verify_sign_sha2 { get; set; }
        public string? card_issuer { get; set; }
        public string? card_brand { get; set; }
        public string? card_issuer_country { get; set; }
        public string? card_issuer_country_code { get; set; }
        public string? currency_type { get; set; }
        public string? currency_amount { get; set; }
        public string? currency_rate { get; set; }
        public string? base_fair { get; set; }
        public string? value_a { get; set; }
        public string? value_b { get; set; }
        public string? value_c { get; set; }
        public string? value_d { get; set; }
        public string? emi_instalment { get; set; }
        public string? emi_amount { get; set; }
        public string? emi_description { get; set; }
        public string? emi_issuer { get; set; }
        public string? risk_title { get; set; }
        public string? risk_level { get; set; }
        public string? APIConnect { get; set; }
        public string? validated_on { get; set; }
        public string? gw_version { get; set; }
        public string? discount_amount { get; set; }
        public string? discount_percentage { get; set; }
        public string? discount_remarks { get; set; }
    }

    public class Gw
    {
        public string visa { get; set; } = string.Empty;
        public string master { get; set; } = string.Empty;
        public string amex { get; set; } = string.Empty;
        public string othercards { get; set; } = string.Empty;
        public string internetbanking { get; set; } = string.Empty;
        public string mobilebanking { get; set; } = string.Empty;
    }

    public class Desc
    {
        public string name { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public string logo { get; set; } = string.Empty;
        public string gw { get; set; } = string.Empty;
        public string redirectGatewayURL { get; set; } = string.Empty;
    }

    public class SSLCommerzInitRequest
    {
        public decimal total_amount { get; set; }
        public string? currency { get; set; }
        public string? cus_name { get; set; }
        public string? cus_email { get; set; }
        public string? cus_add1 { get; set; }
        public string? cus_city { get; set; }
        public string? cus_postcode { get; set; }
        public string? cus_country { get; set; }
        public string? cus_phone { get; set; }
        public int MemberId { get; set; }

        public List<MemberPaymentReqSSl>? SubscriptionDetails { get; set; }
    }
    public class SSLCommerzInitResponse : Result
    {
        public string status { get; set; } = string.Empty;
        public string failedreason { get; set; } = string.Empty;
        public string sessionkey { get; set; } = string.Empty;
        public Gw gw { get; set; }
        public string GatewayPageURL { get; set; } = string.Empty;
        public string storeBanner { get; set; } = string.Empty;
        public string storeLogo { get; set; } = string.Empty;
        public List<Desc> desc { get; set; } = new List<Desc>();
        public string TrxNo { get; set; } = string.Empty;
    }
    public class SSLCommerzValidatorResponseReq
    {
        public string? APIConnect { get; set; }
        public string? status { get; set; }
        public string? sessionkey { get; set; }
        public DateTime? tran_date { get; set; }
        public string? tran_id { get; set; }
        public string? val_id { get; set; }
        public decimal? amount { get; set; }
        public decimal? store_amount { get; set; }
        public string? card_type { get; set; }
        public string? card_no { get; set; }
        public string? currency { get; set; }
        public string? bank_tran_id { get; set; }
        public string? card_issuer { get; set; }
        public string? card_brand { get; set; }
        public string? card_issuer_country { get; set; }
        public string? card_issuer_country_code { get; set; }
        public string? currency_type { get; set; }
        public decimal? currency_amount { get; set; }
        public int? emi_instalment { get; set; }
        public decimal? emi_amount { get; set; }
        public decimal? discount_percentage { get; set; }
        public string? discount_remarks { get; set; }
        public string? value_a { get; set; }
        public string? value_b { get; set; }
        public string? value_c { get; set; }
        public string? value_d { get; set; }
        public int? risk_level { get; set; }
        public string? risk_title { get; set; }

    }

}
