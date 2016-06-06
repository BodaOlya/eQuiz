(function (angular) {
    angular.module("equizModule")
            .controller('confirmCtrl', confirmCtrl);
    confirmCtrl.$inject = ["$scope", "$uibModalInstance"];

    function confirmCtrl($scope, $uibModalInstance) {
        var vm = this;
        vm.ok = function () {
            $uibModalInstance.close(true);
        };

        vm.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };
})(angular);