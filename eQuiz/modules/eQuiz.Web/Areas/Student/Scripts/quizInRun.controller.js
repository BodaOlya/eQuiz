/// <reference path="D:\MyFork_eQuiz\eQuiz\modules\eQuiz.Web\Scripts/libs/angularjs/angular.js" />
(function (angular) {
    var equizModule = angular.module("equizModule");

    equizModule.controller("quizInRunCtrl", ["$scope", "$http", "$routeParams", "$interval",
        function ($scope, $http, $routeParams, $interval) {
            $scope.quizQuestions = null;

            $scope.quizId = $routeParams.id;
            $scope.quizDuration = $routeParams.dura;
            $scope.currentQuestion = 0;
            $scope.finalUserResult = {
                quizId: $scope.quizId,
                startDate: null,
                finishDate: null,
                answerResult: []
            };

            $scope.startDate = null;
            $scope.finishDate = null;

            $scope.setCurrentQuestion = function (questionId) {
                if (questionId < $scope.quizQuestions.length && questionId >= 0)
                    $scope.currentQuestion = questionId;
            };

            getQuestionById($scope.quizId);

            function getQuestionById(questionId) {
                $http({
                    method: "GET",
                    url: "GetQuestionsByQuizId",
                    params: { id: questionId }

                }).then(function (response) {
                    console.log(response.data);
                    $scope.quizQuestions = response.data;
                    $scope.startDate = Date.now();
                });
            };

            $scope.setUserChoice = function (index, questionId, answerId, isAutomatic, quizBlock, answerText, answerTime) {
                $scope.finalUserResult.answerResult[index] = {
                    Id: questionId, AnswerId: answerId, AnswerText: answerText, answerTime: answerTime, IsAutomatic: isAutomatic, QuizBlock: quizBlock
                };
                $scope.finalUserResult.startDate = $scope.startDate.toString();
                console.log($scope.finalUserResult);
                //console.log($scope.finalUserResult.answerResult.length);
            };

            /////////////////////////////////TIMER
            $scope.tSeconds = 0;
            $scope.tMinutes = $scope.quizDuration;

            $scope.timer_1 = $scope.tSeconds;
            $scope.minutes = $scope.tMinutes;
            $scope.myStyle = {};
            var stop;


            $scope.start = function () {
                if (angular.isDefined(stop)) return;

                stop = $interval(function () {

                    if ($scope.timer_1 > 0) {
                        $scope.timer_1 = $scope.timer_1 - 1;
                    } else if ($scope.minutes > 0) {
                        $scope.minutes = $scope.minutes - 1;
                        $scope.timer_1 = 59;
                    } else {
                        $scope.Stop();
                    }
                    if ($scope.minutes <= 119) {
                        $scope.myStyle = {
                            color: 'red'
                        }
                    } else {
                        $scope.myStyle = {
                            color: 'black'
                        }
                    }
                }, 1000);
            };//start timer

            $scope.stop = function () {
                if (angular.isDefined(stop)) {
                    $interval.cancel(stop);
                    stop = undefined;
                }
            };

            $scope.reset = function () {
                $scope.Stop();
                if ($scope.tSeconds >= 60 && $scope.tSeconds < 0) {
                    $scope.tSeconds = 0;
                }

                if ($scope.tMinutes < 0) {
                    $scope.tMinutes = 0;
                }

                $scope.timer_1 = $scope.tSeconds;
                $scope.minutes = $scope.tMinutes;
                $scope.myStyle = {
                    color: 'black'
                }
            };

            $scope.$on('$destroy', function () {
                $scope.Stop();
            });
        }]);
})(angular);