(function (angular) {
    var equizModule = angular.module("equizModule");

    equizModule.controller('refreshWarningCtrl', ["$scope", "$uibModalInstance", function ($scope, $uibModalInstance) {
        $scope.ok = function () {
            $uibModalInstance.close(true);
        };
    }]);
})(angular);