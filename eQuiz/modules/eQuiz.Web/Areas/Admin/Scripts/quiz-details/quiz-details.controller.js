﻿(function (angular) {
    angular
        .module('equizModule')
        .controller('QuizDetailsController', quizDetailsController);

    quizDetailsController.$inject = ['$scope', '$filter', '$http', '$location', '$timeout', 'quizDetailsDataService', 'quizInfo', 'quizStudents'];

    function quizDetailsController($scope, $filter, $http, $location, $timeout, quizDetailsDataService, quizInfo, quizStudents) {
        var vm = this;

        var orderBy = $filter('orderBy');

        vm.search = ''; // Represents search field on the form
        vm.myPredicate = null;
        vm.resultsPerPage = 10;
        vm.resultsCount = [10, 25, 50, 100];
        vm.tablePage = 0;
        vm.contentsToExport = []; // Contains data for exporting into excel file
        vm.excelPath = 'D:\\'; // Default path for excel file
        $scope.showNotification = false;
        $scope.showWarning = false;

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
            });


            vm.quizInfo.forEach(function (currVal, index, array) {
                currVal.quizId = currVal.quizId.toString();
                currVal.quizName = currVal.quizName.toString();
                currVal.groupName = currVal.groupName.toString();
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
            vm.linkToProfile = "Index/Student?Id=" + studentId + "#Profile";
        };

        vm.setLinkToQuiz = function (studentId, quizId) {
            vm.linkToQuizRewiew = "Index/Quiz?Student=" + studentId + "&Quiz=" + quizId;
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
        }; // Says for method below what it should do

        vm.addOrRemoveFromExport = function (student) {
            if (vm.singleExportToDo(student) === 'Cancel') {
                vm.contentsToExport.splice(vm.contentsToExport.indexOf(student), 1);
            } else {
                vm.contentsToExport.push(student);
            };

        }; // Adds/removes student to/from the object vm.contentsToExport,
        // which contains all information, that will be putted into excel file

        vm.multipleExportToDo = function (students) {
            if (students) {
                for (var i = 0; i < students.length; i++) {
                    if (vm.contentsToExport.indexOf(students[i]) !== -1) {
                        return 'Cancel All';
                    }
                };
            };
            return 'Export All';
        }; // Says for method below what it should do

        vm.addOrRemoveFromExportAll = function (students) {
            if (vm.multipleExportToDo(students) === 'Cancel All') {
                var length = vm.contentsToExport.length;
                for (var i = 0; i < length; i++) {
                    for (var j = 0; j < students.length; j++) {
                        if (vm.contentsToExport[i] === students[j]) {
                            vm.addOrRemoveFromExport(students[j]);
                            i--;
                            break;
                        };
                    };
                };
            } else {
                for (var i = 0; i < students.length; i++) {
                    vm.addOrRemoveFromExport(students[i]);
                };
            };
        };// Adds/removes students to/from the object vm.contentsToExport,
        // which contains all information, that will be putted into excel file

        vm.CreateExcel = function () {
            var dataForExport = [];
            vm.contentsToExport.forEach(function (currVal, index, array) {
                dataForExport.push({
                    'student': currVal.student,
                    'email': currVal.email,
                    'score': currVal.studentScore
                });
            });

            var promise = $http({
                method: 'GET',
                url: '/Admin/QuizDetails/ExportToExcel',
                params: {
                    nameOfFile: vm.quizInfo[0].quizName + " by " + vm.quizInfo[0].groupName,
                    pathToFile: vm.excelPath,
                    currentUrl: $location.$$absUrl,
                    data: dataForExport
                },
                headers: { 'Content-Type': 'application/json' }
            }).then(function successCallback(response) {
                if (response.data.indexOf('File was successfully saved to:') > -1) {
                    $scope.showNotifyPopUp(response.data);
                    $timeout($scope.closePopUp, 5000);
                } else {
                    $scope.showErrorPopUp(response.data);
                    $timeout($scope.closePopUp, 30000);
                }
            }, function errorCallback(response) {
                document.write(response.data);
            });
        }; // Method that sends data to the server

        vm.cancelExport = function () {
            var ifOK = function () {
                vm.exportShow = !vm.exportShow;
                vm.contentsToExport = [];
            };
            $scope.showWarningPopUp("Do you realy want cancel exporting?", ifOK, undefined);
        };
    };
})(angular);