using MediatR;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemberTemps.Models;
using Microsoft.EntityFrameworkCore;
using MemApp.Application.Interfaces;

namespace MemApp.Application.Mem.MemberTemps.Queries
{
    public class GetAllMemberTempQuery : IRequest<MemberTempListVm>
    {
        public int Id { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }


    public class GetAllMemberTempQueryHandler : IRequestHandler<GetAllMemberTempQuery, MemberTempListVm>
    {
        private readonly IMemDbContext _context;
        private readonly IPermissionHandler _permissionHandler;
        public GetAllMemberTempQueryHandler(IMemDbContext context, IPermissionHandler permissionHandler)
        {
            _context = context;
            _permissionHandler = permissionHandler;
        }

        public async Task<MemberTempListVm> Handle(GetAllMemberTempQuery request, CancellationToken cancellationToken)
        {
            var result = new MemberTempListVm();
           
            try
            {             
                var data = await _context.MemberTemps
                    .Include(i => i.Colleges)
                    .Include(i => i.BloodGroup)
                    .Include(i => i.MemberChildrens)
                    .Include(i => i.MemberEducations)
                    .Include(i => i.MemberProfessions)
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

                if (data.TotalCount == 0)
                {
                    result.HasError = true;
                    result.Messages.Add("Data Not Found");
                }
                else
                {
                    result.HasError = false;
                    result.DataCount = data.TotalCount;
                    result.DataList = data.Data.Select(s => new MemberTempRes
                    {
                        Id = s.Id,
                        AffiliateMember = s.AffiliateMember ?? "",
                        AncestralHome = s.AffiliateMember ?? "",
                        Anniversary = s?.Anniversary == null ? "" : s.Anniversary.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        BatchNo = s.BatchNo ?? "",
                        BloodGroupId = s.BloodGroupId ?? 0,
                        CadetName = s.CadetName ?? "",
                        CadetNo = s.CadetNo ?? "",
                        CCCertificate = s.CCCertificate ?? "",
                        ClubName = s.ClubName ?? "",
                        ColorOfEye = s.ColorOfEye ?? "",
                        ColorOfHair = s.ColorOfHair ?? "",
                        DateOfApplication = s?.DateOfApplication == null ? "" : s.DateOfApplication.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                     
                        Designation = s.Designation ?? "",
                        Dob = s?.Dob == null ? "" : s.Dob.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        Email = s.Email ?? "",
                        FatherName = s.FatherName ?? "",
                        FullName = s.FullName ?? "",
                        HeightCms = s.HeightCms ?? "",
                        Hobbies = s.Hobbies ?? "",
                        HomeAddress = s.HomeAddress ?? "",
                        HonorAndAwards = s.HonorAndAwards ?? "",
                        IdentificationMarks = s.IdentificationMarks ?? "",
                        ImgFileUrl = s.ImgFileUrl ?? "",
                        IsActive = s.IsActive,
                        JoiningDate = s?.JoiningDate == null ? "" : s.JoiningDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        LeavingDate = s?.LeavingDate == null ? "" : s.LeavingDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),

                        MemberProfessionId = s.MemberProfessionId ?? 0,
                        MotherName = s.MotherName ?? "",
                        Nationality = s.Nationality ?? "",
                        NID = s.NID ?? "",
                        OfficeAddress = s.OfficeAddress ?? "",
                        Organaization = s.Organaization ?? "",
                        Phone = s.Phone ?? "",
                        PrimaryMember = s.PrimaryMember ?? "",
                        Specialization = s.Specialization ?? "",
                        Spouse = s.Spouse ?? "",
                        SpouseBloodGroupId = s.SpouseBloodGroupId ?? 0,
                        SpouseDob = s?.SpouseDob == null ? "" : s.SpouseDob.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),

                        SpouseOccupation = s.SpouseOccupation ?? "",
                        Status = s.Status ?? "",
                        TIN = s.TIN ?? "",
                        WeightKgs = s.WeightKgs ?? "",
                        MemberEducationReqs = s.MemberEducations.Select(i => new MemberEducationReq {
                            Id = i.Id,
                            Board  = i.Board ??"",
                            Exam = i.Exam ??"",
                            Grade = i.Grade ??"",
                            Institution = i.Institution ?? "",
                            MemberId = i.MemberId,
                            PassingYear = i.PassingYear ?? ""

                        }).ToList(),
                        MemberChildrenReqs = s.MemberChildrens.Select(i => new Members.Models.MemberChildrenReq
                        {
                            Id = i.Id,
                            CadetNo = i.CadetNo ?? "",
                            ContactName = i.ContactName ?? "",
                            Dob = i?.Dob == null ? "" : i.Dob.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            Email = i.Email ?? "",
                            Gender = i.Gender ?? "",
                            Phone = i.Phone ?? "",
                            RegisterMemberId = i.RegisterMemberId
                        }).ToList(),

                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.Messages.Add(ex.ToString());
            }
            
            return result;
        }
    }
}
