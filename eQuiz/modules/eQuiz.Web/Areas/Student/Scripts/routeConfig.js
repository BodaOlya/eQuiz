/// <reference path="templetes/QuizInRunTemplete.html" />
(function (angular) {
    var equizModule = angular.module("equizModule");
    equizModule.config(function ($routeProvider) {
        $routeProvider.when("/", {
            templateUrl: "/Areas/Student/Scripts/app/pages/dashboard/dashboard.html",
            controller: "dashboardCtrl",
            controllerAs: "dashboardCtrl"
        })
        .when("/quizInRun/:id/", {
            templateUrl: "/Areas/Student/Scripts/app/pages/quizInRun/quizInRun.html",
            controller: "quizInRunCtrl",
            controllerAs: "quizCtrl"
        })
        .otherwise({
            redirectTo: '/'
        });

    });
})(angular);