using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyArtPlace.Areas.Identity.Data;

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
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
