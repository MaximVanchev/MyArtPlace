using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyArtPlace.Areas.Identity.Data;
using MyArtPlace.Infrastructure.Data;

namespace MyArtPlace.Data;

public class MyArtPlaceContext : IdentityDbContext<MyArtPlaceUser>
{
    public MyArtPlaceContext(DbContextOptions<MyArtPlaceContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UsersCart>().HasKey(x => new { x.UserId, x.ProductId });
    }

    public DbSet<UsersCart> UsersCarts { get; set; }

    public DbSet<Shop> Shops { get; set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<Currency> Currencies { get; set; }

    public DbSet<Category> Categories { get; set; }

}
