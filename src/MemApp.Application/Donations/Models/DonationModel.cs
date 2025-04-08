using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Donations.Models;

public class DonationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsFixed { get; set; }
    public decimal? Amount { get; set; }
    public string? FileUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public IFormFile? DonationFile {  get; set; } 
}
