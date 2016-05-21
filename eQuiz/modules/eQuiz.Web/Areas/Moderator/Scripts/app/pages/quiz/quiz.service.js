(function () {
    angular.module('equizModule')
           .factory('quizService', quizService);

    quizService.$inject = ['$http'];
    function quizService($http) {
        quiz = {};

        return {
            save: save,
            quiz: quiz,
            get: get,
            isNameUnique: isNameUnique,
            getStates: getStates,
            getQuizzesForCopy: getQuizzesForCopy
        }

        function get(id) {
            var promise = $http.get("/moderator/quiz/get?id=" + id.toString());
            promise.then(function (data) {
                quiz.Id = data.data.quiz.Id;
                quizBlock = data.data.quizBlock;
            });
            return promise;
        }

        function save(quiz) {
            var promise = $http.post("/quiz/save", quiz);
            promise.then(function (data) {
                quiz.Id = data.data.quiz.Id;
            });
            return promise;
        }

        function isNameUnique(name, id) {
            if (id) {
                return $http.get("/quiz/IsNameUnique?name=" + name.toString() + "&id=" + id.toString());
            }
            else {
                return $http.get("/quiz/IsNameUnique?name=" + name.toString());
            }
        }

        function getQuizzesForCopy() {
            return $http.get("/quiz/GetQuizzesForCopy");
        }

        function getStates() {
            return $http.get("/quiz/GetStates");
        }
    }
})();