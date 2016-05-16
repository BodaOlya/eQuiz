(function () {
    angular.module('quizModule')
           .factory('questionService', questionService);
    questionService.$inject = ['$http'];

    function questionService($http) {
        var questionTypes = [];
        var quizQuestions = {};

        function getTypes() {
            var promise = $http.get('/Moderator/QuizQuestion/GetQuestionTypes');
            promise.then(function(promise){
                questionTypes = promise.data;
            });

            return promise;
        }

        function saveQuestions(questions) {
            var promise = $http.post("/Moderator/QuizQuestion/Save", questions);
            promise.then(function(response){
                quizQuestions = response.data;
            });

            return promise;
        }

        function getQuestions(quizId) {
            return $http.get("/Moderator/QuizQuestion/Get/" + quizId);
            promise.then(function(response){
                quizQuestions = response.data;
            });

            return promise;
        }

        return {
            questionTypes: questionTypes,
            quizQuestions: quizQuestions,
            getQuestionTypes: getTypes,
            saveQuestions: saveQuestions,
            getQuestions: getQuestions
        };

    }
})();