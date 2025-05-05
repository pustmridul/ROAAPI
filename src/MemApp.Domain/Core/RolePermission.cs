using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Res.Domain.Core
{
    public static class RolePermission
    {
        public static readonly Dictionary<string, List<string>> ApiPermissions = new Dictionary<string, List<string>>
        {
            { "User.Get", new List<string> { "1003" } },
            { "User.Post", new List<string> { "1001" } },
            { "User.Delete", new List<string> { "1004" } },

            { "MemberService.Get", new List<string> { "1101" } },
            { "MemberService.Post", new List<string> { "1103" } },
            { "MemberService.Delete", new List<string> { "1104" } },

            { "CollegeSetup.Get", new List<string> { "1201" } },
            { "CollegeSetup.Post", new List<string> { "1203" } },
            { "CollegeSetup.Delete", new List<string> { "1204" } },

            { "MemberTypeSetup.Get", new List<string> { "1301" } },
            { "MemberTypeSetup.Post", new List<string> { "1303" } },
            { "MemberTypeSetup.Delete", new List<string> { "1304" } },

            { "MemberStatusSetup.Get", new List<string> { "1401" } },
            { "MemberStatusSetup.Post", new List<string> { "1403" } },
            { "MemberStatusSetup.Delete", new List<string> { "1404" } },

            { "MemberProfessionSetup.Get", new List<string> { "1501" } },
            { "MemberProfessionSetup.Post", new List<string> { "1503" } },
            { "MemberProfessionSetup.Delete", new List<string> { "1504" } },

            { "MemberShipFeesSetup.Get", new List<string> { "1601" } },
            { "MemberShipFeesSetup.Post", new List<string> { "1603" } },
            { "MemberShipFeesSetup.Delete", new List<string> { "1604" } },

            { "MemberActiveStatusSetup.Get", new List<string> { "1701" } },
            { "MemberActiveStatusSetup.Post", new List<string> { "1703" } },
            { "MemberActiveStatusSetup.Delete", new List<string> { "1704" } },

            { "RoleSetup.Get", new List<string> { "1801" } },
            { "RoleSetup.Post", new List<string> { "1803" } },
            { "RoleSetup.Delete", new List<string> { "1804" } },

            { "ServiceTicketTypeSetup.Get", new List<string> { "1901" } },
            { "ServiceTicketTypeSetup.Post", new List<string> { "1903" } },
            { "ServiceTicketTypeSetup.Delete", new List<string> { "1904" } },

            { "AvailabilitySetup.Get", new List<string> { "2001" } },
            { "AvailabilitySetup.Post", new List<string> { "2003" } },
            { "AvailabilitySetup.Delete", new List<string> { "2004" } },

            { "AreaLayoutSetup.Get", new List<string> { "2101" } },
            { "AreaLayoutSetup.Post", new List<string> { "2103" } },
            { "AreaLayoutSetup.Delete", new List<string> { "2104" } },

            { "TableSetup.Get", new List<string> { "2201" } },
            { "TableSetup.Post", new List<string> { "2203" } },
            { "TableSetup.Delete", new List<string> { "2204" } },

            { "Member.Get", new List<string> { "2301" } },
            { "Member.Post", new List<string> { "2303" } },
            { "Member.Delete", new List<string> { "2304" } },

            { "Topup.Get", new List<string> { "2501" } },
            { "Topup.Post", new List<string> { "2503" } },
            { "Topup.Delete", new List<string> { "2504" } },

            { "SubscriptionCharge.Get", new List<string> { "2701" } },
            { "SubscriptionCharge.Post", new List<string> { "2703" } },
            { "SubscriptionCharge.Delete", new List<string> { "2704" } },

            { "ServiceSetup.Get", new List<string> { "2801" } },
            { "ServiceSetup.Post", new List<string> { "2803" } },
            { "ServiceSetup.Delete", new List<string> { "2804" } },

            { "ActivityTicket.Get", new List<string> { "2901" } },
            { "ActivityTicket.Post", new List<string> { "2903" } },
            { "ActivityTicket.Delete", new List<string> { "2904" } },
            { "ActivityTicket.Status", new List<string> { "2905" } },

        };
    }
}
