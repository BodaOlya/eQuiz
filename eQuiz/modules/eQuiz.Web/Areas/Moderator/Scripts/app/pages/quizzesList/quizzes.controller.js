(function (angular) {

    angular
        .module('quizzesModule')
        .controller('QuizzesController', QuizzesController);        

    QuizzesController.$inject = ['$scope', 'quizzesService'];

    function QuizzesController($scope, quizzesService) {
        var vm = this;

        //fields
        vm.quizzes = [];        

        //paging with sorting 
        vm.pagingInfo = {
            currentPage: 1,
            quizzesPerPage: 5,
            predicate: 'Name',
            reverse: false,
            quizzesTotal: 0
        };

        //functions
        vm.reloadQuizzes = reloadQuizzes;
        vm.sortBy = sortBy;
        vm.showOrderArrow = showOrderArrow;
        vm.getNumber = getNumber;

        activate();

        function activate() {
            reloadQuizzes();
        };

        function reloadQuizzes() {
            var quizzesPromise = quizzesService.getQuizzesPage(vm.pagingInfo);
            quizzesPromise.then(function (data) {
                vm.quizzes = data.Quizzes;
                vm.pagingInfo.quizzesTotal = data.QuizzesTotal;
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

        function getNumber(num) {
            //TODO
            num = Math.ceil(vm.pagingInfo.quizzesTotal / num);
            return new Array(num);
        };

        function errorCallBack(error) {
            console.log('An unexpected error has occured: ' + error.statusText);
        };
    }
})(angular);