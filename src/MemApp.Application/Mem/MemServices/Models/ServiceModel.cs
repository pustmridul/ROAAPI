using MemApp.Application.Extensions;
using MemApp.Application.Models;

namespace MemApp.Application.Mem.Service.Model
{
    public class ServiceModel
    {
    }

    #region ServiceType
    public class ServiceTypeReq
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string DisplayName { get; set; }=string.Empty;

    }
    public class ServiceTypeRes
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public Boolean Visible { get; set; }
    }

    public class ServiceTypeVm:Result
    {
        public ServiceTypeRes Data { get; set; } = new ServiceTypeRes();
    }
    public class ServiceTypeListVm:Result
    {
        public int DataCount { get; set; }
        public List<ServiceTypeRes> DataList { get; set; } = new List<ServiceTypeRes>();
    }
    #endregion

    public class SubscriptionModDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Value { get; set; }
    }
    

    #region SubscriptionMode

    #endregion


    #region AvailabilityVm

    public class AvailabilityRes 
    {
        public int Id { get; set; }
      
        public string Name { get; set; } = string.Empty;
        public bool IsLifeTime { get; set; }
        public string AvailabilityDate { get; set; } = string.Empty;
        public string? AvailabilityEndDate { get; set; }

        public List<AvailabilityDetailVm> AvailabilityDetailVms { get; set; } = new List<AvailabilityDetailVm>();
    }

    public class AvailabilityVm : Result
    {
      public AvailabilityRes Data { get; set; } =new AvailabilityRes();
    }

    public class AvailabilityDetailVm
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int AvailabilityId { get; set; }
        
        public int ServiceAvailabilityDetailsId { get; set; }
        public bool IsChecked { get; set; }
        public bool IsWholeDay { get; set; }
        public string? Status { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string ReservationFrom { get; set; } = string.Empty;
        public string ReservationTo { get; set; } = string.Empty;

    }
    public class AvailabilityListVm : Result
    {
        public int DataCount { get; set; }
        public List<AvailabilityRes> DataList { get; set; } = new List<AvailabilityRes>();
    }

    #endregion
}
