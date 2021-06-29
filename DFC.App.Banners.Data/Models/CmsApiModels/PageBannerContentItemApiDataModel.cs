using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace DFC.App.Banners.Data.Models.CmsApiModels
{
    public class PageBannerContentItemApiDataModel
    {
        [JsonProperty(PropertyName = "skos__prefLabel")]
        public string? Title { get; set; }

        [JsonProperty("PageUrl")]
        public string? PageUrl { get; set; }

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime? ModifiedDate { get; set; }

        [JsonProperty(PropertyName = "CreatedDate")]
        public DateTime? CreatedDate { get; set; }

        [JsonProperty(PropertyName = "uri")]
        public Uri? Url { get; set; }

        [JsonProperty("WebpPgeName")]
        public string? WebPageName { get; set; }

        [JsonProperty(PropertyName = "id")]
        public Guid? Id { get; set; }

        [JsonProperty("_links")]
        public JObject? ContentLinks { get; set; }
    }
}
