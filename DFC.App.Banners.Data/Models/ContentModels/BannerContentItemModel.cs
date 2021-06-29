using System;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

namespace DFC.App.Banners.Data.Models.ContentModels
{
    [ExcludeFromCodeCoverage]
    public class BannerContentItemModel
    {
        public Guid? ItemId { get; set; }

        public string Title { get; set; } = string.Empty;

        public int Ordinal { get; set; }

        public bool IsActive { get; set; }

        public bool IsGlobal { get; set; }

        public bool UseBrowserWidth { get; set; }

        public DateTime LastCached { get; set; } = DateTime.UtcNow;

        public DateTime? CreatedDate { get; set; }

        public DateTime LastReviewed { get; set; }

        public string Content { get; set; } = string.Empty;
    }
}
