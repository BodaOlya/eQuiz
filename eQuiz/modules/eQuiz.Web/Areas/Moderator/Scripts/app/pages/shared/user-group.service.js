(function () {
    angular.module('equizModule')
           .factory('userGroupService', userGroupService)
    userGroupService.$inject = ['$http'];

    function userGroupService($http) {

        return {
            get: get,
            getGroup: getGroup,
            save: save,
            isUsersValid: isUsersValid
        };

        function get() {
            return $http.get('/moderator/usergroup/get');
        }

        function getGroup(id) {
            var promise = $http.get("/moderator/usergroup/getusergroup?id=" + id.toString());
            promise.then(function (data) {
                group = data.data.group;
                users = data.data.users;
            });
            return promise;
        };

        function isUsersValid(user) {            
            return $http.post("/moderator/usergroup/IsUserValid", user);
        }

        function save(data) {
            return $http.post('/moderator/usergroup/save', data);
        };
    };
})();