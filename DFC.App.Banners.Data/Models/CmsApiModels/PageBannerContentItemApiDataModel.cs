using System.Diagnostics.CodeAnalysis;

using DFC.Content.Pkg.Netcore.Data.Models;

using Newtonsoft.Json;

namespace DFC.App.Banners.Data.Models.CmsApiModels
{
    [ExcludeFromCodeCoverage]
    public class PageBannerContentItemApiDataModel : BaseContentItemModel
    {
        [JsonProperty("PageUrl")]
        public string PageLocation { get; set; } = string.Empty;
    }
}
