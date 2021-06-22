using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Html;

namespace DFC.App.Banners.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class BodyViewModel
    {
        public HtmlString? Body { get; set; } = new HtmlString("Unknown content");
    }
}
