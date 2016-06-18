(function (angular) {
    angular
        .module('equizModule')
        .controller('UserGroupController', UserGroupController);

    UserGroupController.$inject = ['$scope', 'userGroupService', '$location', '$timeout'];

    function UserGroupController($scope, userGroupService, $location, $timeout) {

        var vm = this;
        vm.users = [];
        vm.group = {};

        vm.predicate = 'LastName';
        vm.reverse = false;
        vm.errorMessageVisible = false;
        vm.successMessageVisible = false;
        vm.loadingVisible = false;
        vm.regEx = /^[_A-Za-z0-9]+(\.[_A-Za-z0-9]+)*@[A-Za-z0-9-]+(\.[A-Za-z0-9-]+)*(\.[A-Za-z]{2,4})$/;
        vm.states = [];

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
        vm.revalidateInputs = revalidateInputs;
        vm.checkEmail = checkEmail;

        vm.useImportedData = useImportedData;

        vm.archive = archive;
        vm.canArchive = canArchive;
        vm.isEditingEnabled = isEditingEnabled;

        activate();

        function activate() {
            userGroupService.getStates().then(function (data) {
                vm.states = data.data;
            });

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
            vm.users.push.apply(vm.users, data);
            $scope.$apply();
            vm.revalidateInputs();
        }

        function revalidateInputs() {
            var elements = document.getElementsByTagName("input");
            angular.element(elements).triggerHandler("blur");
            elements = document.getElementsByTagName("select");
            angular.element(elements).triggerHandler("blur");
            elements = document.getElementsByTagName("textarea");
            angular.element(elements).triggerHandler("blur");
        }

        function checkEmail() {
            $timeout(function () {                       
                var inputEmail = document.getElementsByName('Email');            
                angular.element(inputEmail).triggerHandler("blur");
            }, 100);
            
        }

        function sortBy(predicate) {
            vm.reverse = (vm.predicate === predicate) ? !vm.reverse : false;
            vm.predicate = predicate;
            
            vm.users.sort(function (a, b) {
                var aElement = angular.isUndefined(a[predicate])? "": a[predicate].toLowerCase();
                var bElement = angular.isUndefined(b[predicate])? "": b[predicate].toLowerCase();
                                
                if (aElement < bElement)
                    return vm.reverse ? 1 : -1;
                if (aElement > bElement)
                    return vm.reverse ? -1 : 1;
                return 0;
            });
            
            $timeout(function () {
                vm.revalidateInputs();
            }, 1000);
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
            $timeout(function () {
                vm.revalidateInputs();
            }, 1000);
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
            if (!vm.canSave()) {
                vm.revalidateInputs();
                return;
            }

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

        function archive() {
            vm.group.UserGroupStateId = vm.states.filter(function (item) {
                return item.Name == "Archived";
            })[0].Id;
            vm.save();
        }

        function canArchive() {
            var archivedState = vm.states.filter(function (item) {
                return item.Name == "Archived";
            })[0];

            return archivedState ? vm.canSave() && vm.group.Id && vm.group.UserGroupStateId != archivedState.Id : false;
        }

        function isEditingEnabled() {
            var archivedState = vm.states.filter(function (item) {
                return item.Name == "Archived";
            })[0];

            return !vm.group.UserGroupStateId || vm.group.UserGroupStateId != archivedState.Id;
        }
    };

})(angular)