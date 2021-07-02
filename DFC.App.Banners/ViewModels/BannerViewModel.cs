using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Html;

namespace DFC.App.Banners.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class BannerViewModel
    {
        public bool IsActive { get; set; }

        public int Ordinal { get; set; }

        public HtmlString? Body { get; set; } = new HtmlString(string.Empty);
    }
}
