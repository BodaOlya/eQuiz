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
        vm.isFinalized = false;
        vm.student = student;      
        vm.quizPassInfo = getQuizPassInfo;        
        vm.quiz = getQuizTests;
        console.log(vm.quiz);
        vm.selectedStatuses = [];
        vm.statusList = [{ id: 0, name: "In Verification" }, { id: 1, name: "Passed" }, { id: 2, name: "Not Passed" }];
        $scope.showNotification = false;
        $scope.showWarning = false;

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

        function activate() {
            vm.student = quizReviewDataService.getStudent($location.search().Student);
            vm.quizPassInfo = quizReviewDataService.getQuizInfo($location.search().Quiz);
            vm.quiz = quizReviewDataService.getQuizBlock($location.search().Quiz);            
            vm.countStats();
        };

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

        vm.setAutoQuestionStatus = function (id, status) {
            if (!vm.isFinalized) {
                for (var i = 0; i < vm.quiz.length; i++) {
                    if (vm.quiz[i].Id === id) {
                        if(status === 1) {
                            vm.quiz[i].UserScore = vm.quiz[i].MaxScore;
                        } else {
                            vm.quiz[i].UserScore = 0;
                        }                        
                    }
                }
            }

            
            vm.saveIsDisabled = false;
            vm.countStats();
        }

        vm.setTextQuestionColor = function (id, userScore) {
            for (var i = 0; i < vm.quiz.length; i++) {
                if (vm.quiz[i].Id === id) {
                    if (vm.quiz[i].UserScore > 0) {
                        return true;
                    }
                    return false;
                }
            }            

            return false;
        }

        vm.cancelQuizReview = function () {            
            activate();            
        }

        vm.saveQuizReview = function () {
            quizReviewDataService.saveQuizReview(vm.quiz);
            vm.saveIsDisabled = true;
            $scope.showNotifyPopUp('Quiz data was sucessfully saved!')
            $timeout($scope.closePopUp, 5000);
        }

        vm.finalizeQuizReview = function () {
            vm.quiz.isFinalized = true;
            quizReviewDataService.finalizeQuizReview(vm.quiz);
            vm.isFinalized = true;
            $scope.showNotifyPopUp('Quiz was sucessfully finalized!')
            $timeout($scope.closePopUp, 5000);
        }


        $scope.setSelectedStatuses = function () { // DONT PUT THIS FUNCTION INTO VM! let it be in scope (because of 'this' in function)
            var id = this.status.id;
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

        // Checking and changing UserScore
        vm.checkAndCount = function (mark, maxScore, questionId, questionPosition) {            
            if (!isNaN(mark) && mark <= maxScore && mark >= 0) {
                for (var i = 0; i < vm.quiz.length; i++) {
                    if (vm.quiz[i].Id === questionId) {
                        vm.quiz[i].UserScore = mark;                      
                    }
                }                
            } else {
                for (var i = 0; i < vm.quiz.length; i++) {
                    if (vm.quiz[i].Id === questionId) {
                        vm.quiz[i].UserScore = 0;
                    }
                }
                alert("Question №" + questionPosition + " mark is invalid");
            }
            
            vm.countStats();
        }
    };
})(angular);