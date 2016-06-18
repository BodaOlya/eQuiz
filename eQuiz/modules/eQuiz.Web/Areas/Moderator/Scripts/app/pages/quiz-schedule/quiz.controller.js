(function () {
    angular.module("equizModule")
           .controller("QuizController", QuizController);
    QuizController.$inject = ['$scope', 'quizService', 'userGroupService', '$timeout'];

    function QuizController($scope, quizService, userGroupService, $timeout) {
        var vm = this;
        vm.loadingVisible = false;
        vm.errorMessageVisible = false;
        vm.successMessageVisible = false;
        vm.save = save;
        vm.saveCanExecute = saveCanExecute;
        vm.revalidateInputs = revalidateInputs;
        vm.model = {
            quizzes: [],
            userGroups: []
        }

        vm.showLoading = showLoading;
        vm.hideLoading = hideLoading;

        activate();

        function activate() {
            userGroupService.getActiveUserGroups().then(function (data) {
                vm.model.userGroups = data.data;
            });

            quizService.getOpenQuizzes().then(function (data) {
                vm.model.quizzes = data.data;
            });
        }



        function saveCanExecute() {
            if (vm.quizForm) {
                return vm.quizForm.$valid;
            }
            return false;
        }

        function revalidateInputs() {
            var elements = document.getElementsByTagName("input");
            angular.element(elements).triggerHandler("blur");
            elements = document.getElementsByTagName("select");
            angular.element(elements).triggerHandler("blur");
            elements = document.getElementsByTagName("textarea");
            angular.element(elements).triggerHandler("blur");
        }

        function save() {

            if (!vm.saveCanExecute()) {
                vm.revalidateInputs();
                return;
            }

            showLoading();
            saveQuiz();

            function saveQuiz() {
                vm.model.quiz.UserGroup = vm.model.userGroup;
                vm.model.quiz.StartDate = vm.model.startDate;
                vm.model.quiz.TimeLimitMinutes = vm.model.durationHours * 60 + vm.model.durationMinutes;
                vm.model.quiz.EndDate = new Date(vm.model.quiz.StartDate.getTime() + vm.model.quiz.TimeLimitMinutes * 60000);
                quizService.schedule(vm.model.quiz).then(function (data) {
                    showSuccess();
                }, function (data) {
                    hideLoading();
                    showError();
                });
            }

            function showSuccess() {
                vm.successMessageVisible = true;
                $timeout(function () {
                    window.location.href = '/moderator/quiz';
                }, 2000);
                
            }
            function showError() {
                vm.errorMessageVisible = true;
                $timeout(function () {
                    vm.errorMessageVisible = false;
                }, 4000);
            }
        }

        function showLoading() {
            vm.loadingVisible = true;
        }
        function hideLoading() {
            vm.loadingVisible = false;
        }
    }
})();