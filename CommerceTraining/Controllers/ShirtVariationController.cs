using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CommerceTraining.Models.Catalog;
using CommerceTraining.Models.ViewModels;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Mediachase.Commerce.Security;

namespace CommerceTraining.Controllers
{
    public class ShirtVariationController : CatalogControllerBase<ShirtVariation>
    {
        public ShirtVariationController(IContentLoader contentLoader, UrlResolver urlResolver, AssetUrlResolver assetUrlResolver, ThumbnailUrlResolver thumbnailUrlResolver) : base(contentLoader, urlResolver, assetUrlResolver, thumbnailUrlResolver)
        {
        }

        public ActionResult Index(ShirtVariation currentContent)
        {
            var viewModel = new ShirtVariationViewModel
            {
                CanBeMonogrammed = currentContent.CanBeMonogrammed,
                image = GetDefaultAsset(currentContent),
                url = GetUrl(currentContent.ContentLink),
                MainBody = currentContent.MainBody,
                name = currentContent.Name,
                priceString = currentContent.GetDefaultPrice().UnitPrice.ToString("C")
            };

            return View(viewModel);
        }
    }
}