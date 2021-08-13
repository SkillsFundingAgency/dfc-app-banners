using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Models.ContentModels;
using DFC.Compui.Cosmos.Contracts;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace DFC.App.Banners.Services.CacheContentService
{
    public class BannerDocumentService : IBannerDocumentService
    {
        private readonly CosmosDbConnection cosmosDbConnection;
        private readonly IDocumentClient documentClient;
        private readonly IDocumentService<PageBannerContentItemModel> documentService;

        public BannerDocumentService(IDocumentService<PageBannerContentItemModel> documentService, CosmosDbConnection cosmosDbConnection, IDocumentClient documentClient)
        {
            this.documentService = documentService;
            this.cosmosDbConnection = cosmosDbConnection;
            this.documentClient = documentClient;
        }

        public Task<bool> DeleteAsync(Guid id) =>
            documentService.DeleteAsync(id);

        public Task<PageBannerContentItemModel?> GetByIdAsync(Guid id, string? partitionKey = null) =>
            documentService.GetByIdAsync(id, partitionKey);

        public async Task<IEnumerable<Uri>> GetPageBannerUrlsAsync(string bannerContentItemId, string? partitionKeyValue = null)
        {
            var urls = new List<Uri>();
            var feedOptions = new FeedOptions { MaxItemCount = 1, EnableCrossPartitionQuery = true };

            if (!string.IsNullOrEmpty(partitionKeyValue))
            {
                feedOptions.EnableCrossPartitionQuery = false;
                feedOptions.PartitionKey = new PartitionKey(partitionKeyValue);
            }

            IDocumentQuery<IEnumerable<string>>? query = documentClient.CreateDocumentQuery<IEnumerable<string>>(
                UriFactory.CreateDocumentCollectionUri(cosmosDbConnection.DatabaseId, cosmosDbConnection.CollectionId),
                new SqlQuerySpec($"SELECT DISTINCT c.Url FROM c JOIN b IN c.Banners WHERE b.ItemId = @itemId")
                {
                    Parameters = new SqlParameterCollection(new[]
                    {
                        new SqlParameter("@itemId", bannerContentItemId),
                    }),
                },
                feedOptions).AsDocumentQuery();

            while (query.HasMoreResults)
            {
                var result = await query.ExecuteNextAsync<PageBannerContentItemModel>();
                urls.AddRange(result.Select(x => x.Url!));
            }

            return urls.Any() ? urls : new List<Uri>();
        }

        public Task<HttpStatusCode> UpsertAsync(PageBannerContentItemModel pageBannerContentItemModel) =>
            documentService.UpsertAsync(pageBannerContentItemModel);

        public Task<bool> PurgeAsync()
        {
            return documentService.PurgeAsync();
        }
    }
}
