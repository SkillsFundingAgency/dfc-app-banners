using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using AutoMapper;

//using DFC.App.Banners.Data.Models.CmsApiModels;
using DFC.App.Banners.ViewModels;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.PageBanner;

using Microsoft.AspNetCore.Html;

namespace DFC.App.Banners.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class PageBannerContentItemModelProfile : Profile
    {
        public PageBannerContentItemModelProfile()
        {
            CreateMap<PageBanner, PageBannerViewModel>()
                .ForMember(d => d.Id, s => s.MapFrom(x => x.GraphSync.NodeId))
                .ForMember(d => d.PageLocation, s => s.MapFrom(x => x.Banner.WebPageUrl))
                .ForMember(d => d.PageName, s => s.MapFrom(x => x.Banner.WebPageName))
                .ForMember(d => d.Banners, s => s.MapFrom(x => x.AddABanner.ContentItems));

            CreateMap<ContentItem, BannerViewModel>()
                .ForMember(d => d.Body, s => s.MapFrom(x => new HtmlString(x.Content.Html)));
        }
    }
}