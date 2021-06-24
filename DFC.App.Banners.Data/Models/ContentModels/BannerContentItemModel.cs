using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace DFC.App.Banners.Data.Models.ContentModels
{
    [ExcludeFromCodeCoverage]
    public class BannerContentItemModel
    {
        public Uri? Url { get; set; }

        public Guid? ItemId { get; set; }

        public DateTime LastCached { get; set; } = DateTime.UtcNow;

        public DateTime? CreatedDate { get; set; }

        public DateTime LastReviewed { get; set; }

        public string? Content { get; set; }

    }
}
