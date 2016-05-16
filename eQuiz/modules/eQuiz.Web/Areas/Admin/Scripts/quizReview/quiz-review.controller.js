(function (angular) {
    angular
        .module("equizModule")
        .controller('QuizReviewController', quizReviewController);

    quizReviewController.$inject = ['$scope', 'quizReviewDataService'];

    function quizReviewController($scope, quizReviewDataService) {
        var vm = this;
        vm.passed = 0;
        vm.notPassed = 0;
        vm.inVerification = 0;

        vm.countStats = function () {
            vm.passed = 0;
            vm.notPassed = 0;
            vm.inVerification = 0;

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
            vm.student = quizReviewDataService.getStudent();
            vm.group = quizReviewDataService.getGroup();
            vm.quiz = quizReviewDataService.getQuiz();

            vm.countStats();
        };

        activate();

        vm.setQuestionStatus = function (id, status) {        
            for (var i = 0; i < vm.quiz.questions.length; i++){            
                if (vm.quiz.questions[i].question_id === id) {                    
                    vm.quiz.questions[i].questionStatus = status;
                }
            }
            
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
        }
    };
})(angular);