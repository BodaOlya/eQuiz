/// <reference path="D:\MyFork_eQuiz\eQuiz\modules\eQuiz.Web\Scripts/libs/angularjs/angular.js" />
(function (angular) {
    var equizModule = angular.module("equizModule");

    equizModule.controller("quizInRunCtrl", ["$scope", "quizService", "trackUserResultService", "$routeParams", "$interval", "$window", "$location", "$uibModal",
    function ($scope, quizService, trackUserResultService, $routeParams, $interval, $window, $location, $uibModal) {
        $scope.quizQuestions = null;
        $scope.quizId = parseInt($routeParams.id);
        $scope.quizDuration = localStorage.getItem('duration');
        $scope.currentQuestion = 0;
        $scope.passedQuiz = trackUserResultService.passedQuiz;
        $scope.passedQuiz.QuizId = $scope.quizId;
        $scope.windowHeight = $window.innerHeight;

        $scope.isLoading = false;

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

            if ($scope.passedQuiz.UserAnswers[$scope.currentQuestion] !== undefined) {
                var answers = null;
                if ($scope.passedQuiz.UserAnswers[$scope.currentQuestion].Answers != undefined && $scope.passedQuiz.UserAnswers[$scope.currentQuestion].Answers != null) {
                    answers = [];
                    for (var prop in $scope.passedQuiz.UserAnswers[$scope.currentQuestion].Answers) {
                        answers.push($scope.passedQuiz.UserAnswers[$scope.currentQuestion].Answers[prop]);
                    }
                }
                var questionResult = {
                    QuestionId: $scope.quizQuestions[$scope.currentQuestion].Id,
                    QuestionType: $scope.quizQuestions[$scope.currentQuestion].QuestionType,
                    QuestionOrder: $scope.quizQuestions[$scope.currentQuestion].QuestionOrder,
                    QuizBlock: $scope.quizQuestions[$scope.currentQuestion].QuizBlock,
                    QuizPassId: $scope.quizQuestions[$scope.currentQuestion].QuizPassId,
                    AnswerId: $scope.passedQuiz.UserAnswers[$scope.currentQuestion].AnswerId,
                    AnswerText: $scope.passedQuiz.UserAnswers[$scope.currentQuestion].AnswerText,
                    Answers: answers,
                    AnswerTime: $scope.passedQuiz.UserAnswers[$scope.currentQuestion].AnswerTime
                }
                console.log(JSON.stringify(questionResult));
                sendQuestionResult(questionResult);
            }

            if (currentQuestionId < $scope.quizQuestions.length && currentQuestionId >= 0) {
                $scope.currentQuestion = currentQuestionId;
            }
        };

        $scope.isLoading = true;

        function openPopUpRefreshWarning() {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: '/Areas/Student/Scripts/app/pages/refreshWarning/refreshWarning.html',
                controller: 'refreshWarningCtrl',
                size: 'sm'
            });
        };
  
        getQuestionById($scope.quizId, $scope.quizDuration);
        
        function getQuestionById(questionId, duration ) {
            quizService.getQuestionsById(questionId, duration)
                .then(function (response) {
                    if (response.data.length === 0 || response.data === "SaveChangeException") {
                        $location.path("/Dashboard");
                    }
                    else {
                        $scope.quizQuestions = response.data;   
                        //openPopUpRefreshWarning();
                        $scope.passedQuiz.StartDate = new Date(Date.now());
                        $scope.isLoading = false;
                    }
                });
        };

        function sendQuestionResult(passedQuestion) {
            quizService.sendQuestionResult(passedQuestion);
        };

        $scope.setUserSingleChoice = function (index, questionId, answerId, isAutomatic, quizBlock, questionOrder) {
            trackUserResultService.setUserAnswers(index, questionId, answerId, isAutomatic, quizBlock, questionOrder);
        };
        $scope.setUserMultipleChoice = function (index, questionId, answerId, isAutomatic, quizBlock, questionOrder) {
            trackUserResultService.setUserMultipleAnswer(index, questionId, answerId, isAutomatic, quizBlock, questionOrder);
        };
        $scope.setUserTextAnswers = function (index, questionId, isAutomatic, quizBlock, questionOrder, answerText) {
            trackUserResultService.setUserTextAnswers(index, questionId, isAutomatic, quizBlock, questionOrder, answerText);
        };

        $scope.sendDataToServer = function () {

            $scope.passedQuiz.FinishDate = new Date(Date.now());
            var passedQuiz = $scope.passedQuiz;
            for (var i in passedQuiz.UserAnswers) {
                if (passedQuiz.UserAnswers[i] != null && passedQuiz.UserAnswers[i] != undefined) {
                    if (passedQuiz.UserAnswers.hasOwnProperty(i)) {
                        var arr = [];
                        if (passedQuiz.UserAnswers[i].Answers != undefined || passedQuiz.UserAnswers[i].Answers != null) {
                            for (var j in passedQuiz.UserAnswers[i].Answers) {
                                arr.push(passedQuiz.UserAnswers[i].Answers[j]);
                            }
                            passedQuiz.UserAnswers[i].Answers = arr;
                        }
                    }
                }
            }
            //quizService.sendUserResult(passedQuiz)
            //    .success(function (data) {
            //    });     
        };

        //Custom confirm function
        var openPopUpConfirm = function () {

            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: '/Areas/Student/Scripts/app/pages/confirm/customConfirm.html',
                controller: 'confirmCtrl',
                size: 'sm'
            });

            modalInstance.result.then(function () {
                $scope.sendDataToServer();
                $scope.resetTimer();
                $location.path("/Dashboard");
            }, function () {
                return;
            });
        };
        //Custom alert function
        var openPopUpAlert = function () {

            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: '/Areas/Student/Scripts/app/pages/alert/customAlert.html',
                controller: 'alertCtrl',
                size: 'sm'
            });

            modalInstance.result.then(function () {
                $location.path("/Dashboard");
            });
        };


        //FINISH BUTTON
        $scope.finishQuiz = function (index, questionId, isAutomatic, quizBlock, questionOrder, answerText) {
            if (!$scope.quizQuestions[index].isAutomatic) {
                $scope.setUserTextAnswers(index, questionId, isAutomatic, quizBlock, questionOrder, answerText);
            }

            openPopUpConfirm();
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
            openPopUpAlert();
        };

        $scope.$on('$destroy', function () {
            $scope.stopTimer();
        });
    }]);
})(angular);