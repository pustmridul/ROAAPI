using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.Models
{
    public class PaginationParams
    {
       public int? PageNo {  get; set; }
       public int? PageSize {  get; set; }
       public string? AppId {  get; set; }     
       public string? SearchText {  get; set; }
           
    }
}
