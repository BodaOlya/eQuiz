(function (angular) {

    angular
        .module("equizModule")
        .factory("quizReviewDataService", quizReviewDataService);

    quizReviewDataService.$inject = ["$http"];

    function quizReviewDataService($http) {

        var service = {
            getStudent: getStudentAjax,
            getQuizBlock: getQuizBlockAjax,
            getQuizInfo: getQuizInfoAjax,
            saveQuizReview: saveQuizReviewAjax,
            finalizeQuizReview: finalizeQuizReviewAjax
        };

        return service;

        function getStudentAjax(studentId) {
            var promise = $http({
                url: '/Admin/Student/GetStudentInfo',
                method: "GET",
                params: { id: studentId }
            });
            
            return promise;
        }

        function getQuizBlockAjax(quizeId) {
            var promise = $http({
                url: '/Admin/Quizzes/GetStudentQuiz',
                method: "GET",
                params: { quizPassId: quizeId }
            });

            return promise;
        }

        function getQuizInfoAjax(quizeId) {
            var promise = $http({
                url: '/Admin/Quizzes/GetQuizInfo',
                method: "GET",
                params: { quizPassId: quizeId }
            });

            return promise;
        }

        function saveQuizReviewAjax(quizToSave) {
            //TODO save quiz review data
        }

        function finalizeQuizReviewAjax(quizToFinalize) {
            // TODO finalize quiz
        }
    }

})(angular);