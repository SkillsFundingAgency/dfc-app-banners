using DFC.Content.Pkg.Netcore.Data.Models;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Banners.Data.Models.CmsApiModels
{
    [ExcludeFromCodeCoverage]
    public class PageBannerContentItemApiDataModel : BaseContentItemModel
    {

        [JsonProperty(PropertyName = "banner_WebPageURL")]
        public string? PageLocation { get; set; }

        [JsonProperty(PropertyName = "banner_WebPageName")]
        public string? WebPageName { get; set; }
    }
}
