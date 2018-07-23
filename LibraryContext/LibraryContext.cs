using Microsoft.EntityFrameworkCore;
using LibraryData;


namespace LibraryData
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions options) : base(options) { }

       // public DbSet<Patron> Patrons { get; set; }


    }
}
