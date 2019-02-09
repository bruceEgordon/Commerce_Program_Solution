using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using CommerceTraining.Models.Pages;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce;
using Mediachase.Commerce.Website.Helpers;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Engine;
using System;
using EPiServer.Security;
using Mediachase.Commerce.Customers;
using EPiServer.ServiceLocation;
using CommerceTraining.Models.ViewModels;
using EPiServer.Commerce.Order;
using EPiServer.Commerce.Marketing;
using Mediachase.Data.Provider;

// for the extension-method
using Mediachase.Commerce.Security;
using EPiServer.Commerce.Order.Calculator;
using Mediachase.Commerce.InventoryService;
using Mediachase.Commerce.Inventory;

namespace CommerceTraining.Controllers
{
    public class CheckOutController : PageController<CheckOutPage>
    {

        private const string DefaultCart = "Default";

        private readonly IContentLoader _contentLoader; // To get the StartPage --> Settings-links
        private readonly ICurrentMarket _currentMarket; // not in fund... yet
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderGroupFactory _orderGroupFactory;
        private readonly IPaymentProcessor _paymentProcessor;
        private readonly IPromotionEngine _promotionEngine;
        private readonly IOrderGroupCalculator _orderGroupCalculator;
        private readonly ILineItemCalculator _lineItemCalculator;
        private readonly IInventoryProcessor _inventoryProcessor;
        private readonly ILineItemValidator _lineItemValidator;
        private readonly IPlacedPriceProcessor _placedPriceProcessor;

        public CheckOutController(IContentLoader contentLoader
    , ICurrentMarket currentMarket
    , IOrderRepository orderRepository
    , IPlacedPriceProcessor placedPriceProcessor
    , IInventoryProcessor inventoryProcessor
    , ILineItemValidator lineItemValidator
    , IOrderGroupCalculator orderGroupCalculator
    , ILineItemCalculator lineItemCalculator
    , IOrderGroupFactory orderGroupFactory
    , IPaymentProcessor paymentProcessor
    , IPromotionEngine promotionEngine)
        {
            _contentLoader = contentLoader;
            _currentMarket = currentMarket;
            _orderRepository = orderRepository;
            _orderGroupCalculator = orderGroupCalculator;
            _orderGroupFactory = orderGroupFactory;
            _paymentProcessor = paymentProcessor;
            _promotionEngine = promotionEngine;
            _lineItemCalculator = lineItemCalculator;
            _inventoryProcessor = inventoryProcessor;
            _lineItemValidator = lineItemValidator;
            _placedPriceProcessor = placedPriceProcessor;
        }

        // ToDo: in the first exercise (E1) Ship & Pay
        public ActionResult Index(CheckOutPage currentPage)
        {
            // Try to load the cart  

            var model = new CheckOutViewModel(currentPage)
            {
                PaymentMethods = GetPaymentMethods(),
                ShipmentMethods = GetShipmentMethods(),
                ShippingRates = GetShippingRates()
            };

            return View(model);
        }


        //Exercise (E2) Do CheckOut
        public ActionResult CheckOut(CheckOutViewModel model)
        {
            var cart = _orderRepository.LoadCart<ICart>(PrincipalInfo.CurrentPrincipal.GetContactId(), "Default");

            if (cart == null) throw new ApplicationException("No cart");

            IOrderAddress orderAddr = AddAddressToOrder(cart);


            // ToDo: Define/update Shipping
            AdjustFirstShipmentInOrder(cart, orderAddr, model.SelectedShipId);

            // ToDo: Add a Payment to the Order 
            AddPaymentToOrder(cart, model.SelectedPayId);

            // ToDo: Add a transaction scope and convert the cart to PO


            // ToDo: Housekeeping (Statuses for Shipping and PO, OrderNotes and save the order)


            // Final steps, navigate to the order confirmation page
            StartPage home = _contentLoader.Get<StartPage>(ContentReference.StartPage);
            ContentReference orderPageReference = home.Settings.orderPage;

            // the below is a dummy, change to "PO".OrderNumber when done
            string passingValue = String.Empty;

            return RedirectToAction("Index", new { node = orderPageReference, passedAlong = passingValue });
        }

        private IEnumerable<PaymentMethodDto.PaymentMethodRow> GetPaymentMethods()
        {
            return PaymentManager.GetPaymentMethodsByMarket(_currentMarket.GetCurrentMarket().MarketId.Value).PaymentMethod;
        }

        private IEnumerable<ShippingMethodDto.ShippingMethodRow> GetShipmentMethods()
        {
            return ShippingManager.GetShippingMethodsByMarket(_currentMarket.GetCurrentMarket().MarketId.Value, false).ShippingMethod;
        }

        private IEnumerable<ShippingRate> GetShippingRates()
        {
            var shippingRates = new List<ShippingRate>();
            var shipMethods = GetShipmentMethods();
            foreach(var method in shipMethods)
            {
                var shipRate = new ShippingRate(method.ShippingMethodId, method.Name, new Money(method.BasePrice, method.Currency));
                shippingRates.Add(shipRate);
            }
            return shippingRates;
        }
        // Prewritten 
        private string ValidateCart(ICart cart)
        {
            var validationMessages = string.Empty;

            cart.ValidateOrRemoveLineItems((item, issue) =>
                validationMessages += CreateValidationMessages(item, issue), _lineItemValidator);

            cart.UpdatePlacedPriceOrRemoveLineItems(GetContact(), (item, issue) =>
                validationMessages += CreateValidationMessages(item, issue), _placedPriceProcessor);

            cart.UpdateInventoryOrRemoveLineItems((item, issue) =>
                validationMessages += CreateValidationMessages(item, issue), _inventoryProcessor);

            return validationMessages; 
        }

        private static string CreateValidationMessages(ILineItem item, ValidationIssue issue)
        {
            return string.Format("Line item with code {0} had the validation issue {1}.", item.Code, issue);
        }

        private void AdjustFirstShipmentInOrder(ICart cart, IOrderAddress orderAddress, Guid selectedShip)
        {
            IShipment shipment = cart.GetFirstShipment();
            shipment.ShippingMethodId = selectedShip;
            shipment.ShippingAddress = orderAddress;
            shipment.ShipmentTrackingNumber = "ABC123";
        }

        private void AddPaymentToOrder(ICart cart, Guid selectedPaymentGuid)
        {
            var payMethod = PaymentManager.GetPaymentMethod(selectedPaymentGuid);
            var payment = _orderGroupFactory.CreatePayment(cart);
            payment.PaymentMethodId = selectedPaymentGuid;
            payment.PaymentMethodName = payMethod.PaymentMethod[0].Name;
            payment.Amount = _orderGroupCalculator.GetTotal(cart);
            cart.AddPayment(payment);
        }

        private IOrderAddress AddAddressToOrder(ICart cart)
        {
            IOrderAddress shippingAddress = null;

            if (CustomerContext.Current.CurrentContact == null)
            {
                var shipment = cart.GetFirstShipment();
                if(shipment.ShippingAddress != null)
                {
                    //return something and maybe do cleanup
                }
                shippingAddress = _orderGroupFactory.CreateOrderAddress(cart);
                shippingAddress.CountryName = "USA";
                shippingAddress.Id = "DemoAddress";
                shippingAddress.Email = "Homer@acme.com";
                shippingAddress.City = "Springfield";
            }
            else
            {

            }

            return shippingAddress;
        }

        private static CustomerContact GetContact()
        {
            return CustomerContext.Current.GetContactById(GetContactId());
        }

        private static Guid GetContactId()
        {
            return PrincipalInfo.CurrentPrincipal.GetContactId();
        }
    }
}