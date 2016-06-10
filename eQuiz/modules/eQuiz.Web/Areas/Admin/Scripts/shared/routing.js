(function (angular) {
    angular
        .module('equizModule', ['ngRoute'])
        .config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
            $routeProvider
                .when('/', {
                    templateUrl: '/Areas/Admin/Scripts/quizzes.html',
                    controller: 'QuizzesController',
                    controllerAs: 'qc',
                    resolve: {
                    quizzesList: function (quizzesDataService) {
                        return quizzesDataService.getQuizzes().then(function (respond) {
                        return respond.data;
                    })
               }
               }
               })
                .when('/Index/Students', {
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
                        getQuizTests: function (quizReviewDataService, $location) {                            
                            return quizReviewDataService.getQuizBlock($location.search().Quiz).then(function (respond) {
                                return respond.data;
                            })
                        },
                        student: function (quizReviewDataService, $location) {
                            return quizReviewDataService.getStudent($location.search().Student).then(function (respond) {
                                return respond.data;
                            })
                        },
                        getQuizPassInfo: function (quizReviewDataService, $location) {
                            return quizReviewDataService.getQuizInfo($location.search().Quiz).then(function (respond) {
                                return respond.data;
                            })
                        }
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
                    },
                    reloadOnSearch: false
                })
                .when('/Index/Details', {
                    templateUrl: '/Areas/Admin/Scripts/quiz-details.html',
                    controller: 'QuizDetailsController',
                    controllerAs: 'qc',
                    resolve: {
                        quizStudents: function (quizDetailsDataService, $location) {
                            return quizDetailsDataService.getQuizPasses($location.search().Id).then(function (respond) {
                                return respond.data;
                            })
                        },
                        quizInfo: function (quizDetailsDataService, $location) {
                            return quizDetailsDataService.getQuiz($location.search().Id).then(function (respond) {
                                return respond.data;
                            })
                        }

                    }
                })
                //.when('/Index/Details', {
                //    templateUrl: '/Areas/Admin/Scripts/quiz-details.html',
                //    controller: 'QuizDetailsController',
                //    controllerAs: 'qc',
                //    resolve: {
                //        quizInfo: function (quizDetailsDataService, $location) {
                //            return quizDetailsDataService.getQuizPasses($location.search().Id).then(function (respond) {
                //                return respond.data;
                //            })
                //        }
                //    }
                //})
                .otherwise({ redirectTo: '/' });

            $locationProvider.html5Mode(true);
        }]);
}
)(angular);