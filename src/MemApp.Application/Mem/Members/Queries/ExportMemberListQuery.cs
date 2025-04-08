using MediatR;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.Members.Models;
using Microsoft.EntityFrameworkCore;

namespace MemApp.Application.Mem.Subscription.Queries
{
    public class ExportMemberListQuery : IRequest<List<ExportMember>>
    {
        public MemberSearchReq Model { get; set; }
        public string WebPath { get; set; }
    }

    public class ExportMemberListQueryHandler : IRequestHandler<ExportMemberListQuery, List<ExportMember>>
    {
        private readonly IMemDbContext _context;
     
        public ExportMemberListQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<List<ExportMember>> Handle(ExportMemberListQuery request, CancellationToken cancellationToken)
        {
            
            string membershipNo = !string.IsNullOrEmpty(request.Model.MemberShipNo) ? request.Model.MemberShipNo.PadLeft(5,'0') :"";

            var result = new List<ExportMember>();
            result = await _context.RegisterMembers
            .Include(i => i.Colleges)
            .Include(i => i.MemberProfessions)
            .Include(i => i.BloodGroup)
            .Include(i => i.MemberStatus)
                  .Include(i => i.MemberTypes)
                  .Where(q => q.IsActive && q.IsMasterMember == true
                  && (!string.IsNullOrEmpty(membershipNo) ? q.MembershipNo.Contains(membershipNo) : true)
                  && (!string.IsNullOrEmpty(request.Model.CadetName) ? q.CadetName.Contains(request.Model.CadetName) : true)
                  && (!string.IsNullOrEmpty(request.Model.FullName) ? q.FullName.Contains(request.Model.FullName) : true)
                  && (request.Model.MemberTypeId > 0 ? (q.MemberTypeId == request.Model.MemberTypeId) : true)
                  && (request.Model.MemberActiveStatusId > 0 ? (q.MemberActiveStatusId == request.Model.MemberActiveStatusId) : true)
                  && (request.Model.CollegeId > 0 ? (q.CollegeId == request.Model.CollegeId) : true)
                  && (request.Model.BloodGroupId > 0 ? (q.BloodGroupId == request.Model.BloodGroupId) : true)
                  && (request.Model.MemberProfessionId > 0 ? (q.MemberProfessionId == request.Model.MemberProfessionId) : true)
                  && (!string.IsNullOrEmpty(request.Model.Phone) ? q.Phone.Contains(request.Model.Phone) : true)
                  && (!string.IsNullOrEmpty(request.Model.Email) ? q.Email.Contains(request.Model.Email) : true)
                  && (!string.IsNullOrEmpty(request.Model.BatchNo) ? q.BatchNo.Contains(request.Model.BatchNo) : true)
                  && (!string.IsNullOrEmpty(request.Model.Organaization) ? q.Organaization.Contains(request.Model.Organaization) : true)
                  && (!string.IsNullOrEmpty(request.Model.Designation) ? q.Designation.Contains(request.Model.Designation) : true)
                  && (!string.IsNullOrEmpty(request.Model.Specialization) ? q.Specialization.Contains(request.Model.Specialization) : true)
                  && (!string.IsNullOrEmpty(request.Model.HscYear) ? q.HscYear.Contains(request.Model.HscYear) : true)
                  && (!string.IsNullOrEmpty(request.Model.CadetNo) ? q.CadetNo.Contains(request.Model.CadetNo) : true)
                  && (!string.IsNullOrEmpty(request.Model.memFullId) ? q.MemberFullId.Contains(request.Model.memFullId) : true)
                  )
                  .OrderBy(s => s.MembershipNo).Select(s => new ExportMember
                  {
                      Id = s.Id,
                      MemberShipNo = s.MembershipNo ?? "",
                      Name = s.FullName,
                      CollegeCode = s.Colleges.Code ?? "",
                      CollegeName = s.Colleges.Name,
                      CadetName = s.CadetName ?? "",
                      TypeText = s.MemberTypes.Name,
                      StatusText = s.MemberStatus.Name,
                      Dbo = s.Dob == null ? "" : s.Dob.Value.ToString(),
                      BloodGroupText = s.BloodGroup.Code,
                      HscYear = s.HscYear ?? "",
                      ProfessionText = s.MemberProfessions.Name,
                      //  ImgFileUrl= ConvertFileToBase64(request.WebPath, s.ImgFileUrl != null ? s.ImgFileUrl.Replace("Members/", "") : ""),
                      Phone = s.Phone ?? "",
                      CardNo = s.CardNo ?? "",
                      Email = s.Email ?? "",
                      MemberFullId = s.MemberFullId,
                      JoinDate = s.JoinDate == null ? "" : s.JoinDate.Value.ToString(),
                      LeaveDate = s.LeaveDate == null ? "" : s.LeaveDate.Value.ToString(),
                      PermanentAddress = s.PermanentAddress ?? "",
                      NID = s.NID ?? "",
                      EmergencyContact = s.EmergencyContact ?? ""
                  }).ToListAsync(cancellationToken);

             

            //if (data.Count == 0)
            //{
              
            //}
            //else
            //{

            //    result = data.Select(s => new ExportMember
            //    {
            //        Id = s.Id,
            //        MemberShipNo = s.MembershipNo ?? "",
            //        Name = s.FullName,
            //        CollegeCode = s.Colleges.Code??"",
            //        CollegeName = s.Colleges.Name,
            //        CadetName = s.CadetName??"",
            //        TypeText = s.MemberTypes.Name,
            //        StatusText = s.MemberStatus.Name,
            //        Dbo = s.Dob ==null ? "": s.Dob.Value.ToString(),
            //        BloodGroupText = s.BloodGroup.Code,
            //        HscYear = s.HscYear ?? "",
            //        ProfessionText = s.MemberProfessions.Name,
            //      //  ImgFileUrl= ConvertFileToBase64(request.WebPath, s.ImgFileUrl != null ? s.ImgFileUrl.Replace("Members/", "") : ""),
            //        Phone = s.Phone ?? "",
            //        CardNo= s.CardNo ?? "",
            //        Email= s.Email ?? "",
            //        MemberFullId= s.MemberFullId,
            //        JoinDate= s.JoinDate == null ? "" : s.JoinDate.Value.ToString(),
            //        LeaveDate= s.LeaveDate == null ? "" : s.LeaveDate.Value.ToString(),
            //        PermanentAddress =s.PermanentAddress ?? "",
            //        NID= s.NID??"",
            //        EmergencyContact=s.EmergencyContact ?? ""
            //    }).ToList();             
            //}
            return result;
        }
        private string ConvertFileToBase64(string roothPath, string fileName)
        {
            if (fileName == "")
            {
                return "";
            }
            if (fileName == null)
            {
                return "";
            }
            
            var files = Directory.GetFiles(Path.Combine(roothPath, "Members")).ToList();

            var file = files.FirstOrDefault(q => q.Contains(fileName));
            if (file == null)
            {
                return "";
            }
            else
            {
                var filebytes = File.ReadAllBytes(file);

                return Convert.ToBase64String(filebytes);
            }

        }
    }
}
