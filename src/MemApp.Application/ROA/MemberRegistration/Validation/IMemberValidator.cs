using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.ROA.MemberRegistration.Validation
{
    public interface IMemberValidator
    {
        public string? EmailId { get; }
        public string? ApplicationNo { get; }
        public string? Name { get;  }
        public string? NomineeName { get;  }
        public string? InstituteNameBengali { get; }
        public string? InstituteNameEnglish { get;}
        public string? PhoneNo { get;  }
        public string? PermanentAddress { get;  }
        public string? MemberNID { get;  }
        public string? MemberTINNo { get;  }
        public string? MemberTradeLicense { get;  }
        public DateTime? BusinessStartingDate { get;  }
        public int? DivisionId { get; }
        public int? DistrictId { get;  }
        public int? ThanaId { get;  }

    }
}
