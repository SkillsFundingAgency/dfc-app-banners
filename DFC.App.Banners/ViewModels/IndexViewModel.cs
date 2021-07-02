using System.Collections.Generic;

namespace DFC.App.Banners.ViewModels
{
    public class IndexViewModel
    {
        public string LocalPath { get; set; } = string.Empty;

        public List<IndexDocumentViewModel> Documents { get; set; } = new List<IndexDocumentViewModel>();
    }
}
