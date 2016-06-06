(function (angular) {
    angular.module("equizModule") 
            .controller('alertCtrl',alertCtrl);
    alertCtrl.$inject = ['$scope', '$uibModalInstance'];

    function alertCtrl($scope, $uibModalInstance) {
        var vm = this;
        vm.ok = function () {
            $uibModalInstance.close(true);
        };
    };
})(angular);