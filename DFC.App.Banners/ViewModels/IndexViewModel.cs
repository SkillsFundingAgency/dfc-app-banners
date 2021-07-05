using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Banners.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class IndexViewModel
    {
        public string LocalPath { get; set; } = string.Empty;

        public List<IndexDocumentViewModel> Documents { get; set; } = new List<IndexDocumentViewModel>();
    }
}
