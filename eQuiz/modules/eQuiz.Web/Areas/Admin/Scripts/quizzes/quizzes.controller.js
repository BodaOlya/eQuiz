(function (angular) {
    angular.module('equizModule').controller('QuizzesController', QuizzesController);

    QuizzesController.$inject = ['$scope', 'quizzesDataService', '$filter', 'quizzesList'];

    function QuizzesController($scope, quizzesDataService, $filter, quizzesList) {
        var vm = this;
        vm.myPredicate = null;
        vm.resultsPerPage = 10;
        vm.resultsCount = [10, 25, 50, 100];
        vm.tablePage = 0;
        vm.search = '';
        var orderBy = $filter('orderBy');

        vm.headers = [{
            name: 'Quiz',
            field: 'quiz_name',
            predicateIndex: 0
        }, {
            name: 'Group',
            field: 'group_name',
            predicateIndex: 1
        }, {
            name: 'Number of questions',
            field: 'questions_amount',
            predicateIndex: 2
        }, {
            name: 'Number of students',
            field: 'students_amount',
            predicateIndex: 3
        }, {
            name: 'Verification type',
            field: 'verification_type ',
            predicateIndex: 4
        }];

        function active() {
            vm.quizzes = quizzesList;
            vm.quizzes.forEach(function (currVal, index, array) {
                currVal.quiz_name = currVal.quiz_name.toString();
                currVal.group_name = currVal.group_name.toString();
                currVal.questions_amount = currVal.questions_amount.toString();
            });
            generatePredicate();            
        }

        active();


        function generatePredicate() {
            vm.myPredicate = [null, null, null, null, null];
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
                        item = '+quiz_name';
                        break;
                    case 1:
                        item = '+group_name';
                        break;
                    case 2:
                        item = '+questions_amount';
                        break;
                    case 3:
                        item = '+students_amount';
                        break;
                    case 4:
                        item = '+verification_type';
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
            vm.quizzes = orderBy(vm.quizzes, predicate, reverse);
            vm.predicate = predicate;
        }; // Orders the data based on the specified predicate

        vm.numberOfPages = function () {
            return Math.ceil(vm.quizzesFiltered.length / vm.resultsPerPage);
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
    }
})(angular);