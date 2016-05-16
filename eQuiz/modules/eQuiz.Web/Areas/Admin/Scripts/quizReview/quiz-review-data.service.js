(function (angular) {

    angular
        .module("equizModule")
        .factory("quizReviewDataService", quizReviewDataService);

    quizReviewDataService.$inject = ["$http"];

    function quizReviewDataService($http) {

        var service = {
            getStudent: getStudentAjax,
            getQuiz: getQuizAjax,
            getGroup: getGroupAjax,
            saveQuizReview: saveQuizReviewAjax
        };

        return service;

        function getStudentAjax() {
            var promise = {
                first_name: "Paul",
                last_name: "Dema",
                parent_name: "Michael"                              
            };
            return promise;
        }

        function getQuizAjax() {
            var promise = {
                quiz_name: ".Net junior test",
                start_date: "21.04.2016 10:05",
                end_date: "21.04.2016 10:45",
                questions: [
                  { question_id: 0, question: "What does 'foo' really mean?", answer: "The terms foobar, fubar, foo are sometimes used as placeholder names in computer programming or computer-related documentation.They have been used to name entities such as variables, functions, and commands whose purpose is unimportant and serve only to demonstrate a concept. The words themselves have no meaning in this usage", questionStatus: 0, questionType: 'Manual' },
                  { question_id: 1, question: "What is class?", answer: "A class is a construct that enables you to create your own custom types by grouping together variables of other types, methods and events.", questionStatus: 0, questionType: 'Manual' },
                  { question_id: 2, question: "Select only value types:", answers: [{ id: 0, answer: "integer", isRight: true }, { id: 1, answer: "string", isRight: false }, { id: 2, answer: "boolean", isRight: true }], questionStatus: 0, questionType: 'Auto', userAnswer: [0, 2] },
                  { question_id: 3, question: "What is difference between an interface and an Abstract class?", answers: [{ id: 0, answer: "A class may implement a number of interfaces, but can extend only one abstract class", isRight: true }, { id: 1, answer: "Word 'Abstract' is written in another way than 'interface'", isRight: false }, { id: 2, answer: "I dont know", isRight: false }], questionStatus: 0, questionType: 'Auto', userAnswer: [0, 1] },
                  { question_id: 4, question: "What CLR stands for?", answer: "Common Language Runtime", questionStatus: 0, questionType: 'Manual' }
                ]
            };
            return promise;
        }

        function getGroupAjax() {
            var promise = {
                group_name: ".Net 2015"
            };
            return promise;
        }

        function saveQuizReviewAjax(quizToSave) {
            //TODO save quiz review data
        }
    }

})(angular);