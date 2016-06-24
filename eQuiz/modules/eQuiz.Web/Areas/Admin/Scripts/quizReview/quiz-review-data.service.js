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
            updateQuizAnswer: updateQuizAnswerAjax,
            insertQuizAnswer: insertQuizAnswerAjax,
            finalizeQuizReview: finalizeQuizReviewAjax,
            getQuizPassScore : getQuizPassScoreAjax
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

        function updateQuizAnswerAjax(quizPassQuestionId, userScore, evaluatedBy) {            
            var promise = $http({
                url: '/Admin/Quizzes/UpdateAnswer',
                method: "POST",
                params: { quizPassQuestionId: quizPassQuestionId, newMark: userScore, evaluatedBy: evaluatedBy }
            });

            return promise;
        }

        function insertQuizAnswerAjax(quizPassQuestionId, userScore, evaluatedBy) {
            var promise = $http({
                url: '/Admin/Quizzes/InsertAnswer',
                method: "POST",
                params: { quizPassQuestionId: quizPassQuestionId, newMark: userScore, evaluatedBy: evaluatedBy }
            });

            return promise;
        }

        function finalizeQuizReviewAjax(quizId, totalScore) {
            var promise = $http({
                url: '/Admin/Quizzes/FinalizeQuiz',
                method: "POST",
                params: { quizId: quizId, totalScore: totalScore, userId: 1 }
            });

            return promise;
        }

        function getQuizPassScoreAjax(quizPassId) {
            var promise = $http({
                url: '/Admin/Quizzes/GetQuizPassScore',
                method: "POST",
                params: { quizPassId: quizPassId }
            });
            return promise;
        }
    }

})(angular);