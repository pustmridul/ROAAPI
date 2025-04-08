using MediatR;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Mem.MemberTemps.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.MemberTemps.Queries
{
    public class GetMemberTempByIdQuery : IRequest<Result<MemberTempReq>>
    {
        public int Id { get; set; }
    }

    public class GetMemberTempByIdQueryHandler : IRequestHandler<GetMemberTempByIdQuery, Result<MemberTempReq>>
    {
        private readonly IMemDbContext _context;
        public GetMemberTempByIdQueryHandler(IMemDbContext context)
        {
            _context = context;
        }

        public async Task<Result<MemberTempReq>> Handle(GetMemberTempByIdQuery request, CancellationToken cancellationToken)
        {
            var result = new Result<MemberTempReq>();
            var data = await _context.MemberTemps
                .Include(i=>i.Colleges)
                .Include(i=>i.BloodGroup)
                .Include(i => i.MemberChildrens)
                .Include(i=>i.MemberEducations)
                .Include(i=>i.MemberProfessions)
                .FirstOrDefaultAsync(q=>q.Id==request.Id, cancellationToken);
            if (data==null)
            {
                result.HasError = true;
                result.Messages.Add("Data Not Found");
            }
            else
            {
                result.HasError = false;

                result.Data = new MemberTempReq
                {
                    Id = data.Id,
                    AffiliateMember = data.AffiliateMember ?? "",
                    AncestralHome = data.AffiliateMember ?? "",
                    Anniversary = data?.Anniversary == null ? "" : data.Anniversary.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    BatchNo = data.BatchNo ?? "",
                    BloodGroupId = data.BloodGroupId ?? 0,
                    CadetName = data.CadetName ?? "",
                    CadetNo = data.CadetNo ?? "",
                    CCCertificate = data.CCCertificate ?? "",
                    ClubName = data.ClubName ?? "",
                    ColorOfEye = data.ColorOfEye ?? "",
                    ColorOfHair = data.ColorOfHair ?? "",
                    DateOfApplication = data?.DateOfApplication == null ? "" : data.DateOfApplication.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    Designation = data.Designation ?? "",
                    Dob = data?.Dob == null ? "" : data.Dob.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                  
                    Email = data.Email ?? "",
                    FatherName = data.FatherName ?? "",
                    FullName = data.FullName ?? "",
                    HeightCms = data.HeightCms ?? "",
                    Hobbies = data.Hobbies ?? "",
                    HomeAddress = data.HomeAddress ?? "",
                    HonorAndAwards = data.HonorAndAwards ?? "",
                    IdentificationMarks = data.IdentificationMarks ?? "",
                    ImgFileUrl = data.ImgFileUrl ?? "",
                    IsActive = data.IsActive,
                    JoiningDate = data?.JoiningDate == null ? "" : data.JoiningDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    LeavingDate = data?.LeavingDate == null ? "" : data.LeavingDate.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    MemberProfessionId = data.MemberProfessionId ?? 0,
                    MotherName = data.MotherName ?? "",
                    Nationality = data.Nationality ?? "",
                    NID = data.NID ?? "",
                    OfficeAddress = data.OfficeAddress ?? "",
                    Organaization = data.Organaization ?? "",
                    Phone = data.Phone ?? "",
                    PrimaryMember = data.PrimaryMember ?? "",
                    Specialization = data.Specialization ?? "",
                    Spouse = data.Spouse ?? "",
                    SpouseBloodGroupId = data.SpouseBloodGroupId ?? 0,
                    SpouseDob = data?.SpouseDob == null ? "" : data.SpouseDob.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    SpouseOccupation = data.SpouseOccupation ?? "",
                    Status = data.Status ?? "",
                    TIN = data.TIN ?? "",
                    WeightKgs = data.WeightKgs ?? "",
                    MemberEducationReqs = data.MemberEducations.Select(i => new MemberEducationReq
                    {
                        Id = i.Id,
                        Board = i.Board ?? "",
                        Exam = i.Exam ?? "",
                        Grade = i.Grade ?? "",
                        Institution = i.Institution ?? "",
                        MemberId = i.MemberId,
                        PassingYear = i.PassingYear ?? ""

                    }).ToList(),
                    MemberChildrenReqs = data.MemberChildrens.Select(i=> new Members.Models.MemberChildrenReq 
                    {
                        Id = i.Id,
                        CadetNo = i.CadetNo ??"",
                        ContactName = i.ContactName ?? "",
                        Dob = i?.Dob == null ? "" : i.Dob.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),                  
                        Email = i.Email ?? "",
                        Gender = i.Gender ?? "",
                        Phone = i.Phone ?? "",
                        RegisterMemberId = i.RegisterMemberId 
                    }).ToList(),
                    //MemberChildrenReqs = (List<Members.Models.MemberChildrenReq>)data.MemberChildrens


                };
            }

            return result;
        }
    }
}
