using DFC.Compui.Cosmos.Contracts;

using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Banners.Data.Models.ContentModels
{
    [ExcludeFromCodeCoverage]
    public class BannerContentItemModel : DocumentModel
    {
        public const string DefaultPartitionKey = "banners";

        public override string? PartitionKey { get; set; } = DefaultPartitionKey;

        [Required]
        public string? Title { get; set; }

        [Required]
        public Uri? Url { get; set; }

        [Required]
        public string? Content { get; set; }

        public DateTime LastReviewed { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastCached { get; set; } = DateTime.UtcNow;
    }
}
