using System.Diagnostics.CodeAnalysis;
using AutoMapper;

using DFC.App.Banners.Data.Models.CmsApiModels;
using DFC.App.Banners.Data.Models.ContentModels;
using DFC.App.Banners.ViewModels;
using DFC.Content.Pkg.Netcore.Data.Models;

using Microsoft.AspNetCore.Html;

namespace DFC.App.Banners.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class PageBannerContentItemModelProfile : Profile
    {
        public PageBannerContentItemModelProfile()
        {
            CreateMap<PageBannerContentItemModel, PageBannerViewModel>();

            CreateMap<BannerContentItemModel, BannerViewModel>()
                .ForMember(d => d.Body, s => s.MapFrom(a => new HtmlString(a.Content)));

            CreateMap<BannerContentItemApiDataModel, BannerContentItemModel>();

            CreateMap<PageBannerContentItemApiDataModel, PageBannerContentItemModel>()
                .ForMember(d => d.Id, s => s.MapFrom(x => x.ItemId))
                .ForMember(d => d.PageName, s => s.MapFrom(x => x.PageName))
                .ForMember(d => d.Banners, s => s.MapFrom(x => x.ContentItems))
                .ForMember(d => d.PartitionKey, s => s.MapFrom(x => x.PageLocation))
                .ForMember(d => d.PageLocation, s => s.MapFrom(x => x.PageLocation))
                .ForMember(d => d.Banners, s => s.Ignore())
                .ForMember(d => d.LastReviewed, s => s.Ignore())
                .ForMember(d => d.LastCached, s => s.Ignore())
                .ForMember(d => d.Etag, s => s.Ignore())
                .ForMember(d => d.TraceId, s => s.Ignore())
                .ForMember(d => d.ParentId, s => s.Ignore());

            CreateMap<LinkDetails, BannerContentItemApiDataModel>()
                .ForMember(d => d.Url, s => s.Ignore())
                .ForMember(d => d.ItemId, s => s.Ignore())
                .ForMember(d => d.Title, s => s.Ignore())
                .ForMember(d => d.Published, s => s.Ignore())
                .ForMember(d => d.CreatedDate, s => s.Ignore())
                .ForMember(d => d.Links, s => s.Ignore())
                .ForMember(d => d.ContentLinks, s => s.Ignore())
                .ForMember(d => d.ContentItems, s => s.Ignore())
                .ForMember(d => d.IsActive, s => s.Ignore())
                .ForMember(d => d.IsGlobal, s => s.Ignore())
                .ForMember(d => d.Content, s => s.Ignore())
                .ForMember(d => d.UseBrowserWidth, s => s.Ignore())
                .ForMember(d => d.LastCached, s => s.Ignore())
                .ForMember(d => d.LastReviewed, s => s.Ignore());
        }
    }
}