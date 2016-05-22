
(function (angular) {
    var equizModule = angular.module("equizModule");

    equizModule.controller("dashboardCtrl", ["$scope", "$http", "dashboardService", function ($scope, $http, dashboardService) {
        $scope.allQuizes = null;

        activate();
        function activate() {
            var quizPromise = dashboardService.getQuizzes();
            quizPromise.success(function (data) {
                $scope.allQuizes = data;
            });
        };
    }]);
})(angular);