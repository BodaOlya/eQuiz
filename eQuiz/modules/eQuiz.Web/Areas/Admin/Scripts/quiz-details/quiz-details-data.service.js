(function (angular) {

    angular
        .module("equizModule")
        .factory("quizDetailsDataService", quizDetailsDataService);

    quizDetailsDataService.$inject = ["$http"];

    function quizDetailsDataService($http) {

        var service = {
            getQuiz: getQuizAjax,
            getQuizPasses: getQuizPassesAjax
        };

        return service;


        function getQuizAjax(quizId) {
            
            var promise = $http({
                url: '/Admin/QuizDetails/GetQuiz',
                method: "GET",
                params: { id: quizId }
            });

            return promise;
        };

        function getQuizPassesAjax(quizId) {
            //var promise = [
            //    {
            //        id: 1,
            //        student: "First Student",
            //        studentScore: 0,
            //        quizStatus: "Not Passed",
            //        questionDetails: { passed: 0, notPassed: 10, inVerification: 0 }
            //    },
            //    {
            //        id: 1,
            //        student: "Second Student",
            //        studentScore: 2,
            //        quizStatus: "In Verification",
            //        questionDetails: { passed: 0, notPassed: 0, inVerification: 10 }
            //    },
            //    {
            //        id: 1,
            //        student: "Third Student",
            //        studentScore: 3,
            //        quizStatus: "In Verification",
            //        questionDetails: { passed: 0, notPassed: 0, inVerification: 10 }
            //    },
            //    {
            //        id: 1,
            //        student: "Fifth Student",
            //        studentScore: 5,
            //        quizStatus: "In Verification",
            //        questionDetails: { passed: 0, notPassed: 0, inVerification: 10 }
            //    }
            //];

            var promise = $http({
                url: '/Admin/QuizDetails/GetQuizPasses',
                method: "GET",
                params: { id: quizId }
            });
            return promise;
        };

    }

})(angular);