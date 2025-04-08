using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Models.DTOs
{
    public class WhatsAppModel
    {
    }
    public class Language
    {
        public string code { get; set; }
    }
    public class Template
    {
        public string name { get; set; }
        public Language language { get; set; }
        public List<WhatsAppComponent> components { get; set; }


    }
    public class WhatsAppComponent
    {
        public string type { get; set; }
        public List<object> parameters { get; set; }
    }
    public class WhatsAppRequest
    {
        public string messaging_product { get; set; } = "whatsapp";
        public string recipient_type { get; set; } = "individual";
        public string to { get; set; }
        public string type { get; set; } = "template";
        public Template template { get; set; }
    }
}
