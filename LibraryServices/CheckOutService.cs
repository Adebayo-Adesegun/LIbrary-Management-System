using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class CheckOutService : ICheckOut
    {
        private LibraryContext _context;
        public CheckOutService(LibraryContext context)
        {
            _context = context;
        }
        public void Add(Checkout newCheckout)
        {
            _context.Add(newCheckout);
            _context.SaveChanges();
        }

        public IEnumerable<Checkout> GetAll()
        {
            return _context.Checkouts;
        }

        public Checkout GetById(int checkoutId)
        {
            return GetAll()
                .FirstOrDefault(checkout => checkout.Id == checkoutId);
                
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int id)
        {
            return _context.CheckoutHistories
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.LibraryAsset.id == id);
        }

        public IEnumerable<Hold> GetCurrentHolds(int id)
        {
            return _context.Holds
                .Include(h => h.LibraryAsset)
                .Where(h => h.LibraryAsset.id == id);

        }
        public Checkout GetLatestCheckout(int assetId)
        {
            return _context.Checkouts
                 .Where(c => c.LibraryAsset.id == assetId)
                 .OrderByDescending(c => c.Since)
                 .FirstOrDefault();
        }


        public void MarkFound(int assetId)
        {
            UpdateAssetStatus(assetId, "Available");

            //remove any existing checkouts on the item 
            RemoveExistingConnections(assetId);

            //close any existing checkout history 
            CloseExistingCheckout(assetId);

            _context.SaveChanges();
        }

        private void UpdateAssetStatus(int assetId, string v)
        {
            var item = _context.LibraryAssets
               .FirstOrDefault(a => a.id == assetId);

            _context.Update(item);
            item.Status = _context.Statuses.FirstOrDefault(status
                => status.Name == v );
        }

        private void CloseExistingCheckout(int assetId)
        {
            var now = DateTime.Now;
            var history = _context.CheckoutHistories
               .FirstOrDefault(h => h.LibraryAsset.id == assetId
                     && h.CheckedIn == null);

            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }
        }

        private void RemoveExistingConnections(int assetId)
        {
            var checkout = _context.Checkouts.
                FirstOrDefault(co => co.LibraryAsset.id == assetId);

            if (checkout != null)
            {
                _context.Remove(checkout);
            }
        }

        public void MarkLost(int assetId)
        {
            UpdateAssetStatus(assetId, "Lost");
            _context.SaveChanges();
        }

        public void PlaceHold(int assetId, int LibraryCardId)
        {
            var now = DateTime.Now;
            var asset = _context.LibraryAssets
                .Include(a => a.Status)
                .FirstOrDefault(a => a.id == assetId);
            var card = _context.LibraryCards
                .FirstOrDefault(c => c.Id == LibraryCardId);

            if (asset.Status.Name == "Available")
            {
                UpdateAssetStatus(assetId, "On Hold");
            }

            var hold = new Hold
            {
                HoldPlaced = now,
                LibraryAsset = asset,
                LibraryCard = card
            };

            _context.Add(hold);
            _context.SaveChanges();
        }

        public void CheckInItem(int assetId)
        {
            var now = DateTime.Now;
            var item = _context.LibraryAssets
                .FirstOrDefault(a => a.id == assetId);
            _context.Update(item);



            // remove any existing checkouts on the item
            RemoveExistingConnections(assetId);
            // close any existing checkout history
            CloseExistingCheckout(assetId);
            // look for existing holds on the item
            var currentHolds = _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.LibraryAsset.id == assetId);
            // if there are holds, checkout the item to the library card with the 
            // earliest hold,
            if (currentHolds.Any())
            {
                CheckoutToEarliestHold(assetId, currentHolds);
                return;
            }
            //  otherwise, update item status to available
            UpdateAssetStatus(assetId, "Available");
            _context.SaveChanges();

        }

        private void CheckoutToEarliestHold(int assetId, IQueryable<Hold> currentHolds)
        {
            var earliestHold = currentHolds
                .OrderBy(holds => holds.HoldPlaced)
                .FirstOrDefault();

            var card = earliestHold.LibraryCard;


            _context.Remove(earliestHold);
            _context.SaveChanges();

            CheckOutItem(assetId, card.Id);
              
        }

        public void CheckOutItem(int assetId, int LibraryCardId)
        {
            var now = DateTime.Now;
            if (IsCheckedOut(assetId))
            {
                return;
                //Add logic to handle feedback to the user
            }

            var item = _context.LibraryAssets
                .FirstOrDefault(a => a.id == assetId);

            UpdateAssetStatus(assetId, "Checked Out");

            var libraryCard = _context.LibraryCards
                .Include(card => card.Checkouts)
                .FirstOrDefault(card => card.Id == LibraryCardId);

            //create new checkout 
            var checkout = new Checkout
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                Since = now,
                Until = GetDefaultCheckoutTime(now)

            };

            _context.Add(checkout);
            var checkoutHistory = new CheckoutHistory
            {
                CheckedOut = now,
                LibraryAsset = item,
                LibraryCard = libraryCard
            };

            _context.Add(checkoutHistory);
            _context.SaveChanges();

        }

        private DateTime GetDefaultCheckoutTime(DateTime now)
        {
            return now.AddDays(30);
        }

        public bool IsCheckedOut(int assetId)
        {
            return _context.Checkouts.Where(co => co.LibraryAsset.id == assetId).Any();

        }

        public string getCurrentHoldPatronName(int holdid)
        {
            var hold = _context.Holds
                 .Include(h => h.LibraryAsset)
                 .Include(h => h.LibraryCard)
                 .FirstOrDefault(h => h.id == holdid);

            //Get the card id from the hold
            var cardId = hold?.LibraryCard.Id;
            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return patron?.FirstName + " " + patron?.LastName;

        }

        public DateTime GetCurrentHoldPlaced(int holdId)
        {
            return _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h => h.id == holdId)
                .HoldPlaced;  
        }

        public string GetCurrentCheckoutPatron(int assetId)
        {
            var checkout = GetCheckOutByAssetId(assetId);

            if (checkout == null)
            {
                return "";
            };
            var cardId = checkout.LibraryCard.Id;

            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return patron.FirstName + " " + patron.LastName;
        }

        private Checkout GetCheckOutByAssetId(int assetId)
        {
            return _context.Checkouts
                    .Include(h => h.LibraryAsset)
                    .Include(h => h.LibraryCard)
                    .FirstOrDefault(co => co.LibraryAsset.id == assetId);
        }

      
    }
}

