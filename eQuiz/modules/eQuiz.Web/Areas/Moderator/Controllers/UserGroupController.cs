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
        public ActionResult Create()
        {
            return RedirectToAction("Edit");
        }

        [HttpPost]
        public ActionResult Save(UserGroup userGroup, User[] users)
        {
            if (userGroup == null || users == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "No data to save");
            }
            int userGroupId = userGroup.Id;
            if (userGroupId != 0)
            {
                var existedUserGroup = _repository.GetSingle<UserGroup>(x => x.Id == userGroupId);
                if (existedUserGroup == null)
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "UserGroup is not found");
                }
                userGroupId = existedUserGroup.Id;
                existedUserGroup.Name = userGroup.Name;
                _repository.Update<UserGroup>(existedUserGroup);
                DeleteUsersFromGroupIfExist(userGroupId, users);
            }
            else
            {
                _repository.Insert<UserGroup>(userGroup);
                var id = _repository.GetSingle<UserGroup>(g => g.Name == userGroup.Name).Id;
                userGroupId = id;
                AddUsersToGroup(userGroupId, users);
            }

            return RedirectToAction("GetUserGroup", new { id = userGroupId });
        }

        private void AddUsersToGroup(int userGroupId, User[] users)
        {
            for (var i = 0; i < users.Length; i++)
            {
                var currentUser = users[i];
                var currentUserEmail = users[i].Email;
                var existedUser = _repository.GetSingle<User>(u => u.Email == currentUserEmail);
                if (existedUser != null)
                {
                    _repository.Insert<UserToUserGroup>(new UserToUserGroup { UserId = existedUser.Id, GroupId = userGroupId });
                }
                else
                {
                    _repository.Insert<User>(currentUser);
                    var currentUserId = _repository.GetSingle<User>(x => x.Email == currentUser.Email).Id;
                    _repository.Insert<UserToUserGroup>(new UserToUserGroup { UserId = currentUserId, GroupId = userGroupId });

                }
            }
        }

        private void DeleteUsersFromGroupIfExist(int userGroupId, User[] users)
        {
            var usersFromUserGroup = _repository.Get<UserToUserGroup>(x => x.GroupId == userGroupId);
            for (var i = 0; i < users.Length; i++)
            {
                int currentUserId = users[i].Id;
                UserToUserGroup matchedUser = _repository.GetSingle<UserToUserGroup>(u => u.UserId == currentUserId);
                if (matchedUser != null)
                {
                    UserToUserGroup userFromList = usersFromUserGroup.Where(u => u.Id == matchedUser.Id).FirstOrDefault();
                    if (userFromList != null)
                    {
                        usersFromUserGroup.Remove(userFromList);
                    }
                }
                else
                {
                    var currentUser = users[i];
                    var userEmail = users[i].Email;
                    var existedUser = _repository.GetSingle<User>(u => u.Email == userEmail);
                    if (existedUser != null)
                    {
                        _repository.Insert<UserToUserGroup>(new UserToUserGroup { UserId = existedUser.Id, GroupId = userGroupId });
                    }
                    else
                    {
                        _repository.Insert<User>(currentUser);
                        var userId = _repository.GetSingle<User>(x => x.Email == currentUser.Email).Id;
                        _repository.Insert<UserToUserGroup>(new UserToUserGroup { UserId = userId, GroupId = userGroupId });

                    }
                }
            }
            if (usersFromUserGroup != null)
            {
                foreach (var item in usersFromUserGroup)
                {
                    _repository.Delete<int, UserToUserGroup>("Id", item.Id);
                }
            }
        }
    }
}