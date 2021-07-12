using System;
using System.Collections.Generic;

using AutoMapper;

using DFC.App.Banners.Data.Models.CmsApiModels;
using DFC.App.Banners.Data.Models.ContentModels;
using DFC.Content.Pkg.Netcore.Data.Contracts;

namespace DFC.App.Banners.AutoMapperProfiles
{
    public class BannerContentItemModelConverter : IValueConverter<IList<IBaseContentItemModel>, List<BannerContentItemModel>>
    {
        public List<BannerContentItemModel> Convert(IList<IBaseContentItemModel> sourceMember, ResolutionContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));

            var result = new List<BannerContentItemModel>();
            if (sourceMember?.Count > 0)
            {
                foreach (var item in sourceMember)
                {
                    if (item is BannerContentItemApiDataModel apiModel)
                    {
                        result.Add(context.Mapper.Map<BannerContentItemModel>(apiModel));
                    }
                }
            }

            return result;
        }
    }
}