(function (angular) {

    angular
        .module("equizModule")
        .factory("studentDataService", studentDataService);

    studentDataService.$inject = ["$http"];

    function studentDataService($http) {

        var service = {
            getStudentInfo: getStudentInfo,
            getStudentQuizzes: getStudentQuizzes,
            getStudentComments: getStudentComments,
            saveProfileInfo: saveProfileInfo
        };

        return service;

        function getStudentInfo(studentId) {
            var promise = $http({
                url: '/Admin/Review/GetStudentInfo',
                method: "GET",
                params: {id: studentId}
            });
            return promise;
        }

        function getStudentQuizzes(studentId) {
            var promise = $http({
                url: '/Admin/Review/GetStudentQuizzes',
                method: "GET",
                params: { id: studentId }
            });
            return promise;
        }

        function getStudentComments(studentId) {
            var promise = $http({
                url: '/Admin/Review/GetStudentQuizzes',
                method: "GET",
                params: { id: studentId }
            });
            //var promise = $http.get("/Admin/Default/GetStudentComments", studentId);
            return promise;
        }

        function saveProfileInfo(saveInfo) {
            var promise = $http.post("/Main/SaveStudentInfo", saveInfo);
            //var promise = $http.post("/Main/SaveStudentComments", studentComments);
            //return promise;
        }
    }

})(angular);