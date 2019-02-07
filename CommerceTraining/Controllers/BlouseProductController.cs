using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CommerceTraining.Models.Catalog;
using CommerceTraining.Models.Pages;
using CommerceTraining.Models.ViewModels;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;

namespace CommerceTraining.Controllers
{
    public class BlouseProductController : CatalogControllerBase<BlouseProduct>
    {
        public BlouseProductController(IContentLoader contentLoader, UrlResolver urlResolver, AssetUrlResolver assetUrlResolver, ThumbnailUrlResolver thumbnailUrlResolver) : base(contentLoader, urlResolver, assetUrlResolver, thumbnailUrlResolver)
        {
        }

        public ActionResult Index(BlouseProduct currentContent, StartPage currentPage)
        {
            var viewModel = new BlouseProductViewModel(currentContent, currentPage);

            viewModel.ProductVariations = _contentLoader.GetItems(currentContent.GetVariants(), new LoaderOptions()).OfType<EntryContentBase>();

            return View(viewModel);
        }
    }
}