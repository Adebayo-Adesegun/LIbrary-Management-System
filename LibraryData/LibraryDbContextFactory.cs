using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace LibraryData
{
   public class LibraryDbContextFactory: IDesignTimeDbContextFactory<LibraryContext>
    {
       // public IConfiguration Configuration;
        public LibraryContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<LibraryContext>();
            string newpath = Path.Combine(Environment.CurrentDirectory,
                                      @"..\Library\");
            
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(newpath)
            .AddJsonFile("appsettings.json")
            .Build();
            builder.UseSqlServer(configuration.GetConnectionString("LibraryConnection"));

            return new LibraryContext(builder.Options);
        }

    }
}
