(function (angular) {

    angular
        .module('equizModule')
        .controller('UserGroupsController', UserGroupsController);

    UserGroupsController.$inject = ['$scope', 'userGroupsService'];

    function UserGroupsController($scope, userGroupsService) {
        var vm = this;

        //fields
        vm.userGroups = [];

        vm.isLoading = true;

        //paging with sorting 
        vm.pagingInfo = {
            currentPage: 1,
            userGroupsPerPage: 5,
            predicate: 'Name',
            reverse: false,
            userGroupsTotal: 0,
            searchText: null,
            selectedStatus: 'All'
        };

        //functions
        vm.reloadUserGroups = reloadUserGroups;
        vm.sortBy = sortBy;
        vm.showOrderArrow = showOrderArrow;
        vm.changedValue = changedValue;

        activate();

        function activate() {
            reloadUserGroups();
        };

        function reloadUserGroups() {
            var userGroupsPromise = userGroupsService.getUserGroups(vm.pagingInfo);
            userGroupsPromise.then(function (data) {
                vm.userGroups = data.UserGroups;
                vm.pagingInfo.userGroupsTotal = data.UserGroupsTotal;
                vm.isLoading = false;
            }, errorCallBack);
        };

        function sortBy(predicate) {
            vm.pagingInfo.reverse = (vm.pagingInfo.predicate === predicate) ? !vm.pagingInfo.reverse : false;
            vm.pagingInfo.predicate = predicate;
            reloadUserGroups();
        };

        function showOrderArrow(predicate) {
            if (vm.pagingInfo.predicate === predicate) {
                return vm.pagingInfo.reverse ? '▲' : '▼';
            }
            return '';
        };

        //for dropdown menu
        vm.statuses = ['All', 'Active', 'Archived'];

        function changedValue(item) {
            vm.pagingInfo.selectedStatus = item;
            reloadUserGroups();
        };

        function errorCallBack(error) {
            console.log('An unexpected error has occured: ' + error.statusText);
        };
    }
})(angular);