(function () {
    angular.module('equizModule')
           .factory('quizService', quizService);

    quizService.$inject = ['$http'];
    function quizService($http) {

        return {
            save: save,
            get: get,
            isNameUnique: isNameUnique,
            getStates: getStates,
            getQuizzesForCopy: getQuizzesForCopy,
            getOpenQuizzes: getOpenQuizzes,
            schedule: schedule
        }

        function get(id) {
            return $http.get("/moderator/quiz/get?id=" + id.toString());
        }

        function save(quiz) {
            return $http.post("/quiz/save", quiz);
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

        function getOpenQuizzes() {
            return $http.get("/quiz/GetOpenQuizzes");
        }

        function getStates() {
            return $http.get("/quiz/GetStates");
        }

        function schedule(quiz) {
            return $http.post("/quiz/Schedule", quiz);
        }
    }
})();