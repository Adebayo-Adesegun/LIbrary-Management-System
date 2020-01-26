using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryData
{
    public interface ICheckOut
    {
        IEnumerable<Checkout> GetAll();
        IEnumerable<Hold> GetCurrentHolds(int id);
        IEnumerable<CheckoutHistory> GetCheckoutHistory(int id);



        void Add(Checkout newCheckout);
        void CheckOutItem(int assetId, int LibraryCardId);
        void CheckInItem(int assetId);
        void PlaceHold(int assetId, int LibraryCardId);
        void MarkLost(int assetId);
        void MarkFound(int assetId);


        string GetCurrentCheckoutPatron(int assetId);
        string getCurrentHoldPatronName(int id);
        DateTime GetCurrentHoldPlaced(int id);
        Checkout GetById(int checkoutId);
        Checkout GetLatestCheckout(int assetId);
        bool IsCheckedOut(int id);
    }
}
