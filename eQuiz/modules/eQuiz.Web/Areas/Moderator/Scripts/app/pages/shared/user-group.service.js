(function () {
    angular.module('equizModule')
           .factory('userGroupService', userGroupService)
    userGroupService.$inject = ['$http'];

    function userGroupService($http) {

        return {
            getActiveUserGroups: getActiveUserGroups,
            getGroup: getGroup,
            save: save,
            isUserValid: isUserValid,
            isNameUnique: isNameUnique,
            getStates: getStates
        };

        function getActiveUserGroups() {
            return $http.get('/moderator/usergroup/getactiveusergroups');
        }

        function getGroup(id) {
            var promise = $http.get("/moderator/usergroup/getusergroup?id=" + id.toString());
            promise.then(function (data) {
                group = data.data.group;
                users = data.data.users;
            });
            return promise;
        };

        function isUserValid(firstName, lastName, email) {            
            return $http.get("/moderator/usergroup/IsUserValid?FirstName=" + escape(firstName) + "&LastName=" + escape(lastName) + "&Email=" + escape(email));
        }

        function save(data) {
            return $http.post('/moderator/usergroup/save', data);
        };

        function isNameUnique(name, id) {
            if (id) {
                return $http.get('/moderator/usergroup/isnameunique?name=' + escape(name) + "&id=" + escape(id));
            }
            else {
                return $http.get('/moderator/usergroup/isnameunique?name=' + escape(name));
            }
        }

        function getStates() {
            return $http.get("/moderator/usergroup/getstates");
        }
    };
})();