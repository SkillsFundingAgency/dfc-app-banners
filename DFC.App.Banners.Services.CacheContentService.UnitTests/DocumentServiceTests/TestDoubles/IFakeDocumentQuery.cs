using Microsoft.Azure.Documents.Linq;
using System.Linq;

namespace DFC.App.Banners.Services.CacheContentService.UnitTests.DocumentServiceTests.TestDoubles
{
    public interface IFakeDocumentQuery<T> : IDocumentQuery<T>, IOrderedQueryable<T>
    {
    }
}