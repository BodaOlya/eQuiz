(function (angular) {
    angular
        .module('equizModule')
        .controller('QuizDetailsController', quizDetailsController);

    quizDetailsController.$inject = ['$scope', '$filter', 'quizDetailsDataService', 'quizInfo', 'quizStudents'];

    function quizDetailsController($scope, $filter, quizDetailsDataService, quizInfo, quizStudents) {
        var vm = this;

        var orderBy = $filter('orderBy');

        vm.search = ''; // Represents search field on the form
        vm.myPredicate = null;
        vm.resultsPerPage = 10;
        vm.resultsCount = [10, 25, 50, 100];
        vm.tablePage = 0;
        vm.linkToProfile = "Index/Student?Id=";
        vm.linkToQuizRewiew = "Index/Student?Id=";        
        vm.contentsToExport = []; // Contains data for exporting into excel file
        vm.excelPath = 'D:/name.xls'; // Default path and name for excel file

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
            vm.students = quizStudents;
            vm.quizInfo = quizInfo;//quizDetailsDataService.getQuiz(1);

            vm.students.forEach(function (currVal, index, array) {
                currVal.student = currVal.student.toString();
                currVal.email = currVal.email.toString();
                currVal.studentScore = currVal.studentScore.toString();
                currVal.quizStatus = currVal.quizStatus.toString();
                currVal.questionDetails = currVal.questionDetails.toString();
            });
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
        };
        vm.singleExportToDo = function (student) {
            if (vm.contentsToExport.indexOf(student) === -1) {
                return 'Export';
            };
            return 'Cancel';
        }; // Says for method below what it must do

        vm.addOrRemoveFromExport = function (student) {
            if (vm.singleExportToDo(student) === 'Cancel') {
                vm.contentsToExport.splice(vm.contentsToExport.indexOf(student), 1);
            } else {
                vm.contentsToExport.push(student);
            };

        }; // Adds/removes student to/from the object vm.contentsToExport,
        // which contains all information, that will be putted into excel file

        vm.multipleExportToDo = function () {
            if (!vm.contentsToExport[0]) {
                return 'Export All';
            } else {
                return 'Cncel All';
            };
        }; // Says for method below what it must do

        vm.addOrRemoveFromExportAll = function (students) {
            if (!vm.contentsToExport[0]) {
                vm.contentsToExport = students;
            } else {
                vm.contentsToExport = [];
            };
        };// Adds/removes students to/from the object vm.contentsToExport,
        // which contains all information, that will be putted into excel file

        vm.writeTableToExcel = function () {
            console.log('Saving . . .');
        }; // Method that
    };
})(angular);