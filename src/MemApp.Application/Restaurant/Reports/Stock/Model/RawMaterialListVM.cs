using MemApp.Application.Extensions;
using MemApp.Domain.Entities.Restaurant;

namespace MemApp.Application.Restaurant.Reports.Stock.Model
{
    public class RawMaterialListVM : Result
    {
        public List<RawMeterial> DataList { get; set; } = new List<RawMeterial>();
        public long DataCount { get; set; }
    }
}
