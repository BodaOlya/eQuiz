using eQuiz.Entities;
using eQuiz.Web.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eQuiz.Web.Areas.Moderator.Controllers
{
    public class UserGroupController : BaseController
    {
        public ActionResult Get()
        {
            UserGroup[] groups = null;
            using (eQuizEntities model = new eQuizEntities(System.Configuration.ConfigurationManager.ConnectionStrings["eQuizDB"].ConnectionString))
            {
                groups = model.UserGroups.ToArray();
                return Json(groups, JsonRequestBehavior.AllowGet);
            }
            
        }
    }
}