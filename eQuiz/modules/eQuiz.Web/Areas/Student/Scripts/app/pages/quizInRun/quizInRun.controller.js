(function (angular) {
    angular.module("equizModule")
            .controller('quizInRunCtrl', quizInRunCtrl);
    quizInRunCtrl.$inject = ["$scope", "quizService", "trackUserResultService", "$routeParams", "$interval", "$window", "$location", "$uibModal"];

    function quizInRunCtrl($scope, quizService, trackUserResultService, $routeParams, $interval, $window, $location, $uibModal) {
        var vm = this;
        vm.quizQuestions = null;
        vm.quizId = parseInt($routeParams.id);
        vm.quizDuration = localStorage.getItem('duration');
        vm.currentQuestion = localStorage.getItem('currentQuestion' + vm.quizId) || 0;
        vm.passedQuiz = JSON.parse(localStorage.getItem('passQuiz' + vm.quizId)) || trackUserResultService.passedQuiz;
        vm.passedQuiz.QuizId = vm.quizId;
        vm.windowHeight = $window.innerHeight;
        
        vm.isLoading = false;

        //Timer Data
        vm.tSeconds = 0;
        vm.tMinutes = vm.quizDuration;

        vm.seconds = vm.tSeconds;
        vm.minutes = vm.tMinutes;
        vm.myStyle = {};
        vm.time = vm.minutes + ":0" + vm.seconds;
        var stop;


        vm.setCurrentQuestion = function (currentQuestionId, index, questionId, isAutomatic, quizBlock, questionOrder, answerText) {

            vm.setUserTextAnswers(index, questionId, isAutomatic, quizBlock, questionOrder, answerText);

            vm.sendDataToServer();

            if (currentQuestionId < vm.quizQuestions.length && currentQuestionId >= 0) {
                vm.currentQuestion = currentQuestionId;

                localStorage.setItem('currentQuestion' + vm.quizId, vm.currentQuestion);
            }
        };

        vm.isLoading = true;

        function openPopUpRefreshWarning() {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: '/Areas/Student/Scripts/app/pages/refreshWarning/refreshWarning.html',
                controller: 'refreshWarningCtrl',
                controllerAs: 'rwc',
                size: 'sm'
            });
        };
  
        getQuestionById(vm.quizId, vm.quizDuration);
        
        function getQuestionById(questionId, duration ) {
            quizService.getQuestionsById(questionId, duration)
                .then(function (response) {
                    if (response.data.length === 0 || response.data === "SaveChangeException") {
                        $location.path("/Dashboard");
                    }
                    else {
                        vm.quizQuestions = response.data.questions;   
                        //openPopUpRefreshWarning();
                        vm.passedQuiz.StartDate = new Date(Date.now());

                        //vm.resetTimer();
                        vm.startTimer();
                        vm.tMinutes = Math.floor(response.data.remainingTime / 60);
                        vm.tSeconds = response.data.remainingTime - vm.tMinutes * 60;

                        vm.minutes = vm.tMinutes;
                        vm.seconds = vm.tSeconds;
                        

                        vm.isLoading = false;
                    }
                });
        };

        function sendQuestionResult(passedQuestion) {
            var promise = quizService.sendQuestionResult(passedQuestion);
            return promise;
        };

        function setFinishTime(quizPassId) {

            quizService.setFinishTime(quizPassId);
        };

        vm.setUserSingleChoice = function (index, questionId, answerId, isAutomatic, quizBlock, questionOrder) {
            trackUserResultService.setUserAnswers(index, questionId, answerId, isAutomatic, quizBlock, questionOrder);

            localStorage.setItem('passQuiz' + vm.quizId, JSON.stringify(vm.passedQuiz));
        };
        vm.setUserMultipleChoice = function (index, questionId, answerId, isAutomatic, quizBlock, questionOrder) {
            trackUserResultService.setUserMultipleAnswer(index, questionId, answerId, isAutomatic, quizBlock, questionOrder);

            localStorage.setItem('passQuiz' + vm.quizId, JSON.stringify(vm.passedQuiz));
        };
        vm.setUserTextAnswers = function (index, questionId, isAutomatic, quizBlock, questionOrder, answerText) {
            trackUserResultService.setUserTextAnswers(index, questionId, isAutomatic, quizBlock, questionOrder, answerText);

            localStorage.setItem('passQuiz' + vm.quizId, JSON.stringify(vm.passedQuiz));
        };

        vm.sendDataToServer = function () {

            if (vm.passedQuiz.UserAnswers != null && vm.passedQuiz.UserAnswers[vm.currentQuestion] !== undefined) {
                var answers = null;
                if (vm.passedQuiz.UserAnswers[vm.currentQuestion] != null && vm.passedQuiz.UserAnswers[vm.currentQuestion] != undefined) {
                    if (vm.passedQuiz.UserAnswers[vm.currentQuestion].Answers != undefined && vm.passedQuiz.UserAnswers[vm.currentQuestion].Answers != null) {
                        answers = [];
                        for (var prop in vm.passedQuiz.UserAnswers[vm.currentQuestion].Answers) {
                            answers.push(vm.passedQuiz.UserAnswers[vm.currentQuestion].Answers[prop]);
                        }
                    }

                    var questionResult = {
                        QuestionId: vm.quizQuestions[vm.currentQuestion].Id,
                        QuestionType: vm.quizQuestions[vm.currentQuestion].QuestionType,
                        QuestionOrder: vm.quizQuestions[vm.currentQuestion].QuestionOrder,
                        QuizBlock: vm.quizQuestions[vm.currentQuestion].QuizBlock,
                        QuizPassId: vm.quizQuestions[vm.currentQuestion].QuizPassId,
                        AnswerId: vm.passedQuiz.UserAnswers[vm.currentQuestion].AnswerId,
                        AnswerText: vm.passedQuiz.UserAnswers[vm.currentQuestion].AnswerText,
                        Answers: answers,
                        AnswerTime: vm.passedQuiz.UserAnswers[vm.currentQuestion].AnswerTime
                    }
                    console.log(JSON.stringify(questionResult));
                    sendQuestionResult(questionResult);
                }
            }
            //vm.passedQuiz.FinishDate = new Date(Date.now());
            //var passedQuiz = vm.passedQuiz;
            //for (var i in passedQuiz.UserAnswers) {
            //    if (passedQuiz.UserAnswers[i] != null && passedQuiz.UserAnswers[i] != undefined) {
            //        if (passedQuiz.UserAnswers.hasOwnProperty(i)) {
            //            var arr = [];
            //            if (passedQuiz.UserAnswers[i].Answers != undefined || passedQuiz.UserAnswers[i].Answers != null) {
            //                for (var j in passedQuiz.UserAnswers[i].Answers) {
            //                    arr.push(passedQuiz.UserAnswers[i].Answers[j]);
            //                }
            //                passedQuiz.UserAnswers[i].Answers = arr;
            //            }
            //        }
            //    }
            //}
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
                controllerAs: 'cc',
                size: 'sm'
            });

            modalInstance.result.then(function () {               
                localStorage.removeItem('passQuiz' + vm.quizId);
                localStorage.removeItem('currentQuestion' + vm.quizId);
                vm.sendDataToServer();
                setFinishTime(vm.quizQuestions[vm.currentQuestion].QuizPassId);
                vm.resetTimer();
                trackUserResultService.passedQuiz.UserAnswers = null;
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
                controllerAs: 'ac',
                size: 'sm'
            });

            modalInstance.result.then(function () {
                $location.path("/Dashboard");
            });
        };


        //FINISH BUTTON
        vm.finishQuiz = function (index, questionId, isAutomatic, quizBlock, questionOrder, answerText) {
            if (!vm.quizQuestions[index].isAutomatic) {
                vm.setUserTextAnswers(index, questionId, isAutomatic, quizBlock, questionOrder, answerText);
            }
            openPopUpConfirm();
        };

        $scope.$watch(function () {
            return $window.innerHeight;
        }, function (value) {
            vm.windowHeight = value;
        });


        //Timer Methods
        vm.startTimer = function () {
            if (angular.isDefined(stop)) return;

            stop = $interval(function () {

                if (vm.seconds < 10) {
                    vm.time = vm.minutes + ":0" + vm.seconds;
                } else {
                    vm.time = vm.minutes + ":" + vm.seconds;
                }

                if (vm.seconds > 0) {
                    vm.seconds = vm.seconds - 1;
                } else if (vm.minutes > 0) {
                    vm.minutes = vm.minutes - 1;
                    vm.seconds = 59;
                } else {
                    vm.stopTimer();
                }
                if (vm.minutes <= vm.minutes / 10) {
                    vm.myStyle = {
                        color: 'red'
                    }
                } else {
                    vm.myStyle = {
                        color: 'black'
                    }
                }
                if (vm.minutes == 0 && vm.seconds == 0) {
                    vm.endQuiz();
                }
            }, 1000);
        };//start timer

        vm.stopTimer = function () {
            if (angular.isDefined(stop)) {
                $interval.cancel(stop);
                stop = undefined;
            }
        };

        vm.resetTimer = function () {
            vm.stopTimer();
            if (vm.tSeconds <= 60 && vm.tSeconds > 0) {
                vm.tSeconds = 0;
            }

            if (vm.tMinutes > 0) {
                vm.tMinutes = 0;
            }

            vm.seconds = vm.tSeconds;
            vm.minutes = vm.tMinutes;
            vm.myStyle = {
                color: 'black'
            }
        };

        // time is running out
        vm.endQuiz = function () {
            vm.sendDataToServer();
            setFinishTime(vm.quizQuestions[vm.currentQuestion].QuizPassId);
            vm.resetTimer();
            openPopUpAlert();

            localStorage.removeItem('passQuiz' + vm.quizId);
            localStorage.removeItem('currentQuestion' + vm.quizId);
            trackUserResultService.passedQuiz.UserAnswers = null;
        };

        $scope.$on('$destroy', function () {
            vm.stopTimer();
        });
    };
})(angular);