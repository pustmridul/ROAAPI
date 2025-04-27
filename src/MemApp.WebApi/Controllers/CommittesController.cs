using MediatR;
using MemApp.Application.Mem.MemberTypes.Queries;
using MemApp.Reporting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Res.Domain.Entities;
using ResApp.Application.Com.Commands.MemberRegistration;
using ResApp.Application.Com.Commands.UpdateThana;
using ResApp.Application.Com.Queries.GetMemberRegistrationInfo;
using ResApp.Application.Com.Queries.ROSubcriptionPaymentReport;
using ResApp.Application.Models.DTOs;
using ResApp.Application.ROA.CommitteeCategory.Command;
using ResApp.Application.ROA.CommitteeCategory.Queries;
using ResApp.Application.ROA.Committees.Commands;
using ResApp.Application.ROA.Committees.Models;
using ResApp.Application.ROA.Committees.Queries;
using ResApp.Application.ROA.MemberRegistration;
using System.Data;

namespace Res.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CommitteesController : ApiNewControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CommitteesController( ISender sender, IWebHostEnvironment webHostEnvironment) : base(sender)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CommitteeSearchParam paginationParams)
        {
            var result = await Mediator.Send(new GetAllRoCommitteeQuery() 
            { 
               Model=paginationParams
            });

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] RoCommitteeReq ExecutiveCommittee)
        {
            // command.WebRootPath = _hostingEnv.WebRootPath;
            var result = await Mediator.Send(new CreateRoCommitteeCommand()
            {
                Model = ExecutiveCommittee
            });

            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMemberInfoByMemberShip(string memberShipNo)
        {
            return Ok(await Mediator.Send(new GetAllMemberByMemberShipNoQuery()
            {
                MemberShipNo = memberShipNo
            }));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetById(int Id)
        {
            return Ok(await Mediator.Send(new GetRoCommitteeByIdQuery()
            {
                Id = Id
            }));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveCategory(CreateCommitteeCatCommand command)
        {
            return Ok(await Mediator.Send(command));

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            return Ok(await Mediator.Send(new GetAllCommitteeCatQuery()
            {
               
            }));

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            return Ok(await Mediator.Send(new GetCommitteeCatByIdQuery()
            {
                CategoryId = categoryId
            }));

        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveCategory(int id)
        {
            var result = await Mediator.Send(new DeleteCommitteeCatCommand() { Id = id });
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ExportExecutiveCommitteeReport(int committeeId, bool hasImage)
        {
            try
            {
                var result = await Mediator.Send(new ExportRoCommitteeInfoQuery() { Id = committeeId });

                string mimeType = "application/pdf";
                string extension = "pdf";

                var masterDataTable = new DataTable();
                masterDataTable.Columns.Add("Title");
                masterDataTable.Columns.Add("CommitteeType");
                masterDataTable.Columns.Add("CommitteeCategoryName");
                masterDataTable.Columns.Add("CommitteeDate");
                masterDataTable.Columns.Add("CommitteeYear");
                masterDataTable.Columns.Add("DivisionName");
                masterDataTable.Columns.Add("DistrictName");
                masterDataTable.Columns.Add("ThanaName");
                masterDataTable.Columns.Add("ZoneName");

                DataRow masterRow;
                masterRow = masterDataTable.NewRow();

                // masterRow["SiteLogo"] = StaticData.ImageConvertToBase64(this._webHostEnvironment.WebRootPath + "\\Image\\sitelogo.png");


                masterRow["Title"] = result.Title;
                masterRow["CommitteeType"] = result.CommitteeType;
                masterRow["CommitteeCategoryName"] = result.CommitteeCategoryName;
                masterRow["CommitteeDate"] = result.CommitteeDate;
                masterRow["CommitteeYear"] = result.CommitteeYear;
                masterRow["DivisionName"] = result.DivisionName;
                masterRow["DistrictName"] = result.DistrictName;
                masterRow["ZoneName"] = result.ZoneName;
                masterRow["ThanaName"] = result.ThanaName;



                masterDataTable.Rows.Add(masterRow);

                var committeDetails = await Mediator.Send(new ExportRoCommitteeDetailQuery() 
                { 
                    Id = committeeId,
                    RoothPath=this._webHostEnvironment.WebRootPath,
                    HasImage = hasImage,
                });

                var dataTable = new DataTable();

                dataTable.Columns.Add("Designation");
                dataTable.Columns.Add("MemberName");
                dataTable.Columns.Add("MembershipNo");
                dataTable.Columns.Add("ImgFileUrl");
              //  dataTable.Columns.Add("LateAmount");


                DataRow row;


                foreach (var item in committeDetails)
                {
                    row = dataTable.NewRow();
                    row["Designation"] = item.Designation;
                    row["MemberName"] = item.MemberName;
                    row["MembershipNo"] = item.MembershipNo;
                    row["ImgFileUrl"] = item.ImgFileUrl;
                   // row["SubscriptionName"] = item.MembershipNo;                

                    dataTable.Rows.Add(row);
                }

                Dictionary<string, DataTable> data = new()
                {
                    { "ds", dataTable },
                    { "master", masterDataTable }
                };
                var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "RoSubCommitteeReport" + ".rdlc";

                ReportDomain reportDomain = new("PDF", data, path, null!);
                return File(new ReportApplication().Load(reportDomain), reportDomain.mimeType, System.Guid.NewGuid().ToString() + "." + "PDF");

                //localReport.DataSources.Add(new ReportDataSource("ds", dataTable));
                //localReport.ReportPath = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "SubscriptionPaymentReport" + ".rdlc";

                //var pdf = localReport.Render("PDF");
                //var file = File(pdf, mimeType, "report." + extension);
                //return file;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
