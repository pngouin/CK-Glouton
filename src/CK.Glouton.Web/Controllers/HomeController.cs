using Microsoft.AspNetCore.Mvc;

namespace CK.Glouton.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return new VirtualFileResult( "~/index.html", "text/html" );
        }
    }
}