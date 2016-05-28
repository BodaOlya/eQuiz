(function (angular) {

    angular
        .module("equizModule")
        .factory("quizzesDataService", quizzesDataService);

    quizzesDataService.$inject = ["$http"];

    function quizzesDataService($http) {

        var service = {            
            getQuizzes: getQuizzesAjax,
        };

        return service;

        function getQuizzesAjax(quizeId) {
            var promise = $http.get('/Admin/Quizzes/GetQuizzesList');           
            return promise;
        }
    }

})(angular);