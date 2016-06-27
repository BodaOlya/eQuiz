using eQuiz.Web.Code;
using System.Web.Mvc;

namespace eQuiz.Web.Areas.Admin.Controllers
{
    public class DefaultController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        
    }
}