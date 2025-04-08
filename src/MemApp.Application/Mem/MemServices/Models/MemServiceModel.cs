using MemApp.Application.Extensions;
using MemApp.Application.Mem.AreaLayouts.Models;
using MemApp.Application.Mem.Service.Model;
using MemApp.Application.Models;
using MemApp.Domain.Entities.ser;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace MemApp.Application.Mem.MemServices.Models
{
    public class MemServiceModel
    {
    }
    #region MemService
    public class MemServiceReq
    {
        public int Id { get; set; }
        public int ServiceTypeId { get; set; }
        public string DisplayTitle { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
       
        public bool IsActive { get; set; }
    }
    public class MemServiceDto
    {
        public string? ImgFileUrl { get; set; }
        public int Id { get; set; }
        public int ServiceTypeId { get; set; }
        public string ServiceTypeTitle { get; set; }

        public string Title { get; set; }
        public string DisplayTitle { get; set; }

        public string Code { get; set; }
        public bool IsActive { get; set; }
        public int ServiceTicketCount { get; set; }
    }
    

   

    public class VenueListWithAvailableListVm : Result
    {
        public List<VenueListWithAvailable> DataList { get; set; } = new List<VenueListWithAvailable>();
    }
    public class VenueListWithAvailable
    {
        //public DateTime SelectedDate { get; set; }
        //public string? SelectedDateText { get; set; }
        public DateTime? BlockedDate { get; set; }
        public string? VenueTitle { get; set; }
        public int VenueId { get; set; }
        public bool IsChecked { get; set; }
        public List<VenueAvailableDetail> VenueAvailableDetails { get; set; }= new List<VenueAvailableDetail>();


    }
    public class VenueDto
    {
        public string? VenueTitle { get; set; }
        public int MemserviceId { get; set; }
        public string? AvailabilityTitle { get; set; }
        public int AvailabilityId { get; set; }
        public bool IsChecked { get; set; }

    }

    public class VenueBlockedDto
    {
        public int AvailabilityId { get; set; }
        public int VenueId { get; set; }
        public string? VenueTitle { get; set; }
        public DateTime? BlockedDate { get; set; }

    }
    public class VenueAvailableDetail
    {
        public int VenueId { get; set; }
        public bool IsChecked { get; set; }
        public int AvailableId { get; set; }
        public string? AvailableText { get; set; }
    }


    #endregion

    #region ServiceTicket
    public class ServiceTicketReq
    {
        public int Id { get; set; }
        public int MemServiceId { get; set; }
        public string? MemServiceText { get; set; }
        public int MemServiceTypeId { get; set; }
        public string? MemberServiceTypeText { get; set; } 
        public string? ImgFileUrl { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? EventDate { get; set; }

        public string? MemServiceTitle { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public decimal? ServiceChargePercent { get; set; }
        public decimal? ServiceChargeAmount { get; set; }
        public decimal? VatChargePercent { get; set; }
        public decimal? VatChargeAmount { get; set; }
        public string? PromoCode { get; set; }
        public bool IsActive { get; set; }
        public bool HasTicket { get; set; }
        public bool HasAvailability { get; set; }
        public bool HasAreaLayout { get; set; }
        public int? AvailabilityId { get; set; }
        public string AvailabilityName { get; set; }= string.Empty;
        public bool AvailabilityIsLifeTime { get; set; }
        public int TicketLimit { get; set; }
        public bool Status { get; set; }
        public IFormFile? formFile { get; set; }   
        
        public List<AvailabilityDetailVm>? AvailabilityDetailList { get; set; } = new List<AvailabilityDetailVm>();
        public List<EventTokenReq>? EventTokenReqs { get; set; } = new List<EventTokenReq>();
        public List<ServiceTicketDetailReq>? ServiceTicketDetailReqs { get; set; } = new List<ServiceTicketDetailReq>();
        public List<SerTicketAreaLayoutReq>? SerTicketAreaLayoutReqs { get; set; }= new List<SerTicketAreaLayoutReq>();
        public List<ServiceTicketAvailabilityReq>? ServiceTicketAvailabilityReqs { get; set; } = new List<ServiceTicketAvailabilityReq>();

    }



    public class SerTicketAreaLayoutReq
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int AreaLayoutId { get; set; }
        public List<SerTicketAreaLayoutMatrixReq> AreaLayoutDetails { get; set; } = new List<SerTicketAreaLayoutMatrixReq>();

    }
    public class ServiceTicketAvailabilityReq
    {
        public int Id { get; set; }
        public int ServiceTicketId { get; set; }
        public string DayText { get; set; } = string.Empty;
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public bool IsWholeDay { get; set; }
        public int Qty { get; set; }
        public int SlotId { get; set; }
        public bool IsChecked { get; set; }


    }
    public class SerTicketAreaLayoutMatrixReq
    {
        public int Id { get; set; }
        public int AreaLayoutId { get; set; } 
        public int TableId { get; set; }
        public string TableName { get; set; } = string.Empty;
        public int NumberOfChair { get; set; }
    }
    public class EventTokenReq
    {
        public int Id { get; set; }
        public int ServiceTicketId { get; set; }
        public string? TokenTitle { get; set; }
        public string TokenCode { get; set; } = string.Empty;

    }
    public class ServiceAvailabilityRes
    {
        public int Id { get; set; }
        public DateTime AvailabiltyDate { get; set; }
        public string Morning { get; set; } = string.Empty;
        public string Afternoon { get; set; }= string.Empty;
        public string Evening { get; set; }=string.Empty;
        public string? WholeDay { get; set; }
    }
    public class ServiceTicketDetailReq
    {
        public int Id { get; set; }
        public int ServiceTicketId { get; set; }
        public int? ServiceTicketTypeId { get; set; }
        public string TicketType { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int MaxQuantity { get; set; }
        public int MinQuantity { get; set; }
    }

    public class ServiceTicketDetailRes
    {
        public int Id { get; set; }
        public int ServiceTicketId { get; set; }
        public int? ServiceTicketTypeId { get; set; }
        public string TicketType { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int MaxQuantity { get; set; }
    }
    public class ServiceTicketVm : Result
    {
        public ServiceTicketReq Data { get; set; } = new ServiceTicketReq();
    }
 
    public class ServiceTicketListVm : Result
    {
        public long DataCount { get; set; }
        public List<ServiceTicketReq> DataList { get; set; } = new List<ServiceTicketReq>();
    }
    public class TicketSearchReq
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? SearchText { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set;}
        public int? ServiceTypeId { get; set; }
        public int? ServiceId { get; set; }

    }
    public class ServiceTicketDetailVm : Result
    {
        public ServiceTicketDetailRes Data { get; set; } = new ServiceTicketDetailRes();
    }
    public class ServiceTicketDetailListVm : Result
    {
        public long DataCount { get; set; }
        public List<ServiceTicketDetailRes> DataList { get; set; } = new List<ServiceTicketDetailRes>();
    }
    public class ServiceAvailabilityVm : Result
    {
        public ServiceAvailabilityRes Data { get; set; } = new ServiceAvailabilityRes();
    }
    public class ServiceAvailabilityListVm : Result
    {
        public long DataCount { get; set; }
        public List<ServiceAvailabilityRes> DataList { get; set; } = new List<ServiceAvailabilityRes>();
    }
    #endregion



    #region ServiceTicketType
    public class ServiceTicketTypeReq
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? ServiceType { get; set; }
    }
    public class ServiceTicketTypeRes
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? ServiceType { get; set; }
    }
    public class ServiceTicketTypeVm:Result
    {
        public ServiceTicketTypeRes Data { get; set; } = new ServiceTicketTypeRes();
    }
    public class ServiceTicketTypeListVm : Result
    {
        public int DataCount { get; set; }
        public List<ServiceTicketTypeRes> DataList { get; set; } = new List<ServiceTicketTypeRes>();
    }


    public class VenueReq
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string Title { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string? ImgFileUrl { get; set; }
        public decimal VatPercent { get; set; }
        public decimal ServicePercent { get; set; }
        public List<VenueAvailability> VenueAvailabilities { get; set; } = new List<VenueAvailability>();
        public List<BookingCriteria> BookingCriterias { get; set; } = new List<BookingCriteria>();

    }
    public class VenueAvailability
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string? Status { get; set; }
    }

    public class BookingCriteria
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public int MaxQuantity { get; set; }
        public int MinQuantity { get; set; }

        public int SaleQty { get; set; }
    }

    public class  VenueVm: Result
    {
        public List<VenueReq> Data { get; set; } = new List<VenueReq>(); 
    }
    public class VenueListVm : Result
    {
        public List<VenueReq> DataList { get; set; } = new List<VenueReq>();
        public long DataCount { get; set; }
    }

    public class EventReq
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
        public string Title { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string? ImgFileUrl { get; set; }
        public int TicketLimit { get; set; }
        public DateTime? StartDate { get; set; } 
        public DateTime? EndDate { get; set; }
        public DateTime? EventDate { get; set; }
        public string? PromoCode { get; set; }

        public decimal? ServicePercent { get; set; }
        public decimal? VatPercent { get; set; }


        public List<EventTokenReq> EventTokenReqs { get; set; } = new List<EventTokenReq>();
        public List<BookingCriteria> BookingCriterias { get; set; } = new List<BookingCriteria>();
        public List<AreaLayoutTableDetail> layoutTableDetails { get; set; } = new List<AreaLayoutTableDetail>();

    }
  
    public class EventVm : Result
    {
        public List<EventReq> Data { get; set; } = new List<EventReq>();
    }
    public class EventListVm : Result
    {
        public List<EventReq> DataList { get; set; } = new List<EventReq>();
        public long DataCount { get; set; }
    }
    #endregion
}
