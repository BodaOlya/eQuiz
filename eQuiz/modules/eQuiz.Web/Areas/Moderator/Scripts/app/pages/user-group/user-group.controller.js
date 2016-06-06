(function (angular) {
    angular
        .module('equizModule')
        .controller('UserGroupController', UserGroupController);

    UserGroupController.$inject = ['$scope', 'userGroupService', '$location', '$timeout'];

    function UserGroupController($scope, userGroupService, $location, $timeout) {

        var vm = this;
        vm.users = [];

        vm.predicate = 'LastName';
        vm.reverse = false;
        vm.errorMessageVisible = false;
        vm.successMessageVisible = false;
        vm.loadingVisible = false;
        vm.regEx = /^[_a-z0-9]+(\.[_a-z0-9]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$/;

        vm.sortBy = sortBy;
        vm.showOrderArrow = showOrderArrow;
        vm.deleteUser = deleteUser;
        vm.addUser = addUser;
        vm.save = save;
        vm.canSave = canSave;
        vm.showSuccess = showSuccess;
        vm.showError = showError;
        vm.showLoading = showLoading;
        vm.hideLoading = hideLoading;
        
        vm.useImportedData = useImportedData;

        activate();

        function activate() {
            if ($location.search().id) {
                vm.showLoading();
                userGroupService.getGroup($location.search().id).then(function (data) {
                    vm.group = data.data.group;
                    vm.users = data.data.users;
                    vm.hideLoading();
                });
            }
        };

        function useImportedData(data) {
            $timeout(function () {
                vm.users.push.apply(vm.users, data);
            }, 1000);            
            var inputFirstName = document.getElementsByName('FirstName');
            var inputLastName = document.getElementsByName('LastName');
            var inputEmail = document.getElementsByName('Email');
            $timeout(function () {
                angular.element(inputFirstName).triggerHandler("blur");
                angular.element(inputLastName).triggerHandler("blur");
                angular.element(inputEmail).triggerHandler("blur");
            }, 1000);
        }

        function sortBy(predicate) {
            vm.reverse = (vm.predicate === predicate) ? !vm.reverse : false;
            vm.predicate = predicate;
        };

        function showOrderArrow(predicate) {
            if (vm.predicate === predicate) {
                return vm.reverse ? '▲' : '▼';
            }
            return '';
        };

        function deleteUser(user) {
            var userIndex = vm.users.indexOf(user);
            vm.users.splice(userIndex, 1);
        };

        function addUser() {
            vm.users.push(
                {
                    Id: 0,
                    LastName: "",
                    FirstName: "",
                    FatheName: "",
                    Email: ""
                });
        };

        function canSave() {            
            if (vm.groupForm) {
                return vm.groupForm.$valid;
            }
            return false;
        };

        function save() {
            vm.showLoading();
            userGroupService.save({ userGroup: vm.group, users: vm.users }).then(function (data) {
                vm.group = data.data.group;
                vm.users = data.data.users;
                vm.hideLoading();
                vm.showSuccess();
            }, function (data) {
                vm.hideLoading();
                vm.showError();
            });
        };

        function showSuccess() {
            vm.successMessageVisible = true;
            $timeout(function () {
                vm.successMessageVisible = false;
                window.location.href = '/moderator/usergroup/index';
            }, 2000);
        }
        function showError() {
            vm.errorMessageVisible = true;
            $timeout(function () {
                vm.errorMessageVisible = false;
            }, 4000);
        }
        function showLoading() {
            vm.loadingVisible = true;
        }
        function hideLoading() {
            vm.loadingVisible = false;
        }
    };

})(angular)