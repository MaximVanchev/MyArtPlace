using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MyArtPlace.Infrastructure.Data;
using MyArtPlace.Infrastructure.Data.Constants;

namespace MyArtPlace.Areas.Identity.Data;

// Add profile data for application users by adding properties to the MyArtPlaceUser class
public class MyArtPlaceUser : IdentityUser
{
    public MyArtPlaceUser()
    {
        LikedProducts = new List<Product>();
        CartProducts = new List<UsersCart>();
    }

    [StringLength(DatabaseConstants.Guid_Max_Length)]
    public Guid? ShopId { get; set; }

    [ForeignKey(nameof(ShopId))]
    public Shop? Shop { get; set; }

    [StringLength(DatabaseConstants.ProfilePicture_Max_Length)]
    public string? ProfilePicture { get; set; }

    public IList<Product> LikedProducts { get; set; }

    public IList<UsersCart> CartProducts { get; set; }
}

