using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CommerceTraining.Models.Catalog;
using CommerceTraining.Models.Pages;
using CommerceTraining.Models.ViewModels;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Globalization;
using EPiServer.Security;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Mediachase.Commerce;
using Mediachase.Commerce.Security;

namespace CommerceTraining.Controllers
{
    public class ShirtVariationController : CatalogControllerBase<ShirtVariation>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILineItemValidator _lineItemValidator;
        private readonly ICurrentMarket _currentMarket;

        public ShirtVariationController(IContentLoader contentLoader, UrlResolver urlResolver,
            AssetUrlResolver assetUrlResolver, ThumbnailUrlResolver thumbnailUrlResolver,
            IOrderRepository orderRepository, ILineItemValidator lineItemValidator, ICurrentMarket currentMarket) : base(contentLoader, urlResolver, assetUrlResolver, thumbnailUrlResolver)
        {
            _orderRepository = orderRepository;
            _lineItemValidator = lineItemValidator;
            _currentMarket = currentMarket;
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

        public ActionResult AddToCart(ShirtVariation currentContent, decimal Quantity, string Monogram)
        {
            // ToDo: (lab D1) add a LineItem to the Cart
            var cart = _orderRepository.LoadOrCreateCart<ICart>(PrincipalInfo.CurrentPrincipal.GetContactId(), "Default");
            var item = cart.GetAllLineItems().Where(i => i.Code == currentContent.Code).FirstOrDefault();
            if (item == null)
            {
                item = cart.CreateLineItem(currentContent.Code);
                item.Quantity = Quantity;
                cart.AddLineItem(item);
            }
            else
            {
                item.Quantity += Quantity;
            }

            var validLineItem = _lineItemValidator.Validate(item, _currentMarket.GetCurrentMarket().MarketId, (li, issue) => { });

            if (validLineItem)
            {
                item.Properties["Monogram"] = Monogram;
                _orderRepository.Save(cart);
            }
            
            // if we want to redirect
            ContentReference cartRef = _contentLoader.Get<StartPage>(ContentReference.StartPage).Settings.cartPage;
            CartPage cartPage = _contentLoader.Get<CartPage>(cartRef);
            var name = cartPage.Name;
            var lang = ContentLanguage.PreferredCulture;
            string passingValue = cart.Name;

            // go to the cart page, if needed
            return RedirectToAction("Index", lang + "/" + name, new { passedAlong = passingValue });
        }


        public void AddToWishList(ShirtVariation currentContent)
        {

        }
    }
}