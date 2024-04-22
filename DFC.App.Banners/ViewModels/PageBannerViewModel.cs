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

        public string PageName { get; set; } = string.Empty;

        public List<BannerViewModel> Banners { get; set; } = new List<BannerViewModel>();
    }
}
