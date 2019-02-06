using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CommerceTraining.SupportingClasses;
using EPiServer;
using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Filters;
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

        public string GetDefaultAsset(IAssetContainer assetContainer)
        {
            return _assetUrlResolver.GetAssetUrl(assetContainer);
        }

        public string GetNamedAsset(IAssetContainer assetContainer, string groupName)
        {
            return _thumbnailUrlResolver.GetThumbnailUrl(assetContainer, groupName);
        }

        public string GetUrl(ContentReference contentReference)
        {
            return _urlResolver.GetUrl(contentReference);
        }
        
        public List<NameAndUrls> GetNodes(ContentReference contentReference)
        {
            var nodes = _contentLoader.GetChildren<NodeContent>(contentReference);
            var filteredNodes = FilterForVisitor.Filter(nodes);
            var result = new List<NameAndUrls>();
            foreach(var node in filteredNodes)
            {
                var item = new NameAndUrls
                {
                    name = node.Name,
                    url = GetUrl(node.ContentLink),
                    imageUrl = GetDefaultAsset(node as NodeContent),
                    imageThumbUrl = GetNamedAsset(node as NodeContent, "Thumbnail")
                };
                result.Add(item);
            }
            return result;
        }

        public List<NameAndUrls> GetEntries(ContentReference contentReference)
        {
            var nodes = _contentLoader.GetChildren<EntryContentBase>(contentReference);
            var filteredNodes = FilterForVisitor.Filter(nodes);
            var result = new List<NameAndUrls>();
            foreach (var node in filteredNodes)
            {
                var item = new NameAndUrls
                {
                    name = node.Name,
                    url = GetUrl(node.ContentLink),
                    imageUrl = GetDefaultAsset(node as EntryContentBase),
                    imageThumbUrl = GetNamedAsset(node as EntryContentBase, "Thumbnail")
                };
                result.Add(item);
            }
            return result;
        }
    }
}