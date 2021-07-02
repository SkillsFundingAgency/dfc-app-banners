using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Banners.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class IndexDocumentViewModel
    {
        public string PageLocation { get; set; } = string.Empty;

        public string PageName { get; set; } = string.Empty;
    }
}
