using Basiscore.Stopgap.Models;
using Basiscore.Stopgap.Utilities;
using HtmlAgilityPack;
using Sitecore.Data.Items;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Basiscore.Stopgap.Controllers
{
    public class StopgapController : Controller
    {
        public ActionResult RenderStopgapSection()
        {
            StopgapDatasource viewModel = null;

            try
            {
                Item datasourceItem = SitecoreUtility.GetRenderingDatasourceItem();

                if (datasourceItem != null && datasourceItem.TemplateID == Templates.StopgapDatasource.ID)
                {
                    viewModel = new StopgapDatasource(datasourceItem);

                    if (viewModel.IsActive)
                    {
                        string markup = viewModel.HTML;

                        if (!string.IsNullOrEmpty(markup) && viewModel.InnerItem.HasChildren)
                        {
                            markup = markup.Replace("{{", "<stopgap>").Replace("}}", "</stopgap>");
                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(markup);
                            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//stopgap");

                            if (nodes != null && nodes.Count > 0)
                            {
                                Item datasourceChildItem = null;

                                foreach (HtmlNode node in nodes)
                                {
                                    try
                                    {
                                        datasourceChildItem = viewModel.InnerItem.Children.Where(x => x.Name.ToLower() == node.InnerText.ToLower()).FirstOrDefault();
                                    }
                                    catch (Exception)
                                    {

                                    }

                                    if (datasourceChildItem != null)
                                    {

                                    }
                                }
                            }
                            foreach (Item item in viewModel.InnerItem.Children)
                            {

                            }

                        }

                        viewModel.OutputHtml = new MvcHtmlString(markup);
                    }
                    else
                    {
                        viewModel = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Stopgap Section", ex, this);
            }

            return View("~/Views/Basiscore/Stopgap/StopgapSection.cshtml", viewModel);
        }
    }
}