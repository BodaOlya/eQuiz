(function (angular) {
    angular
        .module('equizModule', ['ngRoute'])
        .config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
            $routeProvider
                .when('/', {
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
                .when('/Index/Quiz', {
                    templateUrl: '/Areas/Admin/Scripts/quiz-review.html',
                    controller: 'QuizReviewController',
                    controllerAs: 'ReviewCtrl',
                    resolve: {
                        student: function (quizReviewDataService, $location) {
                            return quizReviewDataService.getStudent($location.search().Student).then(function (respond) {
                                return respond.data;
                            })
                        },
                        //group: function (quizReviewDataService, $location) {
                        //    return quizReviewDataService.getGroup($location.search().Quiz).then(function (respond) {
                        //        return respond.data;
                        //    })
                        //},
                        //quiz: function (quizReviewDataService, $location) {
                        //    return quizReviewDataService.getQuiz($location.search().Quiz).then(function (respond) {
                        //        return respond.data;
                        //    })
                        //}
                    }
                })
                .when('/Index/Student', {
                    templateUrl: '/Areas/Admin/Scripts/student.html',
                    controller: 'StudentController',
                    controllerAs: 'sc',
                    resolve: {
                        studentInfo: function (studentDataService, $location) {
                            var Id = $location.search().Id;
                            return studentDataService.getStudentInfo(Id).then(function (respond) {
                                return respond.data;
                            })
                        },
                        studentQuizzes: function (studentDataService, $location) {
                            return studentDataService.getStudentQuizzes($location.search().Id).then(function (respond) {
                                return respond.data;
                            })
                        },
                        studentComments: function (studentDataService, $location) {
                            return studentDataService.getStudentComments($location.search().Id).then(function (respond) {
                                return respond.data;
                            })
                        }
                    } 
                })
                .when('/Index/Details', {
                    templateUrl: '/Areas/Admin/Scripts/quiz-details.html',
                    controller: 'QuizDetailsController'
                })
                .otherwise({ redirectTo: '/' });

            $locationProvider.html5Mode(true);
        }]);
}
)(angular);