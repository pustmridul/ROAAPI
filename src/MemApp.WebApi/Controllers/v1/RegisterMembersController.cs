using Hangfire;
using MediatR;
using MemApp.Application.Com.Commands.ChangedPassword;
using MemApp.Application.Com.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Mem.Colleges.Queries;
using MemApp.Application.Mem.Commands;
using MemApp.Application.Mem.Members.Command;
using MemApp.Application.Mem.Members.Models;
using MemApp.Application.Mem.Members.Queries;
using MemApp.Application.Mem.MemberStatuss.Command;
using MemApp.Application.Mem.MemberStatuss.Queries;
using MemApp.Application.Mem.Subscription.Queries;
using MemApp.Application.Models.DTOs;
using MemApp.Reporting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using Newtonsoft.Json;
using System.Data;

namespace MemApp.WebApi.Controllers.v1
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RegisterMembersController : ApiControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public RegisterMembersController(IWebHostEnvironment webHostEnvironment, ISender mediator) : base(mediator)
        {
            _webHostEnvironment = webHostEnvironment;

        }
        [HttpPost]
        public async Task<IActionResult> Save(RegisterMemberRes model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var result = await Mediator.Send(new CreateMemberCommand { Model = model });
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateMember(MemberUpdateVm model)
        {
            var result = await Mediator.Send(new UpdateMemberCommand { Model = model });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> FamilySave(MemberFamilyReq model)
        {
            var result = await Mediator.Send(new CreateMemberFamilyCommand { Model = model });
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> MemberRegistrationFeeSave(MemberFeeReq model)
        {
            var result = await Mediator.Send(new CreateMemberRegistrationFeeCommand { Model = model });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetMemberList()
        {
            var result = await Mediator.Send(new GetAllMembersQuery());
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetCommitteeMembers(string memberShipNo)
        {
            var result = await Mediator.Send(new GetAllMembersQuery()
            {
                SearchText = memberShipNo,
                webRootPath = _webHostEnvironment.WebRootPath,
            });
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> GetMemberData(MemberSearchReq? model, int pageNumber = 1, int pageSize = 10)
        {
            var result = await Mediator.Send(new GetMemberSearchQuery() { Model = model, PageNumber = pageNumber, PageSize = pageSize, WebPath = _webHostEnvironment.WebRootPath });
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await Mediator.Send(new GetMemberByIdQuery() { Id = id, WebRootPath = _webHostEnvironment.WebRootPath });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetByMemberShipNo(string memberShipNo)
        {
            var result = await Mediator.Send(new GetMemberByMemberShipNoQuery() { MemberShipNo = memberShipNo });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetByMemberCardNo(string cardNo)
        {
            var result = await Mediator.Send(new GetMemberByMemberCardNoQuery() { CardNo = cardNo });
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> ResetPin(int id)
        {
            var result = await Mediator.Send(new ResetMemberPinCommand() { MemberId = id });
            return Ok(result);
        }
        [HttpDelete]
        public async Task<IActionResult> Remove(int id)
        {
            var result = await Mediator.Send(new DeleteMemberCommand() { Id = id });
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetByMemberFiles(int memberId)
        {
            var result = await Mediator.Send(new GetMemberFileByIdQuery() { Id = memberId });
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> ChangeUserPin(ChangePin model)
        {
            var result = await Mediator.Send(new ChangedPinCommand() { Model = model });
            return Ok(result);
        }
        [HttpDelete]
        public async Task<IActionResult> AccountRemove(int id)
        {
            var result = new Result();
            result.HasError = false;
            result.Messages?.Add("Account Remove Success");
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> ExportMemberList(MemberSearchReq model, string reportType = "PDF", string reportName = "Member")
        {
            IActionResult response = Unauthorized();

            Dictionary<string, DataTable> data = new();

            var result = await Mediator.Send(new ExportMemberListQuery() { Model = model, WebPath = _webHostEnvironment.WebRootPath });

            if (result != null)
            {
                DataTable dt = CustomExtensions.ConvertListToDataTable(result);
                data.Add("ds", dt);
            }

            var path = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + reportName + ".rdlc";
            ReportDomain reportDomain = new(reportType, data, path, null);
            response = Ok(File(new ReportApplication().Load(reportDomain), reportDomain.mimeType, System.Guid.NewGuid().ToString() + "." + reportType));
            return response;
        }

        [HttpGet]
        public async Task<IActionResult> GetSelectedMemberView(string queryString, string model)
        {
            if (!string.IsNullOrEmpty(queryString))
            {

                var fmodel = JsonConvert.DeserializeObject<MemberSearchReq>(model);
                if (fmodel == null)
                {
                    fmodel = new MemberSearchReq();
                }
                fmodel.queryString = queryString;
                var result = await Mediator.Send(new SelectedMemberViewQuery() { Model = fmodel });
                return Ok(result);
            }
            return BadRequest();

        }

        [HttpGet]
        public async Task<FileResult> Export(string queryString)
        {
            var vm = await Mediator.Send(new ExportMemberListSelectedColumnQuery() { QueryString = queryString });

            return File(vm.Content, vm.ContentType, vm.FileName);
        }

        [HttpPost]
        public async Task<IActionResult> PrintAddress(MemberSearchReq model)
        {
            if (!string.IsNullOrEmpty(model.queryString))
            {
                var result = await Mediator.Send(new SelectedMemberViewQuery() { Model = model });


                var newResultList = new List<ReportDto>();
                var newResult = new ReportDto();
                var i = 0;
                if (result.Count() > 1)
                {
                    foreach (var item in result)
                    {
                        if (i % 2 == 0)
                        {
                            newResult.MembershipNo1 = item.MembershipNo;
                            newResult.FullName1 = item.FullName;
                            newResult.HomeAddress1 = item.HomeAddress;

                        }
                        else
                        {
                            newResult.MembershipNo2 = item.MembershipNo;
                            newResult.FullName2 = item.FullName;
                            newResult.HomeAddress2 = item.HomeAddress;
                            newResultList.Add(newResult);
                            newResult = new ReportDto();
                        }
                        i++;
                    }
                }
                else
                {

                    newResult.MembershipNo1 = result[0].MembershipNo;
                    newResult.FullName1 = result[0].FullName;
                    newResult.HomeAddress1 = result[0].HomeAddress;
                    newResultList.Add(newResult);
                }

                LocalReport localReport = new LocalReport();

                string mimeType = "application/pdf";
                string extension = "pdf";

                var dataTabel = new DataTable();
                dataTabel.Columns.Add("MemberShipNo1");
                dataTabel.Columns.Add("FullName1");
                dataTabel.Columns.Add("HomeAddress1");
                dataTabel.Columns.Add("MemberShipNo2");
                dataTabel.Columns.Add("FullName2");
                dataTabel.Columns.Add("HomeAddress2");


                DataRow row;

                foreach (var item in newResultList)
                {
                    row = dataTabel.NewRow();
                    if (item.MembershipNo1 != null)
                    {
                        row["MembershipNo1"] = item.MembershipNo1;
                        row["FullName1"] = item.FullName1;
                        row["HomeAddress1"] = item.HomeAddress1;

                        row["MembershipNo2"] = item.MembershipNo2;
                        row["FullName2"] = item.FullName2;
                        row["HomeAddress2"] = item.HomeAddress2;
                    }



                    dataTabel.Rows.Add(row);
                }


                localReport.DataSources.Add(new ReportDataSource("memberAddressPrintDataSet", dataTabel));



                localReport.ReportPath = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "MemberAddressPrintReport" + ".rdlc";

                var pdf = localReport.Render("PDF");
                var file = File(pdf, mimeType, "report." + extension);
                return file;



            }
            return BadRequest();

        }

        [HttpPost]
        public async Task<IActionResult> GetAllMemberPhoneNo(MemberSearchReq model)
        {
            var result = await Mediator.Send(new GetMemberPhoneListQuery()
            {
                Model = model
            });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SendMail([FromBody] int Id)
        {
            var templatePath = $"{this._webHostEnvironment.WebRootPath}\\EmailTemplate\\" + "MemberLedgerTemplate" + ".html";
            var result = await Mediator.Send(new SendMemberLedgerMailCommand() { Id = Id, TemplatePath = templatePath });
            if (result.HasError == false)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }

        }


        //[HttpPost]
        //public async Task<IActionResult> SendMemberLedgerMailToSelectedMember([FromBody] List<int> ids)
        //{
        //    var templatePath = $"{this._webHostEnvironment.WebRootPath}\\EmailTemplate\\" + "MemberLedgerTemplate" + ".html";
        //    var result = await Mediator.Send(new SendSelectedMemberLedgerMailCommand() { ids = ids, TemplatePath = templatePath });
        //    if (result.HasError == false)
        //    {
        //        return Ok();
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }

        //}

        [HttpPost]
        public async Task<IActionResult> SendMemberLedgerMailToSelectedMember([FromBody] MailMemberLedgerDto model)
        {
            var templatePath = $"{this._webHostEnvironment.WebRootPath}\\EmailTemplate\\" + "SelectedMemberLedgerTemplate" + ".html";
            // Queue the job to run in the background
            BackgroundJob.Enqueue(() => ExecuteMailSending(model, templatePath));
            return Ok();
        }

        // Background job method
        //[AutomaticRetry(Attempts = 3)] // optional retry configuration for Hangfire
        [NonAction]
        public async Task ExecuteMailSending(MailMemberLedgerDto model, string templatePath)
        {
            var result = await Mediator.Send(new SendSelectedMemberLedgerMailCommand() { Model = model, TemplatePath = templatePath });
            if (result.HasError)
            {
                // Log or handle the error as necessary
            }
        }





        [HttpGet]
        public async Task<IActionResult> GetMemberReportData(string model)
        {
            var fmodel = JsonConvert.DeserializeObject<MemberSearchReq>(model);
            if (fmodel == null)
            {
                fmodel = new MemberSearchReq();
            }

            var data = await Mediator.Send(new GetMemberReportSearchQuery() { Model = fmodel, WebPath = _webHostEnvironment.WebRootPath });

            LocalReport localReport = new LocalReport();

            string mimeType = "application/pdf";
            string extension = "pdf";

            var dataTabel = new DataTable();
            dataTabel.Columns.Add("Id");
            dataTabel.Columns.Add("MemberShipNo");
            dataTabel.Columns.Add("Name");
            dataTabel.Columns.Add("CollegeCode");
            dataTabel.Columns.Add("CollegeName");
            dataTabel.Columns.Add("CadetName");
            dataTabel.Columns.Add("CadetNo");
            dataTabel.Columns.Add("BatchNo");
            dataTabel.Columns.Add("TypeText");
            dataTabel.Columns.Add("StatusText");
            dataTabel.Columns.Add("ActiveStatusText");
            dataTabel.Columns.Add("Dbo");
            dataTabel.Columns.Add("BloodGroupText");
            dataTabel.Columns.Add("HscYear");
            dataTabel.Columns.Add("ProfessionText");
            dataTabel.Columns.Add("ImgFileUrl");
            dataTabel.Columns.Add("Phone");
            dataTabel.Columns.Add("CardNo");
            dataTabel.Columns.Add("Email");
            dataTabel.Columns.Add("MemberFullId");
            dataTabel.Columns.Add("JoinDate");
            dataTabel.Columns.Add("LeaveDate");
            dataTabel.Columns.Add("PermanentAddress");
            dataTabel.Columns.Add("NID");
            dataTabel.Columns.Add("EmergencyContact");

            DataRow row;

            foreach (var item in data.DataList)
            {
                row = dataTabel.NewRow();
                row["Id"] = item.Id;
                row["MemberShipNo"] = item.MemberShipNo;
                row["Name"] = item.Name;

                row["CollegeCode"] = item.CollegeCode;
                row["CollegeName"] = item.CollegeName;
                row["CadetName"] = item.CadetName;
                row["CadetNo"] = item.CadetNo;
                row["BatchNo"] = item.BatchNo;
                row["TypeText"] = item.TypeText;
                row["StatusText"] = item.StatusText;
                row["ActiveStatusText"] = item.ActiveStatusText;
                row["Dbo"] = item.Dbo;
                row["BloodGroupText"] = item.BloodGroupText;
                row["HscYear"] = item.HscYear;
                row["ProfessionText"] = item.ProfessionText;
                row["ImgFileUrl"] = item.ImgFileUrl;
                row["Phone"] = item.Phone;
                row["CardNo"] = item.CardNo;
                row["Email"] = item.Email;
                row["MemberFullId"] = item.MemberFullId;
                row["JoinDate"] = item.JoinDate;
                row["LeaveDate"] = item.LeaveDate;
                row["PermanentAddress"] = item.PermanentAddress;
                row["NID"] = item.NID;
                row["EmergencyContact"] = item.EmergencyContact;

                dataTabel.Rows.Add(row);
            }

            localReport.DataSources.Add(new ReportDataSource("ds", dataTabel));
            localReport.ReportPath = $"{this._webHostEnvironment.WebRootPath}\\RDLC\\" + "MemberReport" + ".rdlc";

            var pdf = localReport.Render("PDF");
            var file = File(pdf, mimeType, "report." + extension);
            return file;
        }

    }
}

