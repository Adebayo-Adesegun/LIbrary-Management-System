

using LibraryData.Models;
using System.Collections.Generic;

namespace LibraryData
{
   public interface IPatron
    {
        Patron Get(int id);
        void Add(Patron newPatron);



        IEnumerable<Patron> GetAll();
        IEnumerable<CheckoutHistory> GetCheckoutHistory(int patronId);
        IEnumerable<Hold> GetHolds(int patronId);
        IEnumerable<Checkout> GetCheckout(int patronId);

    }
}
