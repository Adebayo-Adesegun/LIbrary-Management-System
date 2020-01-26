using LibraryData.Models;
using System;
using System.Collections.Generic;

namespace LibraryData
{
    public class LibraryCard
    {
        public int Id { get; set; }
        public decimal Fees { get; set; }
        public DateTime Created { get; set; } 
        public ICollection<Checkout> Checkouts { get; set; }

    }
}