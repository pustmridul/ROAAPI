using MemApp.Application.Extensions;
using MemApp.Domain.Entities.Restaurant;

namespace MemApp.Application.Restaurant.Reports.Stock.Model
{
    public class DepartmentListVM : Result
    {
        public List<Department> DataList { get; set; } = new List<Department>();
        public long DataCount { get; set; }
    }
}
