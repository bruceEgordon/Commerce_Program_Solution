using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CommerceTraining.Models.Catalog;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using Mediachase.Commerce.Security;

namespace CommerceTraining.Controllers
{
    public class FashionNodeController : CatalogControllerBase<FashionNode>
    {
        public ActionResult Index(FashionNode currentContent)
        {
            /* Implementation of action. You can create your own view model class that you pass to the view or
             * you can pass the page type for simpler templates */

            return View(currentContent);
        }
    }
}