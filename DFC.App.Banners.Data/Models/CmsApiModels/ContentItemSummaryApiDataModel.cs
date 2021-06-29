using Newtonsoft.Json;
using System;

namespace DFC.App.Banners.Data.Models.CmsApiModels
{
    public class ContentItemSummaryApiDataModel
    {
        [JsonProperty(PropertyName = "CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty(PropertyName = "skos__prefLabel")]
        public string? Title { get; set; }

        [JsonProperty(PropertyName = "ModifiedDate")]
        public DateTime Published { get; set; }

        [JsonProperty(PropertyName = "Uri")]
        public Uri? Url { get; set; }
    }
}
