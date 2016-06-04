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
        vm.searchInfo = {};
        vm.searchInfo.quizName = "";

        vm.isLoading = true;

        vm.setToLocalStorage = function (durationValue) {
            localStorage.setItem('duration', durationValue);
        }

        activate();
        function activate() {
            var _onSuccess = function (value) {
                vm.allQuizzes = value.data;

                vm.isLoading = false;
                console.log(vm.allQuizzes);
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
                    return quiz.Name.toLowerCase().indexOf(vm.searchInfo.quizName.toLowerCase()) > -1 ? true : false;
                });

            // Filter by quiz internet access.
            if (vm.searchInfo.InternetAccess != undefined) {
                filteredQuizzes = filteredQuizzes.filter(
                    function (quiz) {
                        return quiz.InternetAccess === vm.searchInfo.InternetAccess ? true : false;
                    });
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

    };
})(angular);