(function () {
    angular.module('equizModule')
           .factory('questionService', questionService);
    questionService.$inject = ['$http', '$q'];

    function questionService($http, $q) {
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

        function getQuestionsCopy(quizId) {
            var deferred = $q.defer();
            getQuestions(quizId).then(function (data){
                data.data.questions.forEach(function(item, i, array){
                    item.Id = 0;
                });
                data.data.answers.forEach(function (item, i, array) {
                    item.forEach(function (innerItem, j, innerArray) {
                        innerItem.Id = 0;
                    });
                });
                data.data.tags.forEach(function(item, i, array){
                    item.forEach(function(innerItem, j, innerArray){
                        innerItem.Id = 0;
                    });
                });

                deferred.resolve(data);
            });

            return deferred.promise;
        }

        return {
            questionTypes: questionTypes,
            quizQuestions: quizQuestions,
            getQuestionTypes: getTypes,
            saveQuestions: saveQuestions,
            getQuestions: getQuestions,
            getQuestionsCopy: getQuestionsCopy
        };

    }
})();