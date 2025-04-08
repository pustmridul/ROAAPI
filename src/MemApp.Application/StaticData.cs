using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application
{
    public static class StaticData
    {
        public class CacheKey
        {
            public static readonly MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromSeconds(60 * 60))
                            .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                            .SetPriority(CacheItemPriority.Normal)
                            .SetSize(1024);

            public static string GetMemberList = "GetMemberList";
            public static string GetMessageTemplate = "GetMessageTemplate";

        }

        public static string SiteTitle = "CADET COLLEGE CLUB";
        public static string ImageConvertToBase64(string url)
        {
            using (var b = new Bitmap(url))
            {
                using (var ms = new MemoryStream())
                {
                    b.Save(ms, ImageFormat.Bmp);
                    return Convert.ToBase64String(ms.ToArray());

                }
            }
        }
    }
}
