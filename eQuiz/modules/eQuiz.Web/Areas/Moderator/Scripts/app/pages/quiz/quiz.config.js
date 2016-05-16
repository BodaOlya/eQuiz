(function () {
    angular.module("quizModule")
            .config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
                $routeProvider.
                  when('/quiz', {
                      templateUrl: '/Areas/Moderator/Scripts/app/pages/quiz/quiz.html',
                  }).
                  when('/questions', {
                      templateUrl: '/Areas/Moderator/Scripts/app/pages/quiz/questions.html',
                  }).
                  otherwise({
                      redirectTo: '/quiz'
                  });
            }]);

})();