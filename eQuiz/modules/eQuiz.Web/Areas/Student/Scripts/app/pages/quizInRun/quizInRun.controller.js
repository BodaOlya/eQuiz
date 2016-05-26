/// <reference path="D:\MyFork_eQuiz\eQuiz\modules\eQuiz.Web\Scripts/libs/angularjs/angular.js" />
(function (angular) {
    var equizModule = angular.module("equizModule");

    equizModule.controller("quizInRunCtrl", ["$scope", "quizService", "trackUserResultService", "$routeParams", "$interval", "$window", "$location",
    function ($scope, quizService, trackUserResultService, $routeParams, $interval, $window, $location) {
        $scope.quizQuestions = null;
        $scope.quizId = parseInt($routeParams.id);
        $scope.quizDuration = $routeParams.dura;
        $scope.currentQuestion = 0;
        $scope.passedQuiz = trackUserResultService.passedQuiz;
        $scope.passedQuiz.QuizId = $scope.quizId;
        $scope.windowHeight = $window.innerHeight;

        //Timer Data
        $scope.tSeconds = 0;
        $scope.tMinutes = $scope.quizDuration;

        $scope.seconds = $scope.tSeconds;
        $scope.minutes = $scope.tMinutes;
        $scope.myStyle = {};
        $scope.time = $scope.minutes + ":0" + $scope.seconds;
        var stop;


        $scope.setCurrentQuestion = function (currentQuestionId, index, questionId, isAutomatic, quizBlock, questionOrder, answerText) {
            $scope.setUserTextAnswers(index, questionId, isAutomatic, quizBlock, questionOrder, answerText);

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

        $scope.setUserChoice = function (index, questionId, answerId, isAutomatic, quizBlock, questionOrder) {
            trackUserResultService.setUserAnswers(index, questionId, answerId, isAutomatic, quizBlock, questionOrder);
        };

        $scope.setUserTextAnswers = function (index, questionId, isAutomatic, quizBlock, questionOrder, answerText) {
            trackUserResultService.setUserTextAnswers(index, questionId, isAutomatic, quizBlock, questionOrder, answerText);
        };

        $scope.sendDataToServer = function () {

            $scope.passedQuiz.FinishDate = new Date(Date.now());
            console.log($scope.passedQuiz);
            var passedQuiz = $scope.passedQuiz;
            quizService.sendUserResult(passedQuiz)
                .success(function (data) {
                    console.log("OK");
                });

            $location.path("/Dashboard");
        };
        //FINISH BUTTON
        $scope.finishQuiz = function (index, questionId, isAutomatic, quizBlock, questionOrder, answerText) {
            if (!$scope.quizQuestions[index].isAutomatic) {
                $scope.setUserTextAnswers(index, questionId, isAutomatic, quizBlock, questionOrder, answerText);
            }

            var isUserWantFinish = confirm("A you sure want to finish the  quiz?");

            if (isUserWantFinish) {
                $scope.sendDataToServer();
            }
            $scope.resetTimer();
        };

        $scope.$watch(function () {
            return $window.innerHeight;
        }, function (value) {
            $scope.windowHeight = value;
        });


        //Timer Methods
        $scope.startTimer = function () {
            if (angular.isDefined(stop)) return;

            stop = $interval(function () {

                if ($scope.seconds < 10) {
                    $scope.time = $scope.minutes + ":0" + $scope.seconds;
                } else {
                    $scope.time = $scope.minutes + ":" + $scope.seconds;
                }

                if ($scope.seconds > 0) {
                    $scope.seconds = $scope.seconds - 1;
                } else if ($scope.minutes > 0) {
                    $scope.minutes = $scope.minutes - 1;
                    $scope.seconds = 59;
                } else {
                    $scope.Stop();
                }
                if ($scope.minutes <= $scope.minutes / 10) {
                    $scope.myStyle = {
                        color: 'red'
                    }
                } else {
                    $scope.myStyle = {
                        color: 'black'
                    }
                }
                if ($scope.minutes == 0 && $scope.seconds == 0) {
                    $scope.endQuiz();
                }
            }, 1000);
        };//start timer

        $scope.stopTimer = function () {
            if (angular.isDefined(stop)) {
                $interval.cancel(stop);
                stop = undefined;
            }
        };

        $scope.resetTimer = function () {
            $scope.stopTimer();
            if ($scope.tSeconds <= 60 && $scope.tSeconds > 0) {
                $scope.tSeconds = 0;
            }

            if ($scope.tMinutes > 0) {
                $scope.tMinutes = 0;
            }

            $scope.seconds = $scope.tSeconds;
            $scope.minutes = $scope.tMinutes;
            $scope.myStyle = {
                color: 'black'
            }
        };

        $scope.endQuiz = function () {
            $scope.sendDataToServer();
            $scope.resetTimer();
            alert("Time is up. Your result was sended to server");

        };

        $scope.$on('$destroy', function () {
            $scope.stopTimer();
        });
    }]);
})(angular);