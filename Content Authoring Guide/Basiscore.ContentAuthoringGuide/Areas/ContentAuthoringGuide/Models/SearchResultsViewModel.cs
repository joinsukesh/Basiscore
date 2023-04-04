
namespace Basiscore.ContentAuthoringGuide.Areas.ContentAuthoringGuide.Models
{
    using System.Collections.Generic;

    public class SearchResultsViewModel
    {
        public string StatusMessage { get; set; }

        public List<SearchResult> SearchResults { get; set; }
    }
}