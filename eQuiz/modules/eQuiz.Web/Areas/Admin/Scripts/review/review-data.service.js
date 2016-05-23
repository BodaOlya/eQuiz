(function (angular) {

    angular
        .module("equizModule")
        .factory("reviewDataService", reviewDataService);

    reviewDataService.$inject = ["$http"];

    function reviewDataService($http) {

        var service = {
            getStudents: getStudentsAjax,
        };

        return service;

        function getStudentsAjax() {
            var promise = $http.get('/Admin/Review/GetStudentsList');
            console.log(promise);
            return promise;
        }
    }

})(angular);