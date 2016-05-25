(function (angular) {
    angular
        .module('equizModule', ['ngRoute'])
        .config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
            $routeProvider
                .when('/List', {
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
                .when('/Quiz', {
                    templateUrl: '/Areas/Admin/Scripts/quiz-review.html',
                    controller: 'QuizReviewController'
                })
                .when('/Student', {
                    templateUrl: '/Areas/Admin/Scripts/student.html',
                    controller: 'StudentController',
                    controllerAs: 'sc',
                    resolve: {
                        studentInfo: function ($route, studentDataService, $location) {
                            var Id = $location.search().Id;
                            return studentDataService.getStudentInfo(Id).then(function (respond) {
                                return respond.data;
                            })
                        },
                        studentQuizzes: function ($route, studentDataService, $location) {
                            return studentDataService.getStudentQuizzes($location.search().Id).then(function (respond) {
                                return respond.data;
                            })
                        },
                        studentComments: function ($route, studentDataService, $location) {
                            return studentDataService.getStudentComments($location.search().Id).then(function (respond) {
                                return respond.data;
                            })
                        }
                    } 
                })
                .otherwise({ redirectTo: '/List' });

            $locationProvider.html5Mode(true);
        }]);
}
)(angular);