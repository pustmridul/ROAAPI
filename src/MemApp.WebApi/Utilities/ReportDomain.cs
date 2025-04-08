using Microsoft.Reporting.NETCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Reporting
{
    public class ReportDomain
    {
        public int index { get; set; }
        public string mimeType { get; set; }
        public string renderFormat { get; set; }
        public Dictionary<string, DataTable> data { get; set; }
        public Dictionary<string, string> param { get; set; }
        public string path { get; set; }
        public ReportDomain(string inputformat, Dictionary<string, DataTable> _data, string _path, Dictionary<string, string> _param)
        {
            switch (inputformat)
            {
                case "PDF":
                    index = 1;
                    mimeType = "application/pdf";
                    renderFormat = inputformat;
                    break;
                case "HTML5":
                    index = 1;
                    mimeType = "text/html";
                    renderFormat = inputformat;
                    break;
                case "WORDOPENXML":
                    index = 1;
                    mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    renderFormat = inputformat;
                    break;
                case "EXCELOPENXML":
                    index = 1;
                    mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    renderFormat = inputformat;
                    break;
                default:
                    index = 1;
                    mimeType = "application/pdf";
                    renderFormat = inputformat;
                    break;
            }

            data = _data;
            path = _path;
            param = _param;
        }
    }
    public class ReportApplication
    {
        public dynamic Load(ReportDomain reportDomain)
        {
            LocalReport report = new();
            List<ReportParameter> reportParameters = new();
            using (var stream = new FileStream(reportDomain.path, FileMode.Open))
            {
                report.LoadReportDefinition(stream);
            }
            foreach (KeyValuePair<string, DataTable> entry in reportDomain.data)
            {
                report.DataSources.Add(new ReportDataSource(entry.Key, entry.Value));
            }
            if (reportDomain.param != null && reportDomain.param.Count > 0)
            {
                foreach (KeyValuePair<string, string> entry in reportDomain.param)
                {
                    reportParameters.Add(new ReportParameter(entry.Key, entry.Value));
                }
                report.SetParameters(reportParameters);
            }

            return report.Render(reportDomain.renderFormat);
        }
    }
}
