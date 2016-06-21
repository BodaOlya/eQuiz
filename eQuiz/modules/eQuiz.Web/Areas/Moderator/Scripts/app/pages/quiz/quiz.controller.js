(function () {
    angular.module("equizModule")
           .controller("QuizController", QuizController);
    QuizController.$inject = ['$scope', 'quizService', '$location', 'questionService', '$timeout', 'mvcLocation'];

    function QuizController($scope, quizService, $location, questionService, $timeout, mvcLocation) {
        var vm = this;
        vm.loadingVisible = false;
        vm.errorMessageVisible = false;
        vm.successMessageVisible = false;
        vm.showSuccess = showSuccess;
        vm.showError = showError;
        vm.isExistingQuestionEnable = false;
        vm.quizSearch = "";
        vm.selectedQuizIdForAddQuestion = 0;
        vm.tab = 'quiz';
        vm.save = save;
        vm.switchTab = switchTab;
        vm.saveCanExecute = saveCanExecute;
        vm.model = {
            quiz: { QuizTypeId: 1, DurationHours: 0, DurationMinutes: 0 },
            states: [],
            quizzesForCopy: [],
            quizBlock: { QuestionCount: 0 },
            questions: [],
            answers: [],
            tags: [],
            orderArray: [],
            questionTypes: [],
            answersDirty: [],
            questionsForAdding: {},
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
        vm.showAddExistingQuestion = showAddExistingQuestion;
        vm.setQuizIdForAddQuestion = setQuizIdForAddQuestion;
        vm.searchTextChange = searchTextChange;
        vm.closeAddingQuestionWindow = closeAddingQuestionWindow;
        vm.getQuestionsCopyForAddindQuestion = getQuestionsCopyForAddindQuestion;
        vm.GetCountSelectedQuestions = GetCountSelectedQuestions;
        vm.AddExistingQuestions = AddExistingQuestions;
        vm.getAnswersForQuestion = getAnswersForQuestion;
        vm.getTagsForQuestion = getTagsForQuestion;
        vm.getFirstCheckedAnswerIndex = getFirstCheckedAnswerIndex;

        vm.toggleQuizzesForCopy = toggleQuizzesForCopy;
        vm.quizzesForCopyVisible = false;
        vm.getQuestionsCopy = getQuestionsCopy;
        vm.selectQuizCopy = selectQuizCopy;
        vm.selectedQuizCopy = { Id: 0, Name: 'New' };
        vm.showLoading = showLoading;
        vm.hideLoading = hideLoading;
        vm.deleteQuiz = deleteQuiz;
        vm.deleteCanExecute = deleteCanExecute;
        vm.archiveQuiz = archiveQuiz;
        vm.archiveQuizCanExecute = archiveQuizCanExecute;
        vm.initQuizFromData = initQuizFromData;
        vm.revalidateInputs = revalidateInputs;

        vm.questionsPagingInfo = {
            currentPage: 1,
            questionsPerPage: 10,
            questionsTotal: 0
        };

        vm.getNumeration = getNumeration;
        vm.getQuestionsPage = getQuestionsPage;

        activate();

        function activate() {
            if (mvcLocation.search("id")) {
                vm.showLoading();
                vm.getQuestions(mvcLocation.search("id"));
                quizService.get(mvcLocation.search("id")).then(function (data) {
                    vm.initQuizFromData(data);
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
                vm.model.archiveState = vm.model.states.filter(function (element, i, array) {
                    return element.Name == 'Archived';
                })[0];
                var index = vm.model.states.indexOf(vm.model.archiveState);
                vm.model.states.splice(index, 1);
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
            if (vm.model.locked) {
                return false;
            }
            if (vm.model.quiz.QuizState && vm.model.quiz.QuizState.Name == 'Scheduled') {
                return false;
            }
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

        function save(callback) {
            if (!vm.saveCanExecute()) {
                vm.revalidateInputs();
                return;
            }

            vm.showLoading();
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
                    vm.hideLoading();
                    vm.showSuccess();
                    if (callback) {
                        callback();
                    }
                }, function (response) {
                    vm.hideLoading();
                    vm.showError();
                });
            }

            function saveQuiz() {
                quizService.save({ quiz: vm.model.quiz, block: vm.model.quizBlock, latestChange: vm.model.latestChange}).then(function (data) {
                    initQuizFromData(data);
                    saveQuestions();
                }, function (data) {
                    vm.hideLoading();
                    vm.showError();
                });
            }


        }

        function revalidateInputs() {
            var elements = document.getElementsByTagName("input");
            angular.element(elements).triggerHandler("blur");
            elements = document.getElementsByTagName("select");
            angular.element(elements).triggerHandler("blur");
            elements = document.getElementsByTagName("textarea");
            angular.element(elements).triggerHandler("blur");
        }

        function initQuizFromData(data) {
                    vm.model.quiz = data.data.quiz;
                    if (data.data.quiz.StartDate) {
                        vm.model.quiz.StartDate = new Date(vm.model.quiz.StartDate);
                    }
                    vm.model.quiz.DurationMinutes = vm.model.quiz.TimeLimitMinutes % 60;
                    vm.model.quiz.DurationHours = (vm.model.quiz.TimeLimitMinutes - vm.model.quiz.TimeLimitMinutes % 60) / 60;
                    vm.model.quizBlock = data.data.block;
                    vm.model.latestChange = data.data.latestChange;
                    vm.model.latestChange.StartDate = new Date(vm.model.latestChange.StartDate);
                    vm.model.latestChange.LastChangeDate = new Date(vm.model.latestChange.LastChangeDate);
                    vm.model.locked = data.data.locked;
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

        function setQuestionType(question, typeId, form) {
            question.QuestionTypeId = typeId;

            var questionIndex = vm.model.questions.indexOf(question);

            var countChecked = vm.model.answers[questionIndex].filter(function (item) {
                return item.IsRight;
            }).length;

            if (typeId == 1) {
                if (vm.model.answers[questionIndex].length == 0) {
                    vm.model.answers[questionIndex].push({
                        Id: 0,
                        QuestionId: 0,
                        AnswerText: "",
                        AnswerOrder: 1
                    });
                }
                else {
                    if (countChecked == 0) {
                        vm.model.answers[questionIndex][0].IsRight = true;
                    }
                }
            }

            if (typeId == 2 && countChecked > 1) {
                var temp = vm.model.answers[questionIndex].filter(function (item) {
                    return item.IsRight;
                })[0];
                var checkedIndex = vm.model.answers[questionIndex].indexOf(temp);
                for (var i = checkedIndex + 1; i < vm.model.answers[questionIndex].length; i++) {
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
                IsActive: true
            });

            var answersForQuestion = [
                {
                    Id: 0,
                    QuestionId: 0,
                    AnswerText: "",
                    AnswerOrder: 1,
                    IsRight: true
                }
            ];

            vm.model.answers.push(answersForQuestion);

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
                IsRight: false
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
                    var secondElementIndex = vm.model.tags[i].slice(0, j).indexOf(vm.model.tags[i][j]);
                    if (secondElementIndex == -1) {
                        tagArray.push({
                            Id: 0,
                            Name: vm.model.tags[i][j]
                        });
                    }
                }
                if (tagArray.length == 0) {
                    tagArray.push(null);
                }
                tags.push(tagArray);
            }

            var answers = [];

            for (var i = 0; i < vm.model.answers.length; i++) {

                var answerArray = [];
                if (vm.model.questions[i].QuestionTypeId == 1) {
                    vm.model.answers[i][vm.getFirstCheckedAnswerIndex(i)].AnswerOrder = 1;
                    answerArray.push(vm.model.answers[i][vm.getFirstCheckedAnswerIndex(i)]);
                }
                else {
                    for (var j = 0; j < vm.model.answers[i].length; j++) {
                        vm.model.answers[i][j].AnswerOrder = j + 1;
                        answerArray.push(vm.model.answers[i][j]);
                    }
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
            if (vm.model.locked) {
                return false;
            }
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

        function deleteQuiz() {
            vm.showLoading();
            quizService.deleteQuiz(vm.model.quiz.Id).then(function () {
                vm.showSuccess();
                window.location.href = '/moderator/quiz';
            }, function () {
                vm.showError();
                vm.hideLoading();
            });
        }

        function archiveQuiz() {
            vm.showLoading();
            vm.model.quiz.QuizState = vm.model.archiveState;
            vm.save(function () {
                showLoading();
                $timeout(function () {
                    window.location.href = '/moderator/quiz';
                }, 2000);                
            });
        }

        function archiveQuizCanExecute() {
            if (vm.model.locked) {
                return false;
            }
            return vm.model.quiz.Id && vm.model.quiz.QuizState && vm.model.quiz.QuizState.Name == 'Opened' && vm.saveCanExecute();
        }

        function deleteCanExecute() {
            if (vm.model.locked) {
                return false;
            }
            return vm.model.quiz.Id && vm.model.quiz.QuizState && vm.model.quiz.QuizState.Name == 'Scheduled';
        }

        function showAddExistingQuestion() {
            vm.isExistingQuestionEnable = true;
        }

        function setQuizIdForAddQuestion(item) {
            vm.selectedQuizIdForAddQuestion = item.Id;
            vm.getQuestionsCopyForAddindQuestion(vm.selectedQuizIdForAddQuestion);
        }

        function searchTextChange() {
            vm.selectedQuizIdForAddQuestion = 0;
            vm.questionsForAdding = {};
            vm.model.questionsForAdding = {};
        }

        function closeAddingQuestionWindow() {
            vm.quizSearch = "";
            vm.selectedQuizIdForAddQuestion = 0;
            vm.questionsForAdding = {};
            vm.isExistingQuestionEnable = false;
            vm.model.questionsForAdding = {};
            vm.questionsPagingInfo.questionsTotal = 0;
        }

        function getQuestionsCopyForAddindQuestion(quizId) {
            vm.showLoading();
            questionService.getQuestionsCopy(quizId).then(function (response) {
                var modelFromServer = response.data;

                vm.model.questionsForAdding = vm.toViewModel(modelFromServer);
                for (var i = 0; i < vm.model.questionsForAdding.questions.length; i++) {
                    vm.model.questionsForAdding.questions[i].checked = false;
                    vm.model.questionsForAdding.questions[i].isExpanded = false;
                }
                vm.questionsPagingInfo.questionsTotal = vm.model.questionsForAdding.questions.length;
                vm.questionsPagingInfo.currentPage = 1;
                vm.hideLoading();
            });
        }

        function GetCountSelectedQuestions() {
            return vm.model.questionsForAdding.questions ? vm.model.questionsForAdding.questions.filter(function (item) {
                return item.checked;
            }).length : 0;
        }

        function getNumeration(index) {
            var result = (index + 1) + ((vm.questionsPagingInfo.currentPage - 1) * vm.questionsPagingInfo.questionsPerPage);
            return result;
        };

        function getQuestionsPage() {
            if (vm.model.questionsForAdding.questions === undefined) {
                return [];
            }
            return vm.model.questionsForAdding.questions.slice(((vm.questionsPagingInfo.currentPage - 1) *
                vm.questionsPagingInfo.questionsPerPage), ((vm.questionsPagingInfo.currentPage) *
                vm.questionsPagingInfo.questionsPerPage));
        }

        function AddExistingQuestions() {
            for (var i = 0; i < vm.model.questionsForAdding.questions.length; i++) {
                if (vm.model.questionsForAdding.questions[i].checked) {

                    delete vm.model.questionsForAdding.questions[i]['checked'];

                    delete vm.model.questionsForAdding.questions[i]['isExpanded'];

                    vm.model.questions.push(vm.model.questionsForAdding.questions[i]);

                    vm.model.answers.push(vm.model.questionsForAdding.answers[i]);

                    vm.model.tags.push(vm.model.questionsForAdding.tags[i]);

                    vm.model.orderArray.push({
                        reverse: false,
                        predicate: ""
                    });

                    vm.model.answersDirty.push({
                        countAnswersDirty: false,
                        checkedAnswersDirty: false
                    });
                }
            }
            vm.closeAddingQuestionWindow();
        }

        function getAnswersForQuestion(question){
            var questionIndex = vm.model.questionsForAdding.questions.indexOf(question);

            return vm.model.questionsForAdding.answers[questionIndex];
        }

        function getTagsForQuestion(question) {
            var questionIndex = vm.model.questionsForAdding.questions.indexOf(question);

            return vm.model.questionsForAdding.tags[questionIndex].join(', ');
        }

        function getFirstCheckedAnswerIndex(questionIndex) {

            var temp = vm.model.answers[questionIndex].filter(function (item) {
                return item.IsRight;
            })[0];

            var checkedIndex = vm.model.answers[questionIndex].indexOf(temp);

            return checkedIndex;
        }
    }
})();