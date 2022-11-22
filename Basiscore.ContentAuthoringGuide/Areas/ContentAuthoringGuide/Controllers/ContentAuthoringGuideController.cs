

namespace Basiscore.ContentAuthoringGuide.Areas.ContentAuthoringGuide.Controllers
{
    using Basiscore.ContentAuthoringGuide.Areas.ContentAuthoringGuide.Models;
    using Basiscore.ContentAuthoringGuide.Areas.ContentAuthoringGuide.Utilities;
    using Sitecore.Data.Items;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;

    public class ContentAuthoringGuideController : Controller
    {
        #region VARIABLES
        #endregion

        #region ACTION RESULTS

        public ActionResult Index(string id)
        {
            GuideRoot viewModel = null;

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    Item documentationRootNode = SitecoreUtility.GetItem(id);

                    if (documentationRootNode != null && documentationRootNode.TemplateID == Templates.GuideRoot.ID)
                    {
                        viewModel = new GuideRoot(documentationRootNode);
                        List<Article> allArticles = null;
                        List<MenuItem> menuItems = null;
                        GetMenuItems(documentationRootNode, out menuItems, out allArticles);
                        viewModel.MenuItems = menuItems;
                        viewModel.Articles = allArticles;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.SaveLog(MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name, ex, LogManager.LogTypes.Error, string.Empty);
                viewModel = null;
            }

            return View("~/Areas/ContentAuthoringGuide/Views/Index.cshtml", viewModel);
        }

        [HttpPost]
        public ActionResult GetSearchResults(string searchKeyword, string rootNodeID)
        {
            SearchResultsViewModel viewModel = new SearchResultsViewModel();
            List<SearchResult> lstSearchResults = new List<SearchResult>();

            try
            {
                if (!string.IsNullOrEmpty(searchKeyword) && searchKeyword.Length > 1 && !string.IsNullOrEmpty(rootNodeID))
                {
                    Item documentationRootNode = SitecoreUtility.GetItem(rootNodeID);

                    if (documentationRootNode != null && documentationRootNode.HasChildren)
                    {
                        Item[] childItems = documentationRootNode.GetChildren().ToArray();
                        string menuCategory = string.Empty;

                        foreach (Item item in childItems)
                        {
                            SearchResult result = null;

                            if (item.TemplateID == Templates.ArticlesFolder.ID &&
                                item.Fields[Templates.ArticlesFolder.Fields.IsActive].Value == "1" &&
                                item.HasChildren)
                            {
                                menuCategory = item.Fields[Templates.ArticlesFolder.Fields.Title].Value;
                                Item[] articleItems = item.GetChildren()
                                    .Where(x => x.TemplateID == Templates.Article.ID &&
                                    x.Fields[Templates.Article.Fields.IsActive].Value == "1").ToArray();

                                if (articleItems != null && articleItems.Length > 0)
                                {
                                    foreach (Item article in articleItems)
                                    {
                                        result = FindKeywordInArticle(searchKeyword, article, menuCategory);

                                        if (result != null && !lstSearchResults.Any(x => x.ArticleItemID == result.ArticleItemID))
                                        {
                                            lstSearchResults.Add(result);
                                        }
                                    }
                                }
                            }
                            else if (item.TemplateID == Templates.Article.ID &&
                                item.Fields[Templates.Article.Fields.IsActive].Value == "1")
                            {
                                menuCategory = string.Empty;
                                result = FindKeywordInArticle(searchKeyword, item, menuCategory);

                                if (result != null && !lstSearchResults.Any(x => x.ArticleItemID == result.ArticleItemID))
                                {
                                    lstSearchResults.Add(result);
                                }
                            }
                        }
                    }
                }

                viewModel.SearchResults = lstSearchResults;

                if (lstSearchResults != null && lstSearchResults.Count > 0)
                {
                    if (lstSearchResults.Count > 1)
                    {
                        viewModel.StatusMessage = lstSearchResults.Count + " results found for '" + searchKeyword + "'";
                    }
                    else
                    {
                        viewModel.StatusMessage = lstSearchResults.Count + " result found for '" + searchKeyword + "'";
                    }
                }
                else
                {
                    viewModel.StatusMessage = "No results found for '" + searchKeyword + "'";
                }
            }
            catch (Exception ex)
            {
                LogManager.SaveLog(MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name, ex, LogManager.LogTypes.Error, string.Empty);
            }

            return PartialView("~/Areas/ContentAuthoringGuide/Views/_SearchResults.cshtml", viewModel);
        }

        #endregion

        #region METHODS

        /// <summary>
        /// search for the keayword in article title, content & slides.
        /// if there is a match in one of the slides, exit the loop.
        /// </summary>
        /// <param name="searchKeyword"></param>
        /// <param name="articleItem"></param>
        /// <param name="menuCategory"></param>
        /// <returns></returns>
        private SearchResult FindKeywordInArticle(string searchKeyword, Item articleItem, string menuCategory)
        {
            SearchResult result = new SearchResult();
            searchKeyword = searchKeyword.ToLower();
            bool isMatchFound = false;
            Article objArticle = new Article(articleItem);

            if (objArticle != null)
            {
                if (objArticle.Title.ToLower().Contains(searchKeyword))
                {
                    isMatchFound = true;
                    result.ArticleItemID = objArticle.InnerItem.ID;
                    result.MenuCategory = menuCategory;
                    result.ArticleTitle = objArticle.Title;
                    result.IsKeywordInArticleTitle = true;
                }

                if (Convert.ToString(objArticle.Description).ToLower().Contains(searchKeyword) ||
                       Convert.ToString(objArticle.ReferenceURL).ToLower().Contains(searchKeyword) ||
                       objArticle.Section1Header.ToLower().Contains(searchKeyword) ||
                       Convert.ToString(objArticle.Section1Description).ToLower().Contains(searchKeyword) ||
                       objArticle.Section2Header.ToLower().Contains(searchKeyword) ||
                       Convert.ToString(objArticle.Section2Description).ToLower().Contains(searchKeyword) ||
                       objArticle.Section3Header.ToLower().Contains(searchKeyword) ||
                       Convert.ToString(objArticle.Section3Description).ToLower().Contains(searchKeyword) ||
                       objArticle.Section4Header.ToLower().Contains(searchKeyword) ||
                       Convert.ToString(objArticle.Section4Description).ToLower().Contains(searchKeyword) ||
                       objArticle.Section5Header.ToLower().Contains(searchKeyword) ||
                       Convert.ToString(objArticle.Section5Description).ToLower().Contains(searchKeyword)
                    )
                {
                    isMatchFound = true;
                    result.ArticleItemID = objArticle.InnerItem.ID;
                    result.MenuCategory = menuCategory;
                    result.ArticleTitle = objArticle.Title;
                    result.IsKeywordInArticleContent = true;
                }

                Item[] slides = articleItem.GetChildren()
       .Where(x => x.TemplateID == Templates.CarouselSlide.ID &&
       x.Fields[Templates.CarouselSlide.Fields.IsActive].Value == "1").ToArray();

                if (slides != null && slides.Length > 0)
                {
                    foreach (Item slide in slides)
                    {
                        CarouselSlide carouselSlide = new CarouselSlide(slide);

                        if (carouselSlide != null && Convert.ToString(carouselSlide.Description).ToLower().Contains(searchKeyword))
                        {
                            isMatchFound = true;
                            result.ArticleItemID = objArticle.InnerItem.ID;
                            result.MenuCategory = menuCategory;
                            result.ArticleTitle = objArticle.Title;
                            result.IsKeywordInArticleSlides = true;
                            break;
                        }
                    }
                }
            }

            if (!isMatchFound)
            {
                result = null;
            }

            return result;
        }

        private void GetMenuItems(Item parentItem, out List<MenuItem> menuItems, out List<Article> articles)
        {
            menuItems = null;
            articles = new List<Article>();

            Item[] documentationRootNodeChildren = parentItem.GetChildren().ToArray();

            if (documentationRootNodeChildren != null && documentationRootNodeChildren.Length > 0)
            {
                menuItems = new List<MenuItem>();
                //if it is an active article item under the root node, add it to the list
                //else, if it is an active article folder item under the root node, add it to the list. Also, add all active child articles under that folder.
                foreach (Item item in documentationRootNodeChildren)
                {
                    if (item.TemplateID == Templates.Article.ID)
                    {
                        MenuItem menuItem = new MenuItem();
                        menuItem.Article = new Article(item);

                        if (menuItem.Article != null && menuItem.Article.IsActive)
                        {
                            menuItem.IsArticle = true;
                            menuItems.Add(menuItem);

                            menuItem.Article.Slides = GetFeaturedSlides(item);
                            articles.Add(menuItem.Article);
                        }
                    }
                    else if (item.TemplateID == Templates.ArticlesFolder.ID)
                    {
                        List<Article> lstArticlesInFolder = new List<Article>();
                        MenuItem menuItem = new MenuItem();
                        menuItem.ArticlesFolder = new ArticlesFolder(item);

                        if (menuItem.ArticlesFolder != null && menuItem.ArticlesFolder.IsActive && menuItem.ArticlesFolder.InnerItem.HasChildren)
                        {
                            IEnumerable<Item> activeArticles = menuItem.ArticlesFolder.InnerItem.GetChildren().Where(x => x.TemplateID == Templates.Article.ID && x.Fields[Templates.Article.Fields.IsActive].Value == "1");

                            if (activeArticles != null && activeArticles.Count() > 0)
                            {
                                foreach (Item articleItem in activeArticles)
                                {
                                    Article article = new Article(articleItem);

                                    if (article != null)
                                    {
                                        article.Slides = GetFeaturedSlides(articleItem);
                                        lstArticlesInFolder.Add(article);
                                        articles.Add(article);
                                    }
                                }
                            }

                            if (lstArticlesInFolder != null && lstArticlesInFolder.Count > 0)
                            {
                                menuItem.ArticlesFolder.Articles = lstArticlesInFolder;
                                menuItem.IsArticle = false;
                                menuItems.Add(menuItem);
                            }
                        }
                    }
                }
            }
        }

        private List<CarouselSlide> GetFeaturedSlides(Item contextItem)
        {
            List<CarouselSlide> lstSlides = null;

            IEnumerable<Item> featuredSlides = SitecoreUtility.GetMultiListValueItems(contextItem, Templates.Article.Fields.Slides)
                .Where(x => x.Fields[Templates.CarouselSlide.Fields.IsActive].Value == "1");

            if (featuredSlides != null && featuredSlides.Count() > 0)
            {
                lstSlides = new List<CarouselSlide>();

                foreach (Item item in featuredSlides)
                {
                    CarouselSlide slide = new CarouselSlide(item);

                    if (slide != null)
                    {
                        lstSlides.Add(slide);
                    }
                }
            }

            return lstSlides;
        }

        #endregion
    }
}