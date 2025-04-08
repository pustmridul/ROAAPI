using MemApp.Domain.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace MemApp.Domain.Entities.Restaurant;

public class Department
{
    [Key]
    public int ID { get; set; } 
    public string? DepartmentName { get; set; } 
    public string? CreatedBy { get; set; } 
    public string? UpdatedBy { get; set; } 
    public DateTime CreatedDate { get; set; } 
    public DateTime UpdatedDate { get; set; } 
    public string? PrinterName { get; set; }
  
}

