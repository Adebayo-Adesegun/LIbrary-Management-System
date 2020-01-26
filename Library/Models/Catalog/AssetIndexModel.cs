using System;
using System.Collections.Generic;

namespace Library.Models.Catalog
{
    public class AssetIndexModel
    {
        //Wrpas the collectionof AssetListing model, althoght 
        //the collection could have been easily passed down to the view
        public IEnumerable<AssetIndexListingModel> Assets { get; set; }
    }
}
