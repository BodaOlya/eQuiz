/// <reference path="E:\eQuizEpam\eQuiz\modules\eQuiz.Web\Scripts/libs/angularjs/angular.js" />
(function (angular) {
    var equizModule = angular.module("equizModule");

    equizModule.factory("quizService", ["$http", function ($http) {
        var service = {
            getQuestionsById: getQuestionsByIdAjax,
            sendUserResult: sendUserResultAjax,
            sendQuestionResult: sendQuestionResultAjax,
            setFinishTime: setFinishTimeAjax
        };

        return service;

        function getQuestionsByIdAjax(questionId, duration) {
            var promise = $http({
                method: "GET",
                url: "GetQuestionsByQuizId",
                params: { id: questionId, duration: duration }

            });

            return promise;
        };

        function sendUserResultAjax(passedQuiz) {
            var promise = $http.post("InsertQuizResult", passedQuiz);

            return promise;
        };

        function sendQuestionResultAjax(passedQuestion) {
            var promise = $http.post("InsertQuestionResult", passedQuestion);

            return promise;
        };

        function setFinishTimeAjax(quizPassId) {
            var promise = $http({
                method: "GET",
                url: "SetQuizFinishTime",
                params: { quizPassId: quizPassId }
            });

            return promise;
        };

       
    }]);
})(angular);