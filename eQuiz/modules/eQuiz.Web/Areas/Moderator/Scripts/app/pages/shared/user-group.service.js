(function () {
    angular.module('equizModule')
           .factory('userGroupService', userGroupService)
    userGroupService.$inject = ['$http'];

    function userGroupService($http) {

        return {
            get: get,
            getGroup: getGroup
        };

        function get() {
            return $http.get('/moderator/usergroup/get');
        }

        function getGroup(id) {
            var promise = $http.get("/moderator/usergroup/getusergroup?id=" + id.toString());
            promise.then(populateResponse);
            return promise;
        };

        function populateResponse(response) {
            return response.data;
        };
    };
})();