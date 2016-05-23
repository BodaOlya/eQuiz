using eQuiz.Web.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        [HttpGet]       
        public JsonResult GetStudentComments(int id)
        {
            var studentComments = new List<object>();

            studentComments.Add(new
            {
                date = "15.04.2016",
                author = "Volodymyr",
                text = "Responsible, initiative student with excellent knowledge of necessary frameworks"
            });
            studentComments.Add(new
            {
                date = "03.05.2016",
                author = "Ivan",
                text = "Demonstrates deep theoretical knowledge"
            });

            return Json(studentComments, JsonRequestBehavior.AllowGet);
        }
    }
}