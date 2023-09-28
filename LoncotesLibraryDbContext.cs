using Microsoft.EntityFrameworkCore;
using LoncotesLibrary.Models;

public class LoncotesLibraryDbContext : DbContext
{

    public DbSet<Genre> Genres { get; set; }
    public DbSet<Patron> Patrons { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<MaterialType> MaterialTypes { get; set; }
    public DbSet<Checkout> Checkouts { get; set; }

    public LoncotesLibraryDbContext(DbContextOptions<LoncotesLibraryDbContext> context) : base(context)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Genre>().HasData(new Genre[]
        {
            new() {Id = 1, Name = "Cosmic Horror"},
            new() {Id = 2, Name = "Historical Fiction"},
            new() {Id = 3, Name = "Mystery"},
            new() {Id = 4, Name = "Science Fiction"},
            new() {Id = 5, Name = "Fantasy"},
            new() {Id = 6, Name = "Health"}
        });
        modelBuilder.Entity<Patron>().HasData(new Patron[]
        {
            new() {Id = 1, FirstName = "Ryan", LastName = "Mathis", Address = "123 Main St", Email = "Ryan@Mail.com", IsActive = true},
            new() {Id = 2, FirstName = "Simone", LastName = "Henderson", Address = "456 Second Ave", Email = "Simone@Mail.com", IsActive = false}
        });
        modelBuilder.Entity<MaterialType>().HasData(new MaterialType[]
        {
            new() {Id = 1, Name = "Book", CheckoutDays = 7},
            new() {Id = 2, Name = "CD", CheckoutDays = 5},
            new() {Id = 3, Name = "Periodical", CheckoutDays = 3}
        });
        modelBuilder.Entity<Material>().HasData(new Material[]
        {
            new() {Id = 1, MaterialName = "At The Mountains Of Madness", MaterialTypeId = 1, GenreId = 1, OutOfCirculationSince = null},
            new() {Id = 2, MaterialName = "The Way of Kings", MaterialTypeId = 1, GenreId = 5, OutOfCirculationSince = null},
            new() {Id = 3, MaterialName = "Dune", MaterialTypeId = 1, GenreId = 4, OutOfCirculationSince = null},
            new() {Id = 4, MaterialName = "GQ", MaterialTypeId = 3, GenreId = 6, OutOfCirculationSince = null},
            new() {Id = 5, MaterialName = "The Lord Of The Rings: The Fellowship of the Ring", MaterialTypeId = 1, GenreId = 5, OutOfCirculationSince = null},
            new() {Id = 6, MaterialName = "The Lord Of The Rings: The Two Towers", MaterialTypeId = 1, GenreId = 5, OutOfCirculationSince = null},
            new() {Id = 7, MaterialName = "The Lord Of The Rings: The Return of the King", MaterialTypeId = 1, GenreId = 5, OutOfCirculationSince = null},
            new() {Id = 8, MaterialName = "The Lord Of The Rings: The Fellowship of the Ring", MaterialTypeId = 2, GenreId = 5, OutOfCirculationSince = null},
            new() {Id = 9, MaterialName = "The Lord Of The Rings: The Two Towers", MaterialTypeId = 2, GenreId = 5, OutOfCirculationSince = null},
            new() {Id = 10, MaterialName = "The Lord Of The Rings: The Return of the King", MaterialTypeId = 2, GenreId = 5, OutOfCirculationSince = null},
            new() {Id = 11, MaterialName = "Encyclopedia Brown Takes the Case", MaterialTypeId = 1, GenreId = 3, OutOfCirculationSince = new DateTime(1994, 08, 25)},
            new() {Id = 12, MaterialName = "I, Claudius", MaterialTypeId = 1, GenreId = 2, OutOfCirculationSince = null}
        });
        modelBuilder.Entity<Checkout>().HasData(new Checkout[]
        {
            new() {Id = 1, MaterialId = 5, PatronId = 1, CheckoutDate = new DateTime(2023, 08, 23), ReturnDate = new DateTime(2023, 08, 29)},
            new() {Id = 2, MaterialId = 12, PatronId = 2, CheckoutDate = new DateTime(2023, 09, 27), ReturnDate = null},
        });
    }
}