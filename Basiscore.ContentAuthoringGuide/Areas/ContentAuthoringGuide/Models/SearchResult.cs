

namespace Basiscore.ContentAuthoringGuide.Areas.ContentAuthoringGuide.Models
{
    using Sitecore.Data;

    public class SearchResult
    {
        public ID ArticleItemID { get; set; }

        public string ArticleTitle { get; set; }

        public string MenuCategory { get; set; }

        public bool IsKeywordInArticleTitle { get; set; }

        public bool IsKeywordInArticleContent { get; set; }

        public bool IsKeywordInArticleSlides { get; set; }

        public string MatchFoundIn
        {
            get
            {
                string matchFoundLocations = string.Empty;

                if (this.IsKeywordInArticleTitle)
                {
                    matchFoundLocations += "Article Title, ";
                }

                if (this.IsKeywordInArticleContent)
                {
                    matchFoundLocations += "Article Content, ";
                }

                if (this.IsKeywordInArticleSlides)
                {
                    matchFoundLocations += "Article Slides";
                }

                matchFoundLocations = matchFoundLocations.Trim().TrimEnd(',');
                return matchFoundLocations;
            }
        }
    }
}