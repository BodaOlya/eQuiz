(function (angular) {
    angular.module('equizModule')
           .factory('quizPreviewService', quizPreviewService);

    quizPreviewService.$inject = ['$http'];

    function quizPreviewService($http) {

        return {
            deleteQuiz: deleteQuiz
        }

        function deleteQuiz(id) {            
            return $http.post("/moderator/quiz/DeleteQuizById?id=" + id.toString());
        }
    }
})(angular);