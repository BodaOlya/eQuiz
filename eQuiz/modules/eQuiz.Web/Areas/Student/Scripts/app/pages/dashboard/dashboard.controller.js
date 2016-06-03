
(function (angular) {
    var equizModule = angular.module("equizModule");

    equizModule.controller("dashboardCtrl", ["$scope", "dashboardService", function ($scope, dashboardService) {
        $scope.allQuizzes = [];
        $scope.pagedQuizzes = [];
        $scope.page = 0;
        $scope.pageSize = 7;
        $scope.pagesCount = 1;
        $scope.search = {};
        $scope.search.quizName = "";

        $scope.isLoading = true;

        $scope.setToLocalStorage = function (durationValue) {
            localStorage.setItem('duration', durationValue)
        }

        activate();
        function activate() {
            var quizPromise = dashboardService.getQuizzes();
            quizPromise.success(function (data) {
                $scope.allQuizzes = data;
                $scope.isLoading = false;
            });
        };

        $scope.search = function (page) {
            $scope.page = page || 0;

            var _onSuccess = function (value) {
                $scope.allQuizzes = value.data;
                $scope.totalCount = $scope.allQuizzes.length;
                $scope.pagesCount = Math.ceil($scope.totalCount / $scope.pageSize);
                
                // ToDo
                // Filtering and sorting.
                //var filteredQuizzes = $scope.allQuizzes.filter(function (quiz) { return quiz.Name.indexOf($scope.search.quizName); });
                $scope.pagedQuizzes = $scope.allQuizzes.slice($scope.page * $scope.pageSize, $scope.page * $scope.pageSize + $scope.pageSize);
                

                $scope.isSearching = false;
                console.log($scope.pagedQuizzes);
            };
            var _onError = function () {
                $scope.isSearching = false;
                console.log("Cannot load quizzes list");
            };

            $scope.isSearching = true;

            dashboardService.getQuizzes()
                .then(_onSuccess, _onError);
        };

        $scope.search();

    }]);
})(angular);