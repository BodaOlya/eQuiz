(function (angular) {
    angular
        .module('equizModule')
        .controller('QuizDetailsController', quizDetailsController);

    quizDetailsController.$inject = ['$scope', '$filter', 'quizDetailsDataService'];

    function quizDetailsController($scope, $filter, quizDetailsDataService) {
        var vm = this;

        var orderBy = $filter('orderBy');

        vm.search = ''; // Represents search field on the form
        vm.myPredicate = null;
        vm.resultsPerPage = 10;
        vm.resultsCount = [10, 25, 50, 100];
        vm.tablePage = 0;
        vm.linkToProfile = "Index/Student?Id=";
        vm.linkToQuizRewiew = "Index/Student?Id=";

        vm.headers = [
            {
                name: 'Student',
                field: 'student',
                predicateIndex: 0
            }, {
                name: 'Score',
                field: 'score',
                predicateIndex: 1
            }, {
                name: 'Quiz Status',
                field: 'quizStatus',
                predicateIndex: 2
            }
        ];
        vm.students = [];
        vm.quizInfo = {};

        function activate() {
            vm.students = quizDetailsDataService.getQuizPasses(1);
            vm.quizInfo = quizDetailsDataService.getQuiz(1);
            generatePredicate();
        };

        activate();

        function generatePredicate() {
            vm.myPredicate = [null, null, null];
        }; // Generates empty predicates that are used for ordering

        function clearPredicatesExcept(index) {
            var temp = vm.myPredicate[index];
            generatePredicate();
            vm.myPredicate[index] = temp;
        }; // Clears all predicates except the one with a specified index

        vm.refreshPredicate = function (index) {
            if (vm.myPredicate[index] === null) {
                var item = null;
                switch (index) {
                    case 0:
                        item = '+student';
                        break;
                    case 1:
                        item = '+studentScore';
                        break;
                    case 2:
                        item = '+quizStatus';
                        break;
                }
                vm.myPredicate[index] = item;
            }
            else if (vm.myPredicate[index][0] === '+') {
                vm.myPredicate[index] = '-' + vm.myPredicate[index].slice(1);
            }
            else if (vm.myPredicate[index][0] === '-') {
                vm.myPredicate[index] = null;
            }
            clearPredicatesExcept(index);
        }; // Changes the value of the predicate with specified index and clears all others 

        vm.direction = function (index) {
            if (vm.myPredicate) {
                if (vm.myPredicate[index] === null) {
                    return null;
                };
                if (vm.myPredicate[index][0] === '+') {
                    return true;
                };
                return false;
            };
            return null;
        }; // Gets the order direction of the predicate with specified index

        vm.order = function (predicate, reverse) {
            vm.students = orderBy(vm.students, predicate, reverse);
            vm.predicate = predicate;
        }; // Orders the data based on the specified predicate

        vm.setLink = function (studentId) {
            vm.linkToProfile += studentId + "#Profile";
        };

        vm.setLinkToQuiz = function (studentId) {
            vm.linkToQuizRewiew += studentId + "#Quizzes";
        };

        vm.numberOfPages = function () {
            return Math.ceil(vm.studentsFiltered.length / vm.resultsPerPage);
        };

        vm.getNumber = function (num) {
            return new Array(num);
        };

        vm.goToPage = function (page) {
            vm.tablePage = page;
        };
        vm.paginationChanged = function () {
            vm.tablePage = 0;
        }
    };
})(angular);