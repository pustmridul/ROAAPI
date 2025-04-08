using MemApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Com.Models;

public class MenuDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Url { get; set; }
    public int DisplayOrder { get; set; }
    public bool Visible { get; set; }
    public string NavIcon { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int? ParentId { get; set; }
    public bool IsChecked { get; set; }
    public List<MenuDto>? Childs { get; set; }
}
