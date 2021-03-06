using System.Linq;
using ImageData;
using UtilityExtensions;

namespace CmsData
{
    public partial class Picture
    {
        public string ThumbUrl
        {
            get { return "/TinyImage/{0}?v={1}".Fmt(ThumbId ?? -1, CreatedDate.HasValue ? CreatedDate.Value.Ticks : 0); }
        }
        public string SmallUrl
        {
            get { return "/Portrait/{0}?v={1}".Fmt(SmallId ?? -2, CreatedDate.HasValue ? CreatedDate.Value.Ticks : 0); }
        }
        public string MediumUrl
        {
            get { return "/Portrait/{0}?v={1}".Fmt(MediumId ?? -2, CreatedDate.HasValue ? CreatedDate.Value.Ticks : 0); }
        }
        public string LargeUrl
        {
            get { return "/Portrait/{0}?v={1}".Fmt(LargeId ?? -2, CreatedDate.HasValue ? CreatedDate.Value.Ticks : 0); }
        }
    }
}
