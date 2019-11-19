using System.Web.Mvc;
using MVC.Models;
using N2.Web.Mvc;
using N2.Web;

namespace RavenTest.Controllers
{
	[Controls(typeof(Page))]
    public class PageController : ContentController<Page>
    {
        //
        // GET: /{page.Url}/

        public override ActionResult Index()
        {
            return View(CurrentPage);
        }

    }
}
