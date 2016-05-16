(function (angular) {
    angular.module('equizModule').controller('LayoutController', LayoutController);

    LayoutController.$inject = ['$scope', '$http', '$sce'];

    function LayoutController($scope, $http, $sce) {
        var vm = this;

        vm.GoToReview = GoToReview;
        vm.GoToStudent = GoToStudent;
        vm.GoToQuiz = GoToQuiz;

        function GoToReview() {
            $http.get('/Areas/Admin/Scripts/review.html')
            .then(function (response) {
                vm.review = $sce.trustAsHtml(response.data);
            });
        };

        function GoToStudent() {
            $http.get('/Areas/Admin/Scripts/student.html')
            .then(function (response) {
                vm.review = $sce.trustAsHtml(response.data);
            });
        };

        function GoToQuiz() {
            $http.get('/Areas/Admin/Scripts/quiz-review.html')
            .then(function (response) {
                vm.review = $sce.trustAsHtml(response.data);
            });
        };

        vm.GoToReview();
    };
})(angular);