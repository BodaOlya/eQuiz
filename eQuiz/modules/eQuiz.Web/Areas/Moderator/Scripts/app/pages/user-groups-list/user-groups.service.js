(function (angular) {

    angular
        .module('equizModule')
        .factory('userGroupsService', userGroupsService);

    userGroupsService.$inject = ['$http'];

    function userGroupsService($http) {

        var service = {
            getUserGroups: getUserGroups
        };

        return service;

        function getUserGroups(pagingInfo) {
            var promise = $http({
                url: '/UserGroup/GetUserGroupsPage',
                method: 'GET',
                params: pagingInfo
            }).then(populateResponse);
            return promise;
        };

        function getUserGroupsJson() {
            if (localStorage.userGroups) {
                return JSON.parse(localStorage.userGroups);
            } else {
                return [];
            }
        };

        function populateResponse(response) {
            return response.data;
        };
    };
})(angular);