using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using DFC.App.Banners.Data.Models.CmsApiModels;
using DFC.App.Banners.Data.Models.ContentModels;
using DFC.App.Banners.Models;
using DFC.App.Banners.ViewModels;
using Microsoft.AspNetCore.Html;

namespace DFC.App.Banners.AutoMapperProfiles
{
    [ExcludeFromCodeCoverage]
    public class SharedContentItemModelProfile : Profile
    {
        public SharedContentItemModelProfile()
        {
            CreateMap<SharedContentItemApiDataModel, BannerContentItemModel>()
                .ForMember(d => d.Id, s => s.MapFrom(a => a.ItemId))
                .ForMember(d => d.Etag, s => s.Ignore())
                .ForMember(d => d.ParentId, s => s.Ignore())
                .ForMember(d => d.TraceId, s => s.Ignore())
                .ForMember(d => d.PartitionKey, s => s.Ignore())
                .ForMember(d => d.LastReviewed, s => s.MapFrom(a => a.Published))
                .ForMember(d => d.LastCached, s => s.Ignore());

            CreateMap<BannerContentItemModel, IndexDocumentViewModel>();

            CreateMap<BannerContentItemModel, DocumentViewModel>()
                .ForMember(d => d.HtmlHead, s => s.MapFrom(a => a))
                .ForMember(d => d.Breadcrumb, s => s.Ignore())
                .ForMember(d => d.BodyViewModel, s => s.MapFrom(a => a));

            CreateMap<BannerContentItemModel, HtmlHeadViewModel>()
                .ForMember(d => d.CanonicalUrl, s => s.Ignore())
                .ForMember(d => d.Description, s => s.Ignore())
                .ForMember(d => d.Keywords, s => s.Ignore());

            CreateMap<BannerContentItemModel, BodyViewModel>()
                .ForMember(d => d.Body, s => s.MapFrom(a => new HtmlString(a.Content)));

            CreateMap<BannerContentItemModel, BreadcrumbItemModel>()
                .ForMember(d => d.Route, s => s.Ignore());

            CreateMap<BreadcrumbItemModel, BreadcrumbItemViewModel>()
                .ForMember(d => d.AddHyperlink, s => s.Ignore());
        }
    }
}
