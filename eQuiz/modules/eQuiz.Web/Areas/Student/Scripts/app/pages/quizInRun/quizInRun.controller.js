/// <reference path="D:\MyFork_eQuiz\eQuiz\modules\eQuiz.Web\Scripts/libs/angularjs/angular.js" />
(function (angular) {
    var equizModule = angular.module("equizModule");

    equizModule.controller("quizInRunCtrl", ["$scope", "quizService", "trackUserResultService", "$routeParams", "$interval", "$window",
        function ($scope, quizService, trackUserResultService, $routeParams, $interval, $window) {
            $scope.quizQuestions = null;

            $scope.quizId = parseInt($routeParams.id);
            $scope.quizDuration = $routeParams.dura;
            $scope.currentQuestion = 0;
            $scope.passedQuiz = trackUserResultService.passedQuiz;
            $scope.passedQuiz.QuizId = $scope.quizId;
            $scope.windowHeight = $window.innerHeight;


            $scope.setCurrentQuestion = function (currentQuestionId, index, questionId, isAutomatic, quizBlock, answerText) {
                $scope.setUserTextAnswers(index, questionId, isAutomatic, quizBlock, answerText);

                if (currentQuestionId < $scope.quizQuestions.length && currentQuestionId >= 0) {
                    $scope.currentQuestion = currentQuestionId;
                }

            };

            getQuestionById($scope.quizId);

            function getQuestionById(questionId) {
                quizService.getQuestionsById(questionId)
                    .then(function (response) {
                        console.log(response.data);
                        $scope.quizQuestions = response.data;
                        $scope.passedQuiz.StartDate = new Date(Date.now());
                    });
            };

            $scope.setUserChoice = function (index, questionId, answerId, isAutomatic, quizBlock) {
                trackUserResultService.setUserAnswers(index, questionId, answerId, isAutomatic, quizBlock);
            };

            $scope.setUserTextAnswers = function (index, questionId, isAutomatic, quizBlock, answerText) {
                trackUserResultService.setUserTextAnswers(index, questionId, isAutomatic, quizBlock, answerText);
            };

            //FINISH BUTTON
            $scope.finishQuiz = function () {

                var isUserWantFinish = confirm("A you sure want to finish the  quiz?");

                if (isUserWantFinish) {
                    $scope.passedQuiz.FinishDate = new Date(Date.now());

                    console.log(JSON.stringify($scope.passedQuiz));

                    var passedQuiz = $scope.passedQuiz;
                    quizService.sendUserResult(passedQuiz)
                        .success(function (data) {
                            console.log("OK");
                        });
                }
                
            };

            $scope.$watch(function () {
                return $window.innerHeight;
            }, function (value) {
                $scope.windowHeight = value;
            });

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