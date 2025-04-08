using Dapper;
using MediatR;
using MemApp.Application.Mem.Members.Models;
using MemApp.Application.Services;
using System.Text;

namespace MemApp.Application.Mem.Subscription.Queries
{
    public class SelectedMemberViewQuery : IRequest<List<ViewMemberDto>>
    {
        public MemberSearchReq Model { get; set; }
    }

    public class SelectedMemberViewQueryHandler : IRequestHandler<SelectedMemberViewQuery, List<ViewMemberDto>>
    {
        private readonly IDapperContext _context;

        public SelectedMemberViewQueryHandler(IDapperContext context)
        {
            _context = context;
        }

        public async Task<List<ViewMemberDto>> Handle(SelectedMemberViewQuery request, CancellationToken cancellationToken)
        {
            var result = new List<ViewMemberDto>();

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("SELECT " + request.Model.queryString + " FROM [dbo].[View_Member]  ");

                    if (request.Model != null)
                    {
                        sb.AppendLine("Where IsMasterMember=1");
                        if (request.Model.MemberShipNo is not null && request.Model.MemberShipNo != "")
                        {
                            string membershipNo = !string.IsNullOrEmpty(request.Model.MemberShipNo) ? request.Model.MemberShipNo.PadLeft(5, '0') : "";
                            sb.AppendLine("AND MembershipNo =" + "'" + membershipNo + "'");
                        }

                        if (request.Model.CadetName is not null && request.Model.CadetName != "")
                        {
                            sb.AppendLine("AND CadetName=" + "'" + request.Model.CadetName + "'");
                        }
                        if (request.Model.FullName is not null && request.Model.FullName != "")
                        {
                            sb.AppendLine("AND FullName =" + "'" + request.Model.FullName + "'");
                        }
                        if (request.Model.MemberTypeId is not null && request.Model.MemberTypeId != 0)
                        {
                            sb.AppendLine("AND MemberTypeId=" + request.Model.MemberTypeId);
                        }
                        if (request.Model.MemberActiveStatusId is not null && request.Model.MemberActiveStatusId != 0)
                        {
                            sb.AppendLine("AND MemberActiveStatusId=" + request.Model.MemberActiveStatusId);
                        }
                        if (request.Model.CollegeId is not null && request.Model.CollegeId != 0)
                        {
                            sb.AppendLine("AND CollegeId=" + request.Model.CollegeId);

                        }
                        if (request.Model.BloodGroupId is not null && request.Model.BloodGroupId != 0)
                        {
                            sb.AppendLine("AND BloodGroupId=" + request.Model.BloodGroupId);
                        }
                        if (request.Model.MemberProfessionId is not null && request.Model.MemberProfessionId != 0)
                        {
                            sb.AppendLine("AND MemberProfessionId=" + request.Model.MemberProfessionId);
                        }
                        if (request.Model.Phone is not null && request.Model.Phone != "")
                        {
                            sb.AppendLine("AND Phone=" + "'" + request.Model.Phone + "'");
                        }
                        if (request.Model.Email is not null && request.Model.Email != "")
                        {
                            sb.AppendLine("AND Email=" + "'" + request.Model.Email + "'");
                        }
                        if (request.Model.BatchNo is not null && request.Model.BatchNo != "")
                        {
                            sb.AppendLine("AND BatchNo=" + "'" + request.Model.BatchNo + "'");
                        }
                        if (request.Model.Organaization is not null && request.Model.Organaization != "")
                        {
                            sb.AppendLine("AND Organaization=" + request.Model.Organaization);
                        }
                        if (request.Model.Designation is not null && request.Model.Designation != "")
                        {
                            sb.AppendLine("AND Designation=" + request.Model.Designation);
                        }
                        if (request.Model.Specialization is not null && request.Model.Specialization != "")
                        {
                            sb.AppendLine("AND Specialization=" + request.Model.Specialization);
                        }
                        if (request.Model.HscYear is not null && request.Model.HscYear != "")
                        {
                            sb.AppendLine("AND HscYear=" + "'" + request.Model.HscYear + "'");
                        }
                        if (request.Model.CadetNo is not null && request.Model.CadetNo != "")
                        {
                            sb.AppendLine("AND CadetNo=" + "'" + request.Model.CadetNo + "'");
                        }
                        if (request.Model.memFullId is not null && request.Model.memFullId != "")
                        {
                            sb.AppendLine("AND memFullId=" + request.Model.memFullId);
                        }

                        
                    }
                    sb.AppendLine("order by MembershipNo");

                    var data = await connection.QueryAsync<ViewMemberDto>(sb.ToString());

                    result = data.ToList();
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
        }

    }
}
