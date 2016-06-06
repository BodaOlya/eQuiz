
(function (angular) {
    angular.module("equizModule")
            .controller("dashboardCtrl", dashboardCtrl);
    dashboardCtrl.$inject = ['$scope', 'dashboardService'];

    function dashboardCtrl($scope, dashboardService) {
        var vm = this;

        vm.allQuizzes = [];
        vm.pagedQuizzes = [];
        vm.page = 0;
        vm.pageSize = 7;
        vm.pagesCount = 1;
        vm.totalCount = 0;
        vm.searchInfo = {
            predicate: null,
            reverse: false,
            searchText: ''
        };

        vm.isLoading = true;

        vm.setToLocalStorage = function (durationValue) {
            localStorage.setItem('duration', durationValue)
        }

        activate();
        function activate() {
            var _onSuccess = function (value) {
                vm.allQuizzes = value.data;

                vm.isLoading = false;
                vm.search(0);
            };
            var _onError = function () {
                vm.isLoading = false;
                console.log("Cannot load quizzes list");
            };

            var quizPromise = dashboardService.getQuizzes();
            quizPromise.then(_onSuccess, _onError);
        };

        vm.search = function (page) {
            vm.page = page || 0;

            // Filter by quiz name.
            var filteredQuizzes = vm.allQuizzes.filter(
                function (quiz) {
                    return quiz.Name.toLowerCase().indexOf(vm.searchInfo.searchText.toLowerCase()) > -1 ? true : false;
                });

            // Filter by quiz internet access.
            if (vm.searchInfo.InternetAccess != undefined) {
                filteredQuizzes = filteredQuizzes.filter(
                    function (quiz) {
                        return quiz.InternetAccess === vm.searchInfo.InternetAccess ? true : false;
                    });
            }

            // Sort by predicate.
            if (vm.searchInfo.predicate != undefined || vm.searchInfo.predicate != null) {
                switch (vm.searchInfo.predicate) {
                    case 'Name': {
                        filteredQuizzes.sort(sortFunc(vm.searchInfo.predicate, !vm.searchInfo.reverse, function (a) { return a.toLowerCase() }));
                        break;
                    }
                    case 'StartDate': {
                        filteredQuizzes.sort(
                            sortFunc(
                                vm.searchInfo.predicate,
                                vm.searchInfo.reverse,
                                function (unix_time) {
                                    return new Date(unix_time);
                                }));
                        break;
                    }
                    case 'Duration': {
                        filteredQuizzes.sort(sortFunc('TimeLimitMinutes', vm.searchInfo.reverse, function (minutes) { return minutes == null ? 0 : minutes; }));
                        break;
                    }
                }
            }

            vm.totalCount = filteredQuizzes.length;
            vm.pagesCount = Math.ceil(vm.totalCount / vm.pageSize);

            if (vm.totalCount > vm.page * vm.pageSize) {
                vm.pagedQuizzes = filteredQuizzes.slice(vm.page * vm.pageSize, vm.page * vm.pageSize + vm.pageSize);
            }
            else {
                vm.pagedQuizzes = filteredQuizzes;
            }
        };


        vm.showOrderArrow = function (predicate) {
            if (vm.searchInfo.predicate === predicate) {
                return vm.searchInfo.reverse ? '▲' : '▼';
            }
            return '';
        };

        vm.sortBy = function (predicate) {
            vm.searchInfo.reverse = (vm.searchInfo.predicate === predicate) ? !vm.searchInfo.reverse : false;
            vm.searchInfo.predicate = predicate;

            vm.search();
        };

        sortFunc = function (field, reverse, primer) {
            var key = function (x) { return primer ? primer(x[field]) : x[field] };

            return function (a, b) {
                var A = key(a), B = key(b);
                return ((A < B) ? -1 : ((A > B) ? 1 : 0)) * [-1, 1][+!!reverse];
            }
        };

    };
})(angular);