(function (angular) {
    angular
        .module("equizModule")
        .controller('QuizReviewController', quizReviewController);

    quizReviewController.$inject = ['$scope', 'quizReviewDataService', '$location', 'student', 'getQuizTests'];

    function quizReviewController($scope, quizReviewDataService, $location, student, getQuizTests) {
        var vm = this;
        vm.passed = 0;
        vm.notPassed = 0;
        vm.inVerification = 0;
        vm.saveIsDisabled = true;
        vm.isFinalized = false;
        vm.student = student;
        vm.group = quizReviewDataService.getGroup($location.search().Quiz);
        vm.quiz = getQuizTests;   
        vm.selectedStatuses = [];
        vm.statusList = [{ id: 0, name: "In Verification" }, { id: 1, name: "Passed" }, { id: 2, name: "Not Passed" }];

        vm.countStats = function () {
            vm.passed = 0;
            vm.notPassed = 0;
            vm.inVerification = 0;
            //vm.isFinalized = vm.quiz.isFinalized;

            vm.quiz.questions.forEach(function (item) {
                if (item.questionStatus === 0) {
                    vm.inVerification++;
                }
                if (item.questionStatus === 1) {
                    vm.passed++;
                }
                if (item.questionStatus === 2) {
                    vm.notPassed++;
                }
            });
        }

        function activate() {
            vm.student = quizReviewDataService.getStudent($location.search().Student);
            vm.group = quizReviewDataService.getGroup($location.search().Student);
            vm.quiz = quizReviewDataService.getQuiz($location.search().Quiz);
            debugger;
            vm.countStats();
        };

        vm.setQuestionStatus = function (id, status) {
            if (!vm.isFinalized) {
                for (var i = 0; i < vm.quiz.questions.length; i++) {
                    if (vm.quiz.questions[i].question_id === id) {
                        vm.quiz.questions[i].questionStatus = status;
                    }
                }
            }

            vm.saveIsDisabled = false;
            vm.countStats();
        }

        vm.addAttriChecked = function (questionId, aswerId) {    //add attribute 'checked' to checkboxes if finds proper user answer     
            for (var i = 0; i < vm.quiz.questions.length; i++) {
                if (vm.quiz.questions[i].question_id == questionId) {
                    for (var j = 0; j < vm.quiz.questions[i].userAnswer.length; j++) {
                        if (vm.quiz.questions[i].userAnswer[j] == aswerId) {
                            return true;
                        }
                    }
                }
            }
        }

        vm.setButtonColor = function (questionStatus, expectedStatus) { // sets button color
            if (questionStatus == expectedStatus) {
                return true;
            }
        }

        vm.cancelQuizReview = function () {            
            activate();            
        }

        vm.saveQuizReview = function () {
            quizReviewDataService.saveQuizReview(vm.quiz);
            vm.saveIsDisabled = true;
        }

        vm.finalizeQuizReview = function () {
            vm.quiz.isFinalized = true;
            quizReviewDataService.finalizeQuizReview(vm.quiz);
            vm.isFinalized = true;
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

        vm.selectStatusId = function (id) { // DONT PUT THIS FUNCTION INTO VM! let it be in scope (because of 'this' in function)
            // debugger;
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
    };
})(angular);