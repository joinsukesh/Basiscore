
namespace Basiscore.ContentAuthoringGuide.Areas.ContentAuthoringGuide.Models
{
    public class MenuItem
    {
        public ArticlesFolder ArticlesFolder { get; set; }

        public Article Article { get; set; }

        public bool IsArticle { get; set; }
    }
}