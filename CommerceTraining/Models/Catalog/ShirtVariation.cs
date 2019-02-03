using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Catalog.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;

namespace CommerceTraining.Models.Catalog
{
    [CatalogContentType(DisplayName = "Shirt Variation", GUID = "ff44e9d7-0979-4093-a403-37e1e4e79342",
        MetaClassName = "Shirt_Variation")]
    public class ShirtVariation : VariationContent
    {

        [CultureSpecific]
        [Display(
            Name = "Main body",
            Description = "The main body will be shown in the main content area of the page, using the XHTML-editor you can insert for example text, images and tables.",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        [IncludeInDefaultSearch]
        [Searchable]
        [Tokenize]
        public virtual XhtmlString MainBody { get; set; }

        [IncludeInDefaultSearch]
        public virtual string Size { get; set; }

        [IncludeInDefaultSearch]
        public virtual string Color { get; set; }
        public virtual bool CanBeMonogrammed { get; set; }

        [Searchable]
        [Tokenize]
        [IncludeValuesInSearchResults]
        public virtual string ThematicTag { get; set; }
    }
}