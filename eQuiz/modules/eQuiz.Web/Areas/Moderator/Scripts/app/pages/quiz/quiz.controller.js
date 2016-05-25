(function () {
    angular.module("equizModule")
           .controller("QuizController", QuizController);
    QuizController.$inject = ['$scope', 'quizService', 'userGroupService', '$location', 'questionService', '$timeout'];

    function QuizController($scope, quizService, userGroupService, $location, questionService, $timeout) {
        var vm = this;
        vm.loadingVisible = false;
        vm.errorMessageVisible = false;
        vm.successMessageVisible = false;
        vm.isStateEditable = true;
        vm.tab = 'quiz';
        vm.save = save;
        vm.switchTab = switchTab;
        vm.saveCanExecute = saveCanExecute;
        vm.model = {
            quiz: { QuizTypeId: 1, DurationHours: 0, DurationMinutes: 0 },
            userGroups: [],
            states: [],
            quizzesForCopy: [],
            quizBlock: { QuestionCount: 0 },
            questions: [],
            answers: [],
            tags: [],
            orderArray: [],
            questionTypes: [],
            answersDirty: []
        }
        vm.setQuestionType = setQuestionType;
        vm.addNewQuestion = addNewQuestion;
        vm.addNewAnswer = addNewAnswer;
        vm.checkAnswerForSelectOne = checkAnswerForSelectOne;
        vm.deleteAnswer = deleteAnswer;
        vm.deleteQuestion = deleteQuestion;
        vm.order = order;
        vm.showOrderArrow = showOrderArrow;
        vm.toViewModel = toViewModel;
        vm.toServerModel = toServerModel;
        vm.getQuestions = getQuestions;
        vm.getAnswerCount = getAnswerCount;
        vm.getCheckedCountForSelectOne = getCheckedCountForSelectOne;
        vm.getCheckedCountForSelectMany = getCheckedCountForSelectMany;
        vm.isEditingEnabled = isEditingEnabled;
        vm.isDirtyAnswerCount = isDirtyAnswerCount;
        vm.isDirtyAnswerChecked = isDirtyAnswerChecked;
        vm.isQuestionsFormValid = isQuestionsFormValid;

        vm.toggleQuizzesForCopy = toggleQuizzesForCopy;
        vm.quizzesForCopyVisible = false;
        vm.getQuestionsCopy = getQuestionsCopy;
        vm.selectQuizCopy = selectQuizCopy;
        vm.selectedQuizCopy = { Id: 0, Name: 'New' };
        vm.showLoading = showLoading;
        vm.hideLoading = hideLoading;

        activate();

        function activate() {
            if ($location.search().id) {
                vm.showLoading();
                vm.getQuestions($location.search().id);
                quizService.get($location.search().id).then(function (data) {
                    vm.model.quiz = data.data.quiz;
                    vm.isStateEditable = vm.model.quiz.QuizState.Name != 'Scheduled';
                    vm.model.quiz.StartDate = new Date(vm.model.quiz.StartDate);
                    vm.model.quiz.DurationMinutes = vm.model.quiz.TimeLimitMinutes % 60;
                    vm.model.quiz.DurationHours = (vm.model.quiz.TimeLimitMinutes - vm.model.quiz.TimeLimitMinutes % 60) / 60;
                    vm.model.quizBlock = data.data.block;
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

            quizService.getQuizzesForCopy().then(function (data) {
                vm.model.quizzesForCopy = data.data;
                vm.model.quizzesForCopy.splice(0, 0, vm.selectedQuizCopy);
            });

            quizService.getStates().then(function (data) {
                vm.model.states = data.data;
            });

            userGroupService.get().then(function (data) {
                vm.model.userGroups = data.data;
            });

            questionService.getQuestionTypes().then(function (response) {
                vm.model.questionTypes = response.data;
            });
        }

        function selectQuizCopy(quiz) {
            if (quiz.Name == 'New') {
                vm.model.questions = [];
                vm.model.answers = [];
                vm.model.tags = [];
                vm.model.quizBlock.QuestionCount = 0;
            }
            else {
                vm.getQuestionsCopy(quiz.Id);
            }
            vm.selectedQuizCopy = quiz;
            vm.toggleQuizzesForCopy();
        }

        function toggleQuizzesForCopy() {
            vm.quizzesForCopyVisible = !vm.quizzesForCopyVisible;
        }

        function saveCanExecute() {
            if (vm.quizForm) {
                var res = vm.quizForm.$valid && vm.isQuestionsFormValid();
                return vm.quizForm.$valid && vm.isQuestionsFormValid();
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
            showLoading();
            saveQuiz();

            function saveQuestions() {
                var quizQuestionVM = vm.toServerModel();
                quizQuestionVM.id = vm.model.quiz.Id;
                questionService.saveQuestions(quizQuestionVM).then(function (response) {
                    var modelFromServer = response.data;
                    var model = vm.toViewModel(modelFromServer);
                    vm.model.questions = model.questions;
                    vm.model.answers = model.answers;
                    vm.model.tags = model.tags;
                    hideLoading();
                    showSuccess();
                }, function (response) {
                    hideLoading();
                    showError();
                });
            }

            function saveQuiz() {
                if (vm.model.quiz.QuizState.Name == 'Scheduled') {
                    vm.model.quiz.TimeLimitMinutes = vm.model.quiz.DurationHours * 60 + vm.model.quiz.DurationMinutes;
                    vm.model.quiz.EndDate = new Date(vm.model.quiz.StartDate.getTime() + vm.model.quiz.TimeLimitMinutes * 60000);
                }
                quizService.save({ quiz: vm.model.quiz, block: vm.model.quizBlock }).then(function (data) {
                    vm.model.quiz = data.data.quiz;
                    vm.model.quiz.StartDate = new Date(vm.model.quiz.StartDate);
                    vm.model.quiz.DurationMinutes = vm.model.quiz.TimeLimitMinutes % 60;
                    vm.model.quiz.DurationHours = (vm.model.quiz.TimeLimitMinutes - vm.model.quiz.TimeLimitMinutes % 60) / 60;
                    vm.model.quizBlock = data.data.block;
                    saveQuestions();
                }, function (data) {
                    hideLoading();
                    showError();
                });
            }

            function showSuccess() {
                vm.successMessageVisible = true;
                $timeout(function () {
                    vm.successMessageVisible = false;
                }, 4000);
            }
            function showError() {
                vm.errorMessageVisible = true;
                $timeout(function () {
                    vm.errorMessageVisible = false;
                }, 4000);
            }
        }


        function setQuestionType(question, typeId, form) {
            question.QuestionTypeId = typeId;

            var questionIndex = vm.model.questions.indexOf(question);

            var countChecked = vm.model.answers[questionIndex].filter(function (item) {
                return item.IsRight;
            }).length;

            if (typeId == 2 && countChecked != 1) {
                for (var i = 0; i < vm.model.answers[questionIndex].length; i++) {
                    vm.model.answers[questionIndex][i].IsRight = false;
                }
            }

            vm.model.answersDirty[questionIndex] = {
                countAnswersDirty: false,
                checkedAnswersDirty: false
            };
            form.$setValidity("No answers", true);
            form.$setValidity("Only one correct answer", true);
            form.$setValidity("At least one correct answer", true);
        }

        function showLoading() {
            vm.loadingVisible = true;
        }
        function hideLoading() {
            vm.loadingVisible = false;
        }

        function addNewQuestion() {
            vm.model.questions.push({
                Id: 0,
                QuestionTypeId: vm.model.questionTypes[0].Id,
                TopicId: 0,
                QuestionText: "",
                QuestionComplexity: 0,
                IsActive: true,
                QuestionType: null,
                Topic: null,
                QuestionAnswers: null,
                QuestionTags: null,
                QuizPassQuestions: null,
                QuizQuestions: null,
            });

            vm.model.answers.push([]);

            vm.model.tags.push([]);

            vm.model.orderArray.push({
                reverse: false,
                predicate: ""
            });

            vm.model.answersDirty.push({
                countAnswersDirty: false,
                checkedAnswersDirty: false
            });
        }

        function addNewAnswer(question, questionIndex) {
            var answerOrder = vm.model.answers[questionIndex].length + 1;
            vm.model.answers[questionIndex].push({
                Id: 0,
                QuestionId: question.Id,
                AnswerText: "",
                AnswerOrder: answerOrder,
                IsRight: false,
                Question: null,
                UserAnswers: null
            });

            vm.model.answersDirty[questionIndex].countAnswersDirty = true;
        }

        function checkAnswerForSelectOne(answer, question) {
            var questionIndex = vm.model.questions.indexOf(question);
            for (var i = 0; i < vm.model.answers[questionIndex].length; i++) {
                vm.model.answers[questionIndex][i].IsRight = false;
            }
            answer.IsRight = true;

            vm.model.answersDirty[questionIndex].checkedAnswersDirty = true;
        }

        function deleteAnswer(answer, question) {
            var questionIndex = vm.model.questions.indexOf(question);
            var answerIndex = vm.model.answers[questionIndex].indexOf(answer);
            vm.model.answers[questionIndex].splice(answerIndex, 1);

            vm.model.answersDirty[questionIndex].countAnswersDirty = true;
            vm.model.answersDirty[questionIndex].checkedAnswersDirty = true;
        }

        function deleteQuestion(questionIndex) {
            vm.model.questions.splice(questionIndex, 1);
            vm.model.answers.splice(questionIndex, 1);
            vm.model.tags.splice(questionIndex, 1);
            vm.model.orderArray.splice(questionIndex, 1);
            vm.model.answersDirty.splice(questionIndex, 1);
        }

        function order(questionIndex, name) {
            vm.model.orderArray[questionIndex].reverse = (vm.model.orderArray[questionIndex].predicate === name) ? !vm.model.orderArray[questionIndex].reverse : false;
            vm.model.orderArray[questionIndex].predicate = name;
        }

        function showOrderArrow(questionIndex, name) {
            if (vm.model.orderArray[questionIndex].predicate === name) {
                return vm.model.orderArray[questionIndex].reverse ? '▼' : '▲';
            }
            return '';
        }

        function toViewModel(modelFromServer) {

            var tags = [];
            for (var i = 0; i < modelFromServer.tags.length; i++) {

                var tagArray = [];

                for (var j = 0; j < modelFromServer.tags[i].length; j++) {
                    tagArray.push(modelFromServer.tags[i][j].Name);
                }

                tags.push(tagArray);

            }
            return {
                id: modelFromServer.id,
                questions: modelFromServer.questions,
                answers: modelFromServer.answers,
                tags: tags
            };
        }

        function toServerModel() {
            var tags = [];
            for (var i = 0; i < vm.model.tags.length; i++) {

                var tagArray = [];
                for (var j = 0; j < vm.model.tags[i].length; j++) {
                    tagArray.push({
                        Id: 0,
                        Name: vm.model.tags[i][j],
                        QuestionTags: null
                    });
                }
                if (tagArray.length == 0) {
                    tagArray.push(null);
                }
                tags.push(tagArray);
            }

            var answers = [];

            for (var i = 0; i < vm.model.answers.length; i++) {

                var answerArray = [];
                for (var j = 0; j < vm.model.answers[i].length; j++) {
                    answerArray.push(vm.model.answers[i][j]);
                }
                if (answerArray.length == 0) {
                    answerArray.push(null);
                }
                answers.push(answerArray);
            }

            return {
                questions: vm.model.questions,
                tags: tags,
                answers: answers
            };
        }

        function getQuestions(quizId) {
            questionService.getQuestions(quizId).then(function (response) {
                var modelFromServer = response.data;

                var model = vm.toViewModel(modelFromServer);
                vm.model.questions = model.questions;
                vm.model.answers = model.answers;
                vm.model.tags = model.tags;
                vm.model.orderArray = Array.apply(null, Array(vm.model.questions.length)).map(function () {
                    return {
                        reverse: false,
                        predicate: ""
                    };
                });
                vm.model.answersDirty = Array.apply(null, Array(vm.model.questions.length)).map(function () {
                    return {
                        countAnswersDirty: false,
                        checkedAnswersDirty: false
                    };
                });
                vm.hideLoading();
            });
        }

        function getQuestionsCopy(quizId) {
            vm.showLoading();
            questionService.getQuestionsCopy(quizId).then(function (response) {
                var modelFromServer = response.data;

                var model = vm.toViewModel(modelFromServer);
                vm.model.questions = model.questions;
                vm.model.answers = model.answers;
                vm.model.tags = model.tags;
                vm.model.orderArray = Array.apply(null, Array(vm.model.questions.length)).map(function () {
                    return {
                        reverse: false,
                        predicate: ""
                    };
                });
                vm.model.answersDirty = Array.apply(null, Array(vm.model.questions.length)).map(function () {
                    return {
                        countAnswersDirty: false,
                        checkedAnswersDirty: false
                    };
                });
                vm.model.quizBlock.QuestionCount = vm.model.questions.length;
                vm.hideLoading();
            });
        }

        function getAnswerCount(questionIndex, form) {
            form.$setValidity("No answers", vm.model.answers[questionIndex].length != 0);
            return vm.model.answers[questionIndex].length;
        }

        function getCheckedCountForSelectOne(questionIndex, form) {
            var countChecked = vm.model.answers[questionIndex].filter(function (item) {
                return item.IsRight;
            }).length;
            form.$setValidity("Only one correct answer", countChecked == 1);
            return countChecked;
        }

        function getCheckedCountForSelectMany(questionIndex, form) {
            var countChecked = vm.model.answers[questionIndex].filter(function (item) {
                return item.IsRight;
            }).length;
            form.$setValidity("At least one correct answer", countChecked > 0);
            return countChecked;
        }

        function isEditingEnabled() {
            return !vm.model.quiz.QuizState || vm.model.quiz.QuizState.Name != 'Scheduled';
        }

        function isDirtyAnswerCount(question) {
            var questionIndex = vm.model.questions.indexOf(question);
            return vm.model.answersDirty[questionIndex].countAnswersDirty;
        }

        function isDirtyAnswerChecked(question) {
            var questionIndex = vm.model.questions.indexOf(question);
            return vm.model.answersDirty[questionIndex].checkedAnswersDirty;
        }

        function isQuestionsFormValid() {
            if (vm.model.questionsForm) {
                var questionCountValid = true;
                if (!vm.model.quiz.QuizState || vm.model.quiz.QuizState.Name != 'Draft') {
                    questionCountValid = (vm.model.questions.length == vm.model.quizBlock.QuestionCount);
                }
                return vm.model.questionsForm.$valid && questionCountValid;
            }
            return false;
        }
    }
})();