(function (angular) {
    angular
        .module('equizModule')
        .controller('UserGroupController', UserGroupController);

    UserGroupController.$inject = ['$scope', 'userGroupService', '$location'];

    function UserGroupController($scope, userGroupService, $location) {

        var vm = this;
        vm.users = [];
        
        vm.predicate = 'LastName';
        vm.reverse = false;        

        vm.sortBy = sortBy;
        vm.showOrderArrow = showOrderArrow;
        vm.deleteUser = deleteUser;
        vm.save = save;

        activate();

        function activate() {
            if ($location.search().id) {
                userGroupService.getGroup($location.search().id).then(function (data) {
                    vm.group = data.data.group;
                    vm.users = data.data.users;
                });
            }
        };


        function sortBy(predicate) {
            vm.reverse = (vm.predicate === predicate) ? !vm.reverse : false;
            vm.predicate = predicate;
            //reloadGroup();
        };

        function showOrderArrow(predicate) {
            if (vm.predicate === predicate) {
                return vm.reverse ? '▲' : '▼';
            }
            return '';
        };

        function deleteUser(user) {
            var userIndex = vm.users.indexOf(user);
            console.log(user);
            console.log(userIndex);
            vm.users.splice(userIndex, 1);
        };

        function save() {
            userGroupService.save({ userGroup: vm.group, users: vm.users }).then(function (data) {
                vm.group = data.data.group;
                vm.users = data.data.users
            });
        };
    };

})(angular)