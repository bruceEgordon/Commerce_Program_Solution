using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;

namespace CommerceTraining.Controllers
{
    public class CatalogControllerBase<T> : ContentController<T> where T : CatalogContentBase
    {
        private readonly IContentLoader _contentLoader;
        private readonly UrlResolver _urlResolver;
        private readonly AssetUrlResolver _assetUrlResolver;
        private readonly ThumbnailUrlResolver _thumbnailUrlResolver;

        public CatalogControllerBase(IContentLoader contentLoader, UrlResolver urlResolver,
            AssetUrlResolver assetUrlResolver, ThumbnailUrlResolver thumbnailUrlResolver)
        {
            _contentLoader = contentLoader;
            _urlResolver = urlResolver;
            _assetUrlResolver = assetUrlResolver;
            _thumbnailUrlResolver = thumbnailUrlResolver;
        }
    }
}