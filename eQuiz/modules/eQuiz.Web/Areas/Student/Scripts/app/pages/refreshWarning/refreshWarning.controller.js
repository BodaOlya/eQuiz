(function (angular) {
    angular.module("equizModule")
            .controller('refreshWarningCtrl', refreshWarningCtrl);
    refreshWarningCtrl.$inject =  ["$scope", "$uibModalInstance"];

    function refreshWarningCtrl($scope, $uibModalInstance) {
        var vm = this;
        vm.ok = function () {
            $uibModalInstance.close(true);
        };
    };
})(angular);