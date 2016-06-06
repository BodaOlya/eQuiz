(function (angular) {

    angular
        .module('equizModule')
        .controller('QuizzesController', QuizzesController);        

    QuizzesController.$inject = ['$scope', 'quizzesService'];

    function QuizzesController($scope, quizzesService) {
        var vm = this;

        //fields
        vm.quizzes = [];

        vm.isLoading = true;

        //paging with sorting 
        vm.pagingInfo = {
            currentPage: 1,
            quizzesPerPage: 5,
            predicate: 'Name',
            reverse: false,
            quizzesTotal: 0,
            searchText: null,
            selectedStatus: 'All'
        };

        //constants for dropdown menu
        vm.statuses = ['All', 'Opened', 'Scheduled', 'Draft'];

        //functions
        vm.reloadQuizzes = reloadQuizzes;
        vm.sortBy = sortBy;
        vm.showOrderArrow = showOrderArrow;
        vm.changedValue = changedValue;

        activate();

        function activate() {
            reloadQuizzes();
        };

        function reloadQuizzes() {
            var quizzesPromise = quizzesService.getQuizzesPage(vm.pagingInfo);
            quizzesPromise.then(function (data) {
                vm.quizzes = data.Quizzes;
                vm.pagingInfo.quizzesTotal = data.QuizzesTotal;
                vm.isLoading = false;
            }, errorCallBack);
        };

        function sortBy(predicate) {
            vm.pagingInfo.reverse = (vm.pagingInfo.predicate === predicate) ? !vm.pagingInfo.reverse : false;
            vm.pagingInfo.predicate = predicate;
            reloadQuizzes();
        };

        function showOrderArrow(predicate) {
            if (vm.pagingInfo.predicate === predicate) {
                return vm.pagingInfo.reverse ? '▲' : '▼';
            }
            return '';
        };        

        function changedValue(item) {
            vm.pagingInfo.selectedStatus = item;
            reloadQuizzes();
        };        

        function errorCallBack(error) {
            console.log('An unexpected error has occured: ' + error.statusText);
        };
    }
})(angular);