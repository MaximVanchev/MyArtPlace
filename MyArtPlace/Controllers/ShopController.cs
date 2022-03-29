using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyArtPlace.Controllers
{
    [Authorize]
    public class ShopController : Controller
    {
    }
}
