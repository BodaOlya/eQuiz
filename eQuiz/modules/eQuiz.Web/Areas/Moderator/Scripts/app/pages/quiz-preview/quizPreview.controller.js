(function (angular) {
    angular.module('equizModule')
           .controller('QuizPreviewController', QuizPreviewController);

    QuizPreviewController.$inject = ['$scope', 'quizPreviewService', '$location', '$route', '$window'];

    function QuizPreviewController($scope, quizPreviewService, $location, $route, $window) {
        var vm = this;

        vm.deleteQuiz = deleteQuiz;

        function deleteQuiz(id) {
            var promise = quizPreviewService.deleteQuiz(id);
            promise.then(function (data) {                
                $window.location.pathname="moderator/quiz/index";
            });
            return promise;
        }
    }
})(angular);