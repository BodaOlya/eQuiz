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

        #region Web Actions
        public ActionResult Index()
        {
            return View();
        }

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

        public ActionResult GetUserGroupsPage(int currentPage = 1, int userGroupsPerPage = 3, string predicate = "Name",
                                            bool reverse = false, string searchText = null, string selectedStatus = null)
        {
            var res = new List<object>();

            var userGroupesTotal = 0;

            if (selectedStatus == "All")
            {
                selectedStatus = null;
            }

            var users = _repository.Get<User>();
            var userGroups = _repository.Get<UserGroup>();
            var userToUserGroups = _repository.Get<UserToUserGroup>();
            var quizzes = _repository.Get<Quiz>();

            var query = (from ug in userGroups
                         join uug in userToUserGroups on ug.Id equals uug.GroupId
                         join u in users on uug.UserId equals u.Id into uOuter
                         from user in uOuter.DefaultIfEmpty()
                         join q in quizzes on ug.Id equals q.GroupId into qOuter
                         from quiz in qOuter.DefaultIfEmpty()
                         group new { ug, user, quiz } by new { ug.Id, ug.Name } into grouped
                         select new
                         {
                             Id = grouped.Key.Id,
                             Name = grouped.Key.Name,
                             CountOfStudents = grouped.Where(g => g.user != null).Select(g => g.user.Id).Distinct().Count(),
                             CountOfQuizzes = grouped.Where(g => g.quiz != null).Select(g => g.quiz.Id).Distinct().Count(),
                             StateName = "Active"
                         }).Where(item => (searchText == null || item.Name.ToLower().Contains(searchText.ToLower())) &&
                                             (item.StateName == "Active" || item.StateName == "Archived" || item.StateName == "Scheduled") &&
                                             (selectedStatus == null || item.StateName == selectedStatus))
                                            .OrderBy(q => q.Name);

            userGroupesTotal = query.Count();

            switch (predicate)
            {
                case "Name":
                    query = reverse ? query.OrderByDescending(q => q.Name) : query.OrderBy(q => q.Name);
                    break;
                case "CountOfStudents":
                    query = reverse ? query.OrderByDescending(q => q.CountOfStudents) : query.OrderBy(q => q.CountOfStudents);
                    break;
                case "CountOfQuizzes":
                    query = reverse ? query.OrderByDescending(q => q.CountOfQuizzes) : query.OrderBy(q => q.CountOfQuizzes);
                    break;
                //case "CreatedDate":
                //    query = reverse ? query.OrderByDescending(q => q.CreatedDate) : query.OrderBy(q => q.CreatedDate);
                //    break;
                //case "CreatedBy":
                //    query = reverse ? query.OrderByDescending(q => q.CreatedBy) : query.OrderBy(q => q.CreatedBy);
                //    break;
                case "StateName":
                    query = reverse ? query.OrderByDescending(q => q.StateName) : query.OrderBy(q => q.StateName);
                    break;
                default:
                    query = reverse ? query.OrderByDescending(q => q.Name) : query.OrderBy(q => q.Name);
                    break;
            }

            res = query.Skip((currentPage - 1) * userGroupsPerPage).Take(userGroupsPerPage).ToList<object>();

            return Json(new { UserGroups = res, UserGroupsTotal = userGroupesTotal }, JsonRequestBehavior.AllowGet);
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
                    currentUser.Phone = currentUser.Phone ?? "";
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
                UserToUserGroup matchedUser = _repository.Get<UserToUserGroup>(u => u.UserId == currentUserId).FirstOrDefault();
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

        [HttpGet]
        public ActionResult IsUserValid(string firstName, string lastName, string email)
        {
            bool isValid = ValidateUser(firstName, lastName, email);
            return Json(isValid, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Helpers

        private bool ValidateUser(string firstName, string lastName, string email)
        {
            var userIsValid = false;

            bool userWithEmailAlreadyExists = _repository.Exists<User>(u => u.Email == email);

            if (userWithEmailAlreadyExists)
            {
                userIsValid = _repository.Exists<User>(u => (u.Email == email) && (u.FirstName == firstName) && (u.LastName == lastName));
            }
            else
            {
                userIsValid = true;
            }

            return userIsValid;
        }

        #endregion
    }
}