using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class PatronService : IPatron
    {
        private LibraryContext _context;

        public PatronService(LibraryContext context)
        {
            _context = context;
        }
        public void Add(Patron newPatron)
        {
            _context.Add(newPatron);
            _context.SaveChanges();
        }

        public Patron Get(int id)
        {
            return 
                GetAll()
                .FirstOrDefault(patron => patron.Id == id);    
        }

        public IEnumerable<Patron> GetAll()
        {
            return _context.Patrons
                 .Include(patron => patron.LibraryCard)
                 .Include(patron => patron.HomeLibraryBranch);
        }

        public IEnumerable<Checkout> GetCheckout(int patronId)
        {
            var patronCardId = Get(patronId).LibraryCard.Id;

            return _context.Checkouts
                .Include(a => a.LibraryCard)
                .Include(a => a.LibraryAsset)
                .Where(v => v.LibraryCard.Id == patronCardId);
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int patronId)
        {
            var cardId = GetLibraryIdCard(patronId);

            return _context.CheckoutHistories
                .Include(co => co.LibraryCard)
                .Include(co => co.LibraryAsset)
                .Where(co => co.LibraryCard.Id == cardId)
                .OrderByDescending(co => co.CheckedOut);

        }

        private int? GetLibraryIdCard(int patronId)
        {
            var CardId = _context.Patrons
               .Include(a => a.LibraryCard)
               .FirstOrDefault(a => a.Id == patronId)?
               .LibraryCard.Id;

            return CardId;
        }

        public IEnumerable<Hold> GetHolds(int patronId)
        {
            var cardId = GetLibraryIdCard(patronId);

            return _context.Holds
                .Include(a => a.LibraryCard)
                .Include(a => a.LibraryAsset)
                .Where(a => a.LibraryCard.Id == cardId)
                .OrderByDescending(a => a.HoldPlaced);
        }
    }
}
