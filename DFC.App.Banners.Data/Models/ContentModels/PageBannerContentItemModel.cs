using DFC.Compui.Cosmos.Contracts;

using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

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

        public string? PageLocation { get; set; } = DefaultPageLocation;

        [Required]
        public Uri? Url { get; set; }

        public bool UseBrowserWidth { get; set; }

        [Required]
        public string? Content { get; set; }

        public DateTime LastReviewed { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastCached { get; set; } = DateTime.UtcNow;
    }
}
