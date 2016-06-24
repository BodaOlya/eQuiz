(function (angular) {
    angular
        .module("equizModule")
        .controller('QuizReviewController', quizReviewController);

    quizReviewController.$inject = ['$scope', 'quizReviewDataService', '$location', 'student', 'getQuizTests', 'getQuizPassInfo', '$timeout'];

    function quizReviewController($scope, quizReviewDataService, $location, student, getQuizTests, getQuizPassInfo, $timeout) {
        var vm = this;
        vm.quizStatistics = {
            passed: 0,
            notPassed: 0,
            inVerification: 0,
            userSumPoints: 0
        }
        vm.saveIsDisabled = true;
        vm.cancelIsDisabled = true;
        vm.isFinalized = false;
        vm.student = {};
        vm.quizPassInfo = [];
        vm.quiz = [];
        vm.quizClone = [];
        vm.selectedStatuses = [];
        vm.statusList = [{ id: 0, name: "In Verification" }, { id: 1, name: "Passed" }, { id: 2, name: "Not Passed" }];
        $scope.showNotification = false;
        $scope.showWarning = false;

        vm.deepCopy = function (obj) {
            if (Object.prototype.toString.call(obj) === '[object Array]') {
                var out = [], i = 0, len = obj.length;
                for (; i < len; i++) {
                    out[i] = arguments.callee(obj[i]);
                }
                return out;
            }
            if (typeof obj === 'object') {
                var out = {}, i;
                for (i in obj) {
                    out[i] = arguments.callee(obj[i]);
                }
                return out;
            }
            return obj;
        }

        function activate() {         
            vm.student = student;
            vm.quizPassInfo = getQuizPassInfo;
            vm.quiz = getQuizTests;            

            for (var i = 0; i < vm.quiz.length; i++) {
                vm.quizClone[i] = vm.deepCopy(vm.quiz[i]);
            }            
        };

        activate();

        (vm.countStats = function () {
            vm.quizStatistics.passed = 0;
            vm.quizStatistics.notPassed = 0;
            vm.quizStatistics.userSumPoints = 0;
            vm.quizStatistics.inVerification = 0;
            //vm.isFinalized = vm.quiz.isFinalized;

            vm.quiz.forEach(function (item) {
                vm.quizStatistics.userSumPoints += item.UserScore;
                if (item.UserScore === 0) {
                    vm.quizStatistics.notPassed += 1;
                } else if (item.UserScore == null) {
                    vm.quizStatistics.inVerification += 1;
                } else {
                    vm.quizStatistics.passed += 1;
                }
            });
        })();

        vm.setAutoQuestionColor = function (UserScore, expectedStatus) { // sets button color
            var status = 2;            
            if (UserScore === 0) {
                status = 2;
            }

            else {
                status = 1;
            }

            if (expectedStatus == status) {
                return true;
            }
        }

        vm.cancelQuizReview = function () {            
            vm.quiz = [];

            for (var i = 0; i < vm.quizClone.length; i++) {
                vm.quiz[i] = vm.deepCopy(vm.quizClone[i]);
            }

            vm.cancelIsDisabled = true;
            vm.saveIsDisabled = true;
        }

        vm.saveQuizReview = function () {
            for (var i = 0; i < vm.quiz.length; i++) {          
                if (vm.quiz[i].WasChanged == true && vm.quiz[i].WasNull == false) {
                    quizReviewDataService.updateQuizAnswer(vm.quiz[i].Id, vm.quiz[i].UserScore, 1); //TODO CHANGE 1 to future admin id
                    vm.quiz[i].WasChanged = false;
                }
                
                if (vm.quiz[i].WasChanged == true && vm.quiz[i].WasNull == true) {
                    quizReviewDataService.insertQuizAnswer(vm.quiz[i].Id, vm.quiz[i].UserScore, 1); //TODO CHANGE 1 to future admin id
                    vm.quiz[i].WasChanged = false;
                    vm.quiz[i].WasNull = false;
                }
            };
            
            vm.quizCopy = [];

            for (var i = 0; i < vm.quiz.length; i++) {
                vm.quizClone[i] = vm.deepCopy(vm.quiz[i]);
            }

            vm.cancelIsDisabled = true;
            vm.saveIsDisabled = true;
            $scope.showNotifyPopUp('Quiz data was sucessfully saved!')
            $timeout($scope.closePopUp, 5000);
        }

        vm.returnButton = function () {
            window.history.back();
        }

        vm.finalizeQuizReview = function () {
            vm.quiz.isFinalized = true;
            quizReviewDataService.finalizeQuizReview(vm.quiz);
            vm.isFinalized = true;
            $scope.showNotifyPopUp('Quiz was sucessfully finalized!')
            $timeout($scope.closePopUp, 5000);
        }

        vm.selectStatusId = function (id) {
            if (vm.selectedStatuses.toString().indexOf(id.toString()) > -1) {
                for (var i = 0; i < vm.selectedStatuses.length; i++) {
                    if (vm.selectedStatuses[i] === id) {
                        vm.selectedStatuses.splice(i, 1);
                    }
                }
            } else {
                vm.selectedStatuses.push(id);
            }
            return false;
        };

        vm.isChecked = function (id) {
            if (vm.selectedStatuses.toString().indexOf(id.toString()) > -1 || vm.selectedStatuses.length === 0) {
                return true;
            }
            return false;
        };

        vm.allChecked = function () {
            if (vm.selectedStatuses.length === 0) {
                return true;
            }
            return false;
        };

        vm.checkAll = function () {
            for (var i = 0; i < vm.statusList.length; i++) {
                vm.selectedStatuses.push(vm.statusList[i].id);
            }
        };

        vm.unCheckAll = function () {
            vm.selectedStatuses = [];
        };

        vm.getDateFromSeconds = function (date_in_seconds) {
            var milliseconds = parseInt(date_in_seconds.slice(6, date_in_seconds.length - 2)); // getting only numbers without '/Date()'
            var result = new Date(milliseconds);
            return result;
        }

        // Changing UserScore for auto questions
        vm.setAutoQuestionStatus = function (id, status) {
            if (vm.isFinalized) {
                alert("This quiz was finalized");
            }
            else {
                for (var i = 0; i < vm.quiz.length; i++) {
                    if (vm.quiz[i].Id === id) {
                        if (status === 1) {
                            vm.quiz[i].UserScore = vm.quiz[i].MaxScore;
                            vm.quiz[i].WasChanged = true;
                        } else {
                            vm.quiz[i].UserScore = 0;
                            vm.quiz[i].WasChanged = true;
                        }
                    }
                }

                vm.cancelIsDisabled = false;
                vm.saveIsDisabled = false;
                vm.countStats();
            }
        }

        // Checking and changing UserScore for text questions
        vm.checkAndCount = function (mark, maxScore, questionId, questionPosition) {
            if (vm.isFinalized) {
                alert("This quiz was finalized");
            } else {
                if (!isNaN(mark) && mark <= maxScore && mark >= 0) {
                    for (var i = 0; i < vm.quiz.length; i++) {
                        if (vm.quiz[i].Id === questionId) {
                            vm.quiz[i].UserScore = mark;
                            
                            vm.quiz[i].WasChanged = true;
                        }
                    }
                } else {
                    for (var i = 0; i < vm.quiz.length; i++) {
                        if (vm.quiz[i].Id === questionId) {
                            vm.quiz[i].UserScore = 0;
                        }
                    }
                    alert("Question №" + questionPosition + " mark is invalid and user score was changed to 0");
                }

                vm.saveIsDisabled = false;
                vm.cancelIsDisabled = false;
                vm.countStats();
            }            
        }
    };
})(angular);