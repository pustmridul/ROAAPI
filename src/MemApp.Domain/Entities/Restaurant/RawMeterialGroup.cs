using MemApp.Domain.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace MemApp.Domain.Entities.Restaurant;

public class RawMeterialGroup
{
    [Key]
    public int ID { get; set; } 
    public string? GroupID { get; set; } 
    public string? GroupName { get; set; }
}
