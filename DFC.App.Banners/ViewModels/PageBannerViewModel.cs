using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Banners.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class PageBannerViewModel
    {
        [Display(Name = "Document Id")]
        public Guid Id { get; set; }

        public string PageLocation { get; set; } = string.Empty;

        //public string PartitionKey { get; set; } = string.Empty;

        public string PageName { get; set; } = string.Empty;

        //public Uri? Url { get; set; }

        //[Display(Name = "Last Reviewed")]
        //[DisplayFormat(DataFormatString = "{0:dd MMM yyyy HH:mm:ss}")]
        //public DateTime LastReviewed { get; set; }

        //[Display(Name = "Created Date")]
        //[DisplayFormat(DataFormatString = "{0:dd MMM yyyy HH:mm:ss}")]
        //public DateTime CreatedDate { get; set; }

        //[Display(Name = "Last Cached")]
        //[DisplayFormat(DataFormatString = "{0:dd MMM yyyy HH:mm:ss}")]
        //public DateTime LastCached { get; set; }

        public List<BannerViewModel> Banners { get; set; } = new List<BannerViewModel>();
    }
}
