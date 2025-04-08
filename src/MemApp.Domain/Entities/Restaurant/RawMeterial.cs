using MemApp.Domain.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace MemApp.Domain.Entities.Restaurant;

public class RawMeterial
{
    [Key]
    public int ID { get; set; } 
    public string? Name { get; set; } 
    public int GroupID { get; set; } 
    public int UnitTypeID { get; set; } 
    public decimal CPU { get; set; } 
    public decimal ROL { get; set; }
    public decimal ReceipeCPU { get; set; } 
    public int ReceipeUnitTypeId { get; set; }
    public decimal ConversionRate { get; set; }
    public bool IsNonInventoryItem { get; set; }
    public bool IsNonExpireItem { get; set; } 
    public bool IsNonReceipyItem { get; set; }
    public DateTime CreatedDate { get; set; } 
    public string? CreateBy { get; set; } 
    public DateTime? UpdateDate { get; set; } 
    public string? UpdateBy { get; set; } 
}

