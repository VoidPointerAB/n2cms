using N2;
using N2.Definitions;
using N2.Definitions.Runtime;
using N2.Web.UI;

namespace MVC.Models
{
    public class Page : ContentItem, IStartPage
    {
    }

    //[Registration]
    //public class StartPageRegisterer : FluentRegisterer<StartPage>
    //{
    //    public override void RegisterDefinition(IContentRegistration<StartPage> register)
    //    {
    //        register.Page(title: "Start Page");
    //        register.UsingConventions();
    //    }
    //}
}
