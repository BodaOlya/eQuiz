(function (angular) {

    angular
        .module('quizzesModule')
        .service('quizzesService', quizzesService);

    quizzesService.$inject = ['$http'];

    function quizzesService($http) {

        var service = {
            getQuizzesPage: getQuizzesPage
        };

        return service;              

        function getQuizzesPage(pagingInfo) {
            var promise = $http({
                url: '/Quiz/GetQuizzesPage',
                method: 'GET',
                params: pagingInfo
            }).then(populateResponse);
            return promise;
        };

        function getQuizzes() {
            if (localStorage.quizzes) {
                return JSON.parse(localStorage.quizzes);
            } else {
                return [];
            }
        };

        function populateResponse(response) {
            return response.data;
        };
    };
})(angular);