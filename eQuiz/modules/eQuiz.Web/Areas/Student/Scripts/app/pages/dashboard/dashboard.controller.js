
(function (angular) {
    var equizModule = angular.module("equizModule");

    equizModule.controller("dashboardCtrl", ["$scope", "dashboardService", function ($scope, dashboardService) {
        $scope.allQuizzes = [];
        $scope.pagedQuizzes = [];
        $scope.page = 0;
        $scope.pageSize = 7;
        $scope.pagesCount = 1;
        $scope.totalCount = 0;
        $scope.searchInfo = {};
        $scope.searchInfo.quizName = "";

        $scope.isLoading = true;

        $scope.setToLocalStorage = function (durationValue) {
            localStorage.setItem('duration', durationValue)
        }

        activate();
        function activate() {
            var _onSuccess = function (value) {
                $scope.allQuizzes = value.data;

                $scope.isLoading = false;
                console.log($scope.allQuizzes);
                $scope.search(0);
            };
            var _onError = function () {
                $scope.isLoading = false;
                console.log("Cannot load quizzes list");
            };

            var quizPromise = dashboardService.getQuizzes();
            quizPromise.then(_onSuccess, _onError);
        };

        $scope.search = function (page) {
            $scope.page = page || 0;

            // Filter by quiz name.
            var filteredQuizzes = $scope.allQuizzes.filter(
                function (quiz) {
                    return quiz.Name.toLowerCase().indexOf($scope.searchInfo.quizName.toLowerCase()) > -1 ? true : false;
                });

            // Filter by quiz internet access.
            if ($scope.searchInfo.InternetAccess != undefined) {
                filteredQuizzes = filteredQuizzes.filter(
                    function (quiz) {
                        return quiz.InternetAccess === $scope.searchInfo.InternetAccess ? true : false;
                    });
            }


            $scope.totalCount = filteredQuizzes.length;
            $scope.pagesCount = Math.ceil($scope.totalCount / $scope.pageSize);
            if ($scope.totalCount > $scope.page * $scope.pageSize) {
                $scope.pagedQuizzes = filteredQuizzes.slice($scope.page * $scope.pageSize, $scope.page * $scope.pageSize + $scope.pageSize);
            }
            else {
                $scope.pagedQuizzes = filteredQuizzes;
            }
        };

    }]);
})(angular);