using eQuiz.Entities;
using eQuiz.Repositories.Abstract;
using eQuiz.Web.Code;
using Newtonsoft.Json;
using System;
using System.Collections;
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
            IEnumerable<UserGroup> groups = _repository.Get<UserGroup>();
            var minGroups = new ArrayList();

            foreach (var group in groups)
            {
                minGroups.Add(GetUserGroupForSerialization(group));
            }

            return Json(minGroups, JsonRequestBehavior.AllowGet);
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

            var minGroups = new ArrayList();
            var minUsers = new ArrayList();

            var minGroup = GetUserGroupForSerialization(group);

            foreach (var user in groupUsers)
            {
                users.Add(_repository.GetSingle<User>(u => u.Id == user.UserId));
            }

            foreach (var user in users)
            {
                minUsers.Add(GetUsersForSerialization(user));
            }

            var result = new { group = minGroup, users = minUsers };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private object GetUserGroupForSerialization(UserGroup group)
        {
            var minGroup = new
            {
                Id = group.Id,
                Name = group.Name
            };

            return minGroup;
        }
        private object GetUsersForSerialization(User user)
         {           
             var minUser = new
             {
                 Id = user.Id,
                 LastName = user.LastName,
                 FirstName = user.FirstName,
                 FatheName = user.FatheName,
                 Email = user.Email                 
             };

            return minUser;
         }
    }
}