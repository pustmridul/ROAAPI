using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Models.DTOs
{
    public class SslCommerzPayment
    {
        public string store_id { get; set; }
        public string store_passwd { get; set; }
        public string success_url { get; set; }
        public string fail_url { get; set; }
        public string cancel_url { get; set; }
        public string ipn_url { get; set; }
        public string SSLCz_URL { get; set; }
        public string Submit_URL { get; set; }



    }
}
