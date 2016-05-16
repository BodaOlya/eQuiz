(function () {

    angular.module('quizModule')
    .controller('QuestionController', QuestionController);

    QuestionController.$inject = ["questionService", "$location"];

    function QuestionController(questionService, $location) {
        var vm = this;
        vm.questions = [];
        vm.answers = [];
        vm.tags = [];
        vm.orderArray = [];
        vm.questionTypes = [];

        vm.setQuestionType = setQuestionType;
        vm.addNewQuestion = addNewQuestion;
        vm.addNewAnswer = addNewAnswer;
        vm.checkAnswerForSelectOne = checkAnswerForSelectOne;
        vm.deleteAnswer = deleteAnswer;
        vm.order = order;
        vm.showOrderArrow = showOrderArrow;
        vm.toViewModel = toViewModel;
        vm.toServerModel = toServerModel;
        vm.saveQuestions = saveQuestions;
        vm.getQuestions = getQuestions;
        vm.getAnswerCount = getAnswerCount;
        vm.getCheckedCountForSelectOne = getCheckedCountForSelectOne;
        vm.getCheckedCountForSelectMany = getCheckedCountForSelectMany;

        if ($location.search().id) {
            vm.getQuestions($location.search().id);
        }

        questionService.getQuestionTypes().then(function (responce) {
            vm.questionTypes = responce.data;
        });

        function setQuestionType(question, typeId, form) {
            question.QuestionTypeId = typeId;

            form.$setValidity("No answers", true);
            form.$setValidity("Only one correct answer", true);
            form.$setValidity("At least one correct answer", true);
        }

        function addNewQuestion() {
            vm.questions.push({
                Id: 0,
                QuestionTypeId: vm.questionTypes[0].Id,
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

            vm.answers.push([]);

            vm.tags.push([]);

            vm.orderArray.push({
                reverse: false,
                predicate: ""
            });
        }

        function addNewAnswer(question, questionIndex) {
            var answerOrder = vm.answers[questionIndex].length + 1;
            vm.answers[questionIndex].push({
                Id: 0,
                QuestionId: question.Id,
                AnswerText: "",
                AnswerOrder: answerOrder,
                IsRight: false,
                Question: null,
                UserAnswers: null
            });
        }

        function checkAnswerForSelectOne(answer, question) {
            var questionIndex = vm.questions.indexOf(question);
            for (var i = 0; i < vm.answers[questionIndex].length; i++) {
                vm.answers[questionIndex][i].IsRight = false;
            }
            answer.IsRight = true;
        }

        function deleteAnswer(answer, question) {
            var questionIndex = vm.questions.indexOf(question);
            var answerIndex = vm.answers[questionIndex].indexOf(answer);
            vm.answers[questionIndex].splice(answerIndex, 1);
        }

        function order(questionIndex, name) {
            vm.orderArray[questionIndex].reverse = (vm.orderArray[questionIndex].predicate === name) ? !vm.orderArray[questionIndex].reverse : false;
            vm.orderArray[questionIndex].predicate = name;
        }

        function showOrderArrow(questionIndex, name) {
            if (vm.orderArray[questionIndex].predicate === name) {
                return vm.orderArray[questionIndex].reverse ? '▼' : '▲';
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
            for (var i = 0; i < vm.tags.length; i++) {

                var tagArray = [];
                for (var j = 0; j < vm.tags[i].length; j++) {
                    tagArray.push({
                        Id: 0,
                        Name: vm.tags[i][j],
                        QuestionTags: null
                    });
                }
                if (tagArray.length == 0) {
                    tagArray.push(null);
                }
                tags.push(tagArray);
            }

            var answers = [];

            for (var i = 0; i < vm.answers.length; i++) {

                var answerArray = [];
                for (var j = 0; j < vm.answers[i].length; j++) {
                    answerArray.push(vm.answers[i][j]);
                }
                if (answerArray.length == 0) {
                    answerArray.push(null);
                }
                answers.push(answerArray);
            }

            return {
                questions: vm.questions,
                tags: tags,
                answers: answers
            };
        }

        function saveQuestions(quizId) {
            var quizQuestionVM = vm.toServerModel();
            quizQuestionVM.id = quizId;
            questionService.saveQuestions(quizQuestionVM).then(function (response) {
                var modelFromServer = response.data;

                var model = vm.toViewModel(modelFromServer);
                vm.questions = model.questions;
                vm.answers = model.answers;
                vm.tags = model.tags;
            });
        }

        function getQuestions(quizId) {
            questionService.getQuestions(quizId).then(function (response) {
                var modelFromServer = response.data;

                var model = vm.toViewModel(modelFromServer);
                vm.questions = model.questions;
                vm.answers = model.answers;
                vm.tags = model.tags;
                vm.orderArray = Array.apply(null, Array(vm.questions.length)).map(function () {
                    return {
                        reverse: false,
                        predicate: ""
                    };
                });
            });
        }

        function getAnswerCount(questionIndex, form) {
            form.$setValidity("No answers", vm.answers[questionIndex].length != 0);
            return vm.answers[questionIndex].length;
        }

        function getCheckedCountForSelectOne(questionIndex, form) {
            var countChecked = vm.answers[questionIndex].filter(function (item) {
                return item.IsRight;
            }).length;
            form.$setValidity("Only one correct answer", countChecked == 1);
            return countChecked;
        }

        function getCheckedCountForSelectMany(questionIndex, form) {
            var countChecked = vm.answers[questionIndex].filter(function (item) {
                return item.IsRight;
            }).length;
            form.$setValidity("At least one correct answer", countChecked > 0);
            return countChecked;
        }
    }
})();