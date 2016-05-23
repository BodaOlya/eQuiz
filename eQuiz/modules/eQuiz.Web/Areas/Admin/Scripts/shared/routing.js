(function (angular) {
    angular
        .module('equizModule', ['ngRoute'])
        .config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
            $routeProvider
                .when('/Index/Student/List', {
                    templateUrl: '/Areas/Admin/Scripts/review.html',
                    controller: 'ReviewController',
                    controllerAs: 'rc',
                    resolve: {
                        studentsList: function (reviewDataService) {
                            return reviewDataService.getStudents().then(function (respond) {
                                return respond.data;
                            })
                        }
                    }
                })
                .when('/Index/Quiz/Review/:quizId', {
                    templateUrl: '/Areas/Admin/Scripts/quiz-review.html',
                    controller: 'QuizReviewController'
                })
                .when('/Index/Student/:studentId/:tabId', {
                    templateUrl: '/Areas/Admin/Scripts/student.html',
                    controller: 'StudentController',
                    controllerAs: 'sc',
                  /*  resolve: {
                        studentInfo: function ($route, studentDataService) {
                            return studentDataService.getStudentInfo($route.current.params.studentId).then(function (respond) {
                                return respond.data;
                            })
                        },
                        studentQuizzes: function ($route ,studentDataService) {
                            return studentDataService.getStudentQuizzes($route.current.params.studentId).then(function (respond) {
                                return respond.data;
                            })
                        },
                        studentComments: function ($route, studentDataService) {
                            return studentDataService.getStudentComments($route.current.params.studentId).then(function (respond) {
                                return respond.data;
                            })
                        }
                    } */
                })
                .otherwise({ redirectTo: '/Index/Student/List' });

            $locationProvider.html5Mode(true);
        }]);
}
)(angular);