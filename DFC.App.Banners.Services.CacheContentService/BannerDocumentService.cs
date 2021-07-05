using DFC.App.Banners.Data.Contracts;
using DFC.App.Banners.Data.Models.ContentModels;
using DFC.Compui.Cosmos.Contracts;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await documentService.DeleteAsync(id);
        }

        public async Task<PageBannerContentItemModel?> GetByIdAsync(Guid id, string? partitionKey = null)
        {
            return await documentService.GetByIdAsync(id, partitionKey);
        }

        public async Task<IEnumerable<Uri>> GetPagebannerUrlsAsync(string bannerContentItemId, string? partitionKeyValue = null)
        {
            var urls = new List<Uri>();
            var feedOptions = new FeedOptions { MaxItemCount = 1, EnableCrossPartitionQuery = true };

            if (!string.IsNullOrEmpty(partitionKeyValue))
            {
                feedOptions.EnableCrossPartitionQuery = false;
                feedOptions.PartitionKey = new PartitionKey(partitionKeyValue);
            }

            Uri documentCollectionUri = UriFactory.CreateDocumentCollectionUri(cosmosDbConnection.DatabaseId, cosmosDbConnection.CollectionId);

            var query = documentClient.CreateDocumentQuery<IEnumerable<string>>(
                documentCollectionUri,
                new SqlQuerySpec($"SELECT c.Url FROM c JOIN b IN c.Banners WHERE b.ItemId = @itemId")
                {
                    Parameters = new SqlParameterCollection(new[]
                    {
                        new SqlParameter("@itemId", bannerContentItemId),
                    }),
                },
                feedOptions).AsDocumentQuery();

            while (query.HasMoreResults)
            {
                var result = await query.ExecuteNextAsync<PageBannerContentItemModel>().ConfigureAwait(false);
                urls.AddRange(result.Select(x => x.Url!));
            }

            return urls.Any() ? urls : new List<Uri>();
        }

        public async Task<HttpStatusCode> UpsertAsync(PageBannerContentItemModel pageBannerContentItemModel)
        {
            return await documentService.UpsertAsync(pageBannerContentItemModel);
        }
    }
}
