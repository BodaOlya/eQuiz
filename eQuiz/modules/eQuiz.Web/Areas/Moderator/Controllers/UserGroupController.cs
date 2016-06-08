using eQuiz.Entities;
using eQuiz.Repositories.Abstract;
using eQuiz.Web.Areas.Moderator.Models;
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

        [HttpGet]
        public ActionResult GetActiveUserGroups()
        {
            UserGroupState activeState = _repository.GetSingle<UserGroupState>(ugs => ugs.Name == "Active");
            IEnumerable<UserGroup> groups = _repository.Get<UserGroup>(ug => ug.UserGroupStateId == activeState.Id);
            var minGroups = new ArrayList();

            foreach (var group in groups)
            {
                minGroups.Add(GetUserGroupForSerialization(group));
            }

            return Json(minGroups, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult IsNameUnique(string name, int? id)
        {
            var result = ValidateUserGroupName(name, id);
            return Json(result, JsonRequestBehavior.AllowGet);
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
            IEnumerable<UserGroupsViewModel> userGroupsList = null;

            var userGroupesTotal = 0;

            if ((selectedStatus != "Active") && (selectedStatus != "Archived"))
            {
                selectedStatus = null;
            }

            var users = _repository.Get<User>();
            var userGroups = _repository.Get<UserGroup>();
            var userToUserGroups = _repository.Get<UserToUserGroup>();
            var quizzes = _repository.Get<Quiz>();
            var userGroupStates = _repository.Get<UserGroupState>();

            userGroupsList = (from ug in userGroups
                              join uug in userToUserGroups on ug.Id equals uug.GroupId
                              join u in users on uug.UserId equals u.Id into uOuter
                              from user in uOuter.DefaultIfEmpty()
                              join q in quizzes on ug.Id equals q.GroupId into qOuter
                              from quiz in qOuter.DefaultIfEmpty()
                              join ugs in userGroupStates on ug.UserGroupStateId equals ugs.Id
                              join uc in users on ug.CreatedByUserId equals uc.Id
                              group new { ug, user, quiz, ugs, uc }
                              by new
                              {
                                  ugId = ug.Id,
                                  ugName = ug.Name,
                                  ugCreatedBy = string.Format("{0} {1} {2}", uc.LastName, uc.FirstName, uc.FatheName),
                                  ugCreatedDate = ug.CreatedDate,
                                  ugStateName = ugs.Name
                              }
                              into grouped
                              select new UserGroupsViewModel
                              {
                                  Id = grouped.Key.ugId,
                                  Name = grouped.Key.ugName,
                                  CountOfStudents = grouped.Where(g => g.user != null).Select(g => g.user.Id).Distinct().Count(),
                                  CountOfQuizzes = grouped.Where(g => g.quiz != null).Select(g => g.quiz.Id).Distinct().Count(),
                                  CreatedDate = grouped.Key.ugCreatedDate,
                                  CreatedBy = grouped.Key.ugCreatedBy,
                                  StateName = grouped.Key.ugStateName
                              }).Where(item => (searchText == null || item.Name.ToLower().Contains(searchText.ToLower())) &&
                                                  (item.StateName == "Active" || item.StateName == "Archived") &&
                                                  (selectedStatus == null || item.StateName == selectedStatus))
                                            .OrderBy(q => q.Name);

            userGroupesTotal = userGroupsList.Count();

            switch (predicate)
            {
                case "Name":
                    userGroupsList = reverse ? userGroupsList.OrderByDescending(q => q.Name) : userGroupsList.OrderBy(q => q.Name);
                    break;
                case "CountOfStudents":
                    userGroupsList = reverse ? userGroupsList.OrderByDescending(q => q.CountOfStudents) : userGroupsList.OrderBy(q => q.CountOfStudents);
                    break;
                case "CountOfQuizzes":
                    userGroupsList = reverse ? userGroupsList.OrderByDescending(q => q.CountOfQuizzes) : userGroupsList.OrderBy(q => q.CountOfQuizzes);
                    break;
                case "CreatedDate":
                    userGroupsList = reverse ? userGroupsList.OrderBy(q => !q.CreatedDate.HasValue).ThenByDescending(q => q.CreatedDate) :
                        userGroupsList.OrderBy(q => !q.CreatedDate.HasValue).ThenBy(q => q.CreatedDate);
                    break;
                case "CreatedBy":
                    userGroupsList = reverse ? userGroupsList.OrderByDescending(q => q.CreatedBy) : userGroupsList.OrderBy(q => q.CreatedBy);
                    break;
                case "StateName":
                    userGroupsList = reverse ? userGroupsList.OrderByDescending(q => q.StateName) : userGroupsList.OrderBy(q => q.StateName);
                    break;
                default:
                    userGroupsList = reverse ? userGroupsList.OrderByDescending(q => q.Name) : userGroupsList.OrderBy(q => q.Name);
                    break;
            }

            userGroupsList = userGroupsList.Skip((currentPage - 1) * userGroupsPerPage).Take(userGroupsPerPage).ToList<UserGroupsViewModel>();

            return Json(new { UserGroups = userGroupsList, UserGroupsTotal = userGroupesTotal }, JsonRequestBehavior.AllowGet);
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
                existedUserGroup.UserGroupStateId = userGroup.UserGroupStateId;
                existedUserGroup.UserGroupState = null;
                _repository.Update<UserGroup>(existedUserGroup);
                DeleteUsersFromGroupIfExist(userGroupId, users);
            }
            else
            {
                userGroup.UserGroupStateId = _repository.GetSingle<UserGroupState>(x => x.Name == "Active").Id;
                userGroup.CreatedByUserId = 1; //temporary
                userGroup.CreatedDate = DateTime.Now;
                _repository.Insert<UserGroup>(userGroup);
                var id = _repository.GetSingle<UserGroup>(g => g.Name == userGroup.Name).Id;
                userGroupId = id;
                AddUsersToGroup(userGroupId, users);
            }

            return RedirectToAction("GetUserGroup", new { id = userGroupId });
        }



        [HttpGet]
        public ActionResult IsUserValid(string firstName, string lastName, string email)
        {
            bool isValid = ValidateUser(firstName, lastName, email);
            return Json(isValid, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetStates()
        {
            var states = _repository.Get<UserGroupState>().ToList();

            var minStates = new ArrayList();

            foreach (var state in states)
            {
                minStates.Add(GetUserGroupStateForSerialization(state));
            }

            return Json(minStates, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Helpers

        private bool ValidateUserGroupName(string name, int? id)
        {
            bool exists = true;

            if (id != null)
            {
                var userGroup = _repository.GetSingle<UserGroup>(q => q.Name == name);
                if (userGroup == null)
                {
                    exists = false;
                }
                else if (userGroup.Id == (int)id)
                {
                    exists = false;
                }
            }
            else
            {
                exists = _repository.Exists<UserGroup>(q => q.Name == name);
            }

            return !exists;
        }

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
                        currentUser.Phone = currentUser.Phone ?? "";
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

        private object GetUserGroupForSerialization(UserGroup group)
        {
            var minGroup = new
            {
                Id = group.Id,
                UserGroupStateId = group.UserGroupStateId,
                CreatedByUserId = group.CreatedByUserId,
                Name = group.Name,
                CreatedDate = group.CreatedDate.ToString("yyyy-MM-ddTHH:mm:ss")
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

        private object GetUserGroupStateForSerialization(UserGroupState state)
        {
            var minState = new
            {
                Id = state.Id,
                Name = state.Name
            };

            return minState;
        }

        #endregion
    }
}