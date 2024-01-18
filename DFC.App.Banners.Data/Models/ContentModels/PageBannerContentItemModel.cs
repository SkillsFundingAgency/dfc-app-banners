using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

using DFC.Compui.Cosmos.Contracts;

namespace DFC.App.Banners.Data.Models.ContentModels
{
    [ExcludeFromCodeCoverage]
    public class PageBannerContentItemModel : DocumentModel
    {
        public const string DefaultPageLocation = "/";

        [Required]
        public override string? PartitionKey
        {
            get => PageLocation;
            set { PageLocation = value ?? DefaultPageLocation; }
        }

        public string PageLocation { get; set; } = DefaultPageLocation;

        //[Required]
        //public Uri? Url { get; set; }

        public Guid? ItemId { get; set; }

        public string PageName { get; set; } = string.Empty;

        public List<BannerContentItemModel> Banners { get; set; } = new List<BannerContentItemModel>();

        //public DateTime LastReviewed { get; set; }

        //public DateTime CreatedDate { get; set; }

        //public DateTime LastCached { get; set; } = DateTime.UtcNow;
    }
}
