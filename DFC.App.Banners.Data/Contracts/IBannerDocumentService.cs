﻿using DFC.App.Banners.Data.Models.ContentModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.Banners.Data.Contracts
{
    public interface IBannerDocumentService
    {
        Task<IEnumerable<Uri>> GetPagebannerUrisAsync(string bannerContentItemId, string? partitionKeyValue = null);

        Task<bool> DeleteAsync(Guid id);

        Task<HttpStatusCode> UpsertAsync(PageBannerContentItemModel pageBannerContentItemModel);

        Task<PageBannerContentItemModel?> GetByIdAsync(Guid id, string? partitionKey = null);
    }
}