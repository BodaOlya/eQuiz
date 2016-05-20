(function () {
    angular.module('equizModule')
           .factory('userGroupService', userGroupService)
    userGroupService.$inject = ['$http'];

    function userGroupService($http) {
        return {
            get: get
        };

        function get() {
            return $http.get('/moderator/usergroup/get');
        }
    }
})();