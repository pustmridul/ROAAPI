using System.ComponentModel;

namespace  MemApp.Application.Collection
{
    public partial class PermissionCollection
    {
        [Description("1")]
        public sealed class Com
        {
            [Description("101")]
            public sealed class PerMenu
            {
                public const int CanView = 10101;
                public const int CanCreateEdit = 10102;
                public const int CanDelete = 10199;
            }

           
            [Description("102")]
            public sealed class PerSubMenu
            {
                public const int CanView = 10201;
                public const int CanCreateEdit = 10202;
                public const int CanDelete = 10299;
            }
            [Description("103")]
            public sealed class PerRole
            {
                public const int CanView = 10301;
                public const int CanCreateEdit = 10302;
                public const int CanDelete = 10399;
            }
            [Description("104")]
            public sealed class PerUser
            {
                public const int CanView = 10401;
                public const int CanCreateEdit = 10402;
                public const int CanDelete = 10499;
            }
        }
    }
}
