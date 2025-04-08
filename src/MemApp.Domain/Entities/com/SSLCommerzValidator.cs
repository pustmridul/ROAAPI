using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.mem;

namespace MemApp.Domain.Entities.com
{
    public class SSLCommerzValidator : BaseEntity
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
