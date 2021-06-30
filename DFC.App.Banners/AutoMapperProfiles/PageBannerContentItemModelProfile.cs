using System.Diagnostics.CodeAnalysis;

using AutoMapper;

using DFC.App.Banners.Data.Models.CmsApiModels;
using DFC.App.Banners.Data.Models.ContentModels;
using DFC.App.Banners.ViewModels;

using Microsoft.AspNetCore.Html;

namespace DFC.App.Banners.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class PageBannerContentItemModelProfile : Profile
    {
        public PageBannerContentItemModelProfile()
        {
            CreateMap<PageBannerContentItemModel, PageBannerViewModel>();

            CreateMap<BannerContentItemModel, BodyViewModel>()
                .ForMember(d => d.Body, s => s.MapFrom(a => new HtmlString(a.Content)));

            CreateMap<PageBannerContentItemApiDataModel, PageBannerContentItemModel>()
                .ForMember(d => d.PartitionKey, s => s.Ignore())
                .ForMember(d => d.PageLocation, s => s.Ignore())
                .ForMember(d => d.PageName, s => s.Ignore())
                .ForMember(d => d.Banners, s => s.Ignore())
                .ForMember(d => d.LastReviewed, s => s.Ignore())
                .ForMember(d => d.LastCached, s => s.Ignore())
                .ForMember(d => d.Id, s => s.Ignore())
                .ForMember(d => d.Etag, s => s.Ignore())
                .ForMember(d => d.TraceId, s => s.Ignore())
                .ForMember(d => d.ParentId, s => s.Ignore());
        }
    }
}
