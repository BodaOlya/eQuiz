(function () {
    angular.module("quizModule")
           .controller("QuizController", QuizController);
    QuizController.$inject = ['$scope', 'quizService', 'userGroupService', '$location'];

    function QuizController($scope, quizService, userGroupService, $location) {
        var vm = this;
        vm.tab = 'quiz';
        vm.quiz = { QuizTypeId: 1, DurationHours: 0, DurationMinutes: 0};
        vm.userGroups = [];
        vm.quizBlock = { QuestionCount: 0 };
        vm.save = save;
        vm.switchTab = switchTab;
        vm.saveCanExecute = saveCanExecute;
        vm.setQuestionController = setQuestionController;

        if ($location.search().id) {
            quizService.get($location.search().id).then(function (data) {
                vm.quiz = data.data.quiz;
                vm.quiz.StartDate = new Date(vm.quiz.StartDate);
                vm.quiz.DurationMinutes = vm.quiz.TimeLimitMinutes % 60;
                vm.quiz.DurationHours = (vm.quiz.TimeLimitMinutes - vm.quiz.TimeLimitMinutes % 60) / 60;
                window.aaa = vm.quiz.UserGroup;
                vm.quizBlock = data.data.block;
            });
        }

        $scope.$on('$locationChangeSuccess', function (event) {
            if ($location.path() == "/quiz") {
                vm.tab = 'quiz';
            }
            else if ($location.path() == '/questions') {
                vm.tab = 'questions';
            }
        });

        userGroupService.get().then(function (data) {
            vm.userGroups = data.data;
        });

        function setQuestionController (controller) {
            if (!vm.questionController) {
                vm.questionController = controller;
            }
        }

        function setForm(form) {
            if (!vm.quizForm) {
                vm.quizForm = form;
            }
        }

        function saveCanExecute() {
            if (vm.quizForm) {
                return !vm.quizForm.$valid;
            }
            return false;
        }

        function switchTab(tab) {
            if (tab == 'quiz') {
                $location.path('/quiz');
            }
            else if (tab == 'questions') {
                $location.path('/questions');
            }
        }

        function save() {
            vm.quiz.TimeLimitMinutes = vm.quiz.DurationHours * 60 + vm.quiz.DurationMinutes;
            quizService.save({ quiz: vm.quiz, block: vm.quizBlock }).then(function (data) {
                vm.quiz = data.data.quiz;
                vm.quiz.StartDate = new Date(vm.quiz.StartDate);
                vm.quiz.DurationMinutes = vm.quiz.TimeLimitMinutes % 60;
                vm.quiz.DurationHours = (vm.quiz.TimeLimitMinutes - vm.quiz.TimeLimitMinutes % 60) / 60;
                vm.quizBlock = data.data.block;
                vm.questionController.saveQuestions(vm.quiz.Id);
            });
        }
    }
})();