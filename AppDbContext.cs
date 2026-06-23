using Microsoft.EntityFrameworkCore;
using CSVImportExportApp.Models;

namespace CSVImportExportApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=CsvImportDb;Trusted_Connection=True;");
        }
    }
}