using System.Diagnostics.CodeAnalysis;

using AutoMapper;

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
        }
    }
}
