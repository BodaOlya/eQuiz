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
            var promise = $http.get("/Admin/Default/GetStudentComments", studentId);
            return promise;

            //return [
            //    {
            //        date: '15.04.2016',
            //        author: 'Volodymyr',
            //        text: 'Responsible, initiative student with excellent knowledge of necessary frameworks'
            //    },
            //    {
            //        date: '03.05.2016',
            //        author: 'Ivan',
            //        text: 'Demonstrates deep theoretical knowledge'
            //    },
            //                    {
            //                        date: '22.04.2016',
            //                        author: 'Volodymyr',
            //                        text: 'Responsible, initiative student with excellent knowledge of necessary frameworks'
            //                    },
            //    {
            //        date: '10.05.2016',
            //        author: 'Ivan',
            //        text: 'Demonstrates deep theoretical knowledge'
            //    },
            //                    {
            //                        date: '10.04.2016',
            //                        author: 'Volodymyr',
            //                        text: 'Responsible, initiative student with excellent knowledge of necessary frameworks'
            //                    },
            //    {
            //        date: '10.05.2016',
            //        author: 'Ivan',
            //        text: 'Demonstrates deep theoretical knowledge'
            //    },
            //                    {
            //                        date: '10.04.2016',
            //                        author: 'Volodymyr',
            //                        text: 'Responsible, initiative student with excellent knowledge of necessary frameworks'
            //                    },
            //    {
            //        date: '11.05.2016',
            //        author: 'Ivan',
            //        text: 'Demonstrates deep theoretical knowledge'
            //    },
            //                    {
            //                        date: '15.04.2016',
            //                        author: 'Volodymyr',
            //                        text: 'Responsible, initiative student with excellent knowledge of necessary frameworks'
            //                    },
            //    {
            //        date: '12.05.2016',
            //        author: 'Ivan',
            //        text: 'Demonstrates deep theoretical knowledge'
            //    },
            //                    {
            //                        date: '12.04.2016',
            //                        author: 'Volodymyr',
            //                        text: 'Responsible, initiative student with excellent knowledge of necessary frameworks'
            //                    },
            //    {
            //        date: '13.05.2016',
            //        author: 'Ivan',
            //        text: 'Demonstrates deep theoretical knowledge'
            //    }
            //];
        }

        function saveProfileInfo(studentInfo, studentComments) {
            //var promise = $http.post("/Main/SaveStudentInfo", studentInfo);
            //var promise = $http.post("/Main/SaveStudentComments", studentComments);
            //return promise;
        }
    }

})(angular);