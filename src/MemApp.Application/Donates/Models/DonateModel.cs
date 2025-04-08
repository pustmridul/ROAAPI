using MemApp.Application.Mem.TopUps.Models;
using MemApp.Domain.Entities.com;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Donates.Models;

public class DonateDto
{
    public int Id { get; set; } 
    public int? MemberId { get; set; }
    public string? MemberShipNo { get; set; }
    public string? MemberName { get; set; }
    public string? DonationText { get; set; }
    public int DonationId { get; set; }
    public decimal Amount { get; set; }
    public string DonateNo { get; set; }
    public string? Note { get; set; }
    public DateTime? DonateDate { get; set; }
    public TopUpReq? topup { get; set; }
    public string? PhoneNo { get; set; }
    public string? SmsText { get; set; }

}

public class PageParams
{
    public int PageSize { get; set; }
    public int PageNo { get; set; }

    public string? SearchText { get; set; }
}

