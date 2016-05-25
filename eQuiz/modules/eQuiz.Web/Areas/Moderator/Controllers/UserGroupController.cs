using eQuiz.Entities;
using eQuiz.Repositories.Abstract;
using eQuiz.Web.Code;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eQuiz.Web.Areas.Moderator.Controllers
{
    public class UserGroupController : BaseController
    {
        #region Fields

        private readonly IRepository _repository;

        #endregion

        #region Constructors

        public UserGroupController(IRepository repository)
        {
            this._repository = repository;
        }

        #endregion

        public ActionResult Get()
        {
            UserGroup[] groups = null;
            using (eQuizEntities model = new eQuizEntities(System.Configuration.ConfigurationManager.ConnectionStrings["eQuizDB"].ConnectionString))
            {
                groups = model.UserGroups.ToArray();
                return Json(groups, JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult Edit()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetUserGroup(int id)
        {
            List<User> users = new List<User>();
            var group = _repository.GetSingle<UserGroup>(g => g.Id == id);
            var groupUsers = _repository.Get<UserToUserGroup>(g => g.GroupId == id).ToList();
                        
            foreach(var user in groupUsers)
            {
                users.Add(_repository.GetSingle<User>(u => u.Id == user.UserId));
            }


            var data = JsonConvert.SerializeObject(new { group = group, users = users }, Formatting.None,
                                                    new JsonSerializerSettings()
                                                    {
                                                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                                    });


            return Content(data, "application/json");

        }
    }
}