using Library.Models.Catalog;
using Library.Models.CheckOut;
using LibraryData;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Library.Controllers
{
    public class CatalogController:Controller
    {
        //constructor Injection
        private ILibraryAsset _assets;
        private ICheckOut _checkouts;

        public CatalogController(ILibraryAsset assets, ICheckOut checkouts)
        {
            _assets = assets;
            _checkouts = checkouts;
        }

        public IActionResult Index()
        {
            var assetModels = _assets.GetAll();

            var listingResult = assetModels
                .Select(result => new AssetIndexListingModel
                {
                    Id = result.id, 
                    ImageUrl = result.ImageUrl,
                    AuthorOrDirector = _assets.GetAuthorOrDirector(result.id),
                    DeweyCallNumber = _assets.GetDeweyIndex(result.id),
                    Title = result.Title,
                    Type = _assets.GetType(result.id)
                });

            var model = new AssetIndexModel()
            {
                Assets = listingResult
            };

            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var asset = _assets.GetById(id);
            var currentHolds = _checkouts.GetCurrentHolds(id)
                .Select(a => new AssetHoldModel
                {
                    HoldPlaced = _checkouts.GetCurrentHoldPlaced(a.id).ToString("d"),
                    PatronName = _checkouts.getCurrentHoldPatronName(a.id)

                });


            var model = new AssetDetailModel
            {
                AssetId = id,
                Title = asset.Title,
                Type = _assets.GetType(id),
                Year = asset.Year,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                ImageUrl = asset.ImageUrl,
                AuthorDirector = _assets.GetAuthorOrDirector(id),
                CurrentLocation = _assets.GetCurrentLocation(id).Name,
                DeweyCallNumber = _assets.GetDeweyIndex(id),
                ISBN = _assets.GetIsbn(id),
                checkoutHistory = _checkouts.GetCheckoutHistory(id),
                PatronName = _checkouts.GetCurrentCheckoutPatron(id),
                LatestCheckout = _checkouts.GetLatestCheckout(id),
                CurrentHolds = currentHolds
            };

            return View(model);
        }

        
        public IActionResult CheckOut(int id)
        {
            var asset = _assets.GetById(id);
            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkouts.IsCheckedOut(id)

            };
            return View(model);
        }

        public IActionResult Checkin (int id)
        {
            _checkouts.CheckInItem(id);
            return RedirectToAction("Detail", new { id = id });
        }

        public IActionResult Hold(int id)
        {
            var asset = _assets.GetById(id);
            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkouts.IsCheckedOut(id),
                HoldCount = _checkouts.GetCurrentHolds(id).Count()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult MarkLost(int assetId)
        {
            _checkouts.MarkLost(assetId);
            return RedirectToAction("Detail", new { id = assetId });

        }

        [HttpPost]
        public IActionResult MarkFound(int assetId)
        {
            _checkouts.MarkFound(assetId);
            return RedirectToAction("Detail", new { id = assetId });

        }

        [HttpPost]
        public IActionResult PlaceCheckOut(int assetId, int LibraryCardId)
        {
            _checkouts.CheckOutItem(assetId, LibraryCardId);
            return RedirectToAction("Detail", new { id = assetId});

        }

        [HttpPost]
        public IActionResult PlaceHold(int assetId, int LibraryCardId)
        {
            _checkouts.PlaceHold(assetId, LibraryCardId);
            return RedirectToAction("Detail", new { id = assetId });

        }

    }
}
