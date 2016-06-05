
(function (angular) {
    var equizModule = angular.module("equizModule");

    equizModule.controller("dashboardCtrl", ["$scope", "dashboardService", function ($scope, dashboardService) {
        $scope.allQuizzes = [];
        $scope.pagedQuizzes = [];
        $scope.page = 0;
        $scope.pageSize = 7;
        $scope.pagesCount = 1;
        $scope.totalCount = 0;
        $scope.searchInfo = {
            predicate: null,
            reverse: false,
            searchText: ''
        };

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
                    return quiz.Name.toLowerCase().indexOf($scope.searchInfo.searchText.toLowerCase()) > -1 ? true : false;
                });

            // Filter by quiz internet access.
            if ($scope.searchInfo.InternetAccess != undefined) {
                filteredQuizzes = filteredQuizzes.filter(
                    function (quiz) {
                        return quiz.InternetAccess === $scope.searchInfo.InternetAccess ? true : false;
                    });
            }

            // Sort by predicate.
            if ($scope.searchInfo.predicate != undefined || $scope.searchInfo.predicate != null) {
                switch ($scope.searchInfo.predicate) {
                    case 'Name': {
                        filteredQuizzes.sort(sortFunc($scope.searchInfo.predicate, !$scope.searchInfo.reverse, function (a) { return a.toLowerCase() }));
                        break;
                    }
                    case 'StartDate': {
                        filteredQuizzes.sort(
                            sortFunc(
                                $scope.searchInfo.predicate,
                                $scope.searchInfo.reverse,
                                function (unix_time) {
                                    return new Date(unix_time);
                                }));
                        break;
                    }
                    case 'Duration': {
                        filteredQuizzes.sort(sortFunc('TimeLimitMinutes', $scope.searchInfo.reverse, function (minutes) { return minutes == null ? 0 : minutes; }));
                        break;
                    }
                }
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


        $scope.showOrderArrow = function (predicate) {
            if ($scope.searchInfo.predicate === predicate) {
                return $scope.searchInfo.reverse ? '▲' : '▼';
            }
            return '';
        };

        $scope.sortBy = function (predicate) {
            $scope.searchInfo.reverse = ($scope.searchInfo.predicate === predicate) ? !$scope.searchInfo.reverse : false;
            $scope.searchInfo.predicate = predicate;

            $scope.search();
        };

        sortFunc = function (field, reverse, primer) {
            var key = function (x) { return primer ? primer(x[field]) : x[field] };

            return function (a, b) {
                var A = key(a), B = key(b);
                return ((A < B) ? -1 : ((A > B) ? 1 : 0)) * [-1, 1][+!!reverse];
            }
        };

    }]);
})(angular);