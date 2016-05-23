(function (angular) {
    angular.module('equizModule').controller('StudentController', StudentController);

    StudentController.$inject = ['$scope', '$filter', 'studentDataService', '$routeParams'/*, 'studentInfo', 'studentQuizzes', 'studentComments'*/];

    function StudentController($scope, $filter, studentDataService, $routeParams/*, studentInfo, studentQuizzes, studentComments*/) {
        var vm = this;

        vm.studentInfo = {};//studentInfo;
        vm.studentQuizzes = [];//studentQuizzes;
        vm.studentComments = [];//studentComments;
        activate1();
        activate2();
        activate3();
        vm.studentQuizzesHeaders = [
        {
            name: 'Name',
            field: 'name',
            predicateIndex: 0
        }, {
            name: 'State',
            field: 'state',
            predicateIndex: 1
        }, {
            name: 'Questions',
            field: 'questions',
            predicateIndex: 2
        }, {
            name: 'Verification Type',
            field: 'verificationType',
            predicateIndex: 3
        }, {
            name: 'Other details',
            field: 'otherDetails',
            predicateIndex: 4
        }
        ];
        vm.myPredicate = null;
        vm.newComment = {}; // Represents a new comment
        vm.currentTab = $routeParams.tabId;
        vm.link = "";
        vm.newCommentFrame = false; // Indicates whether new comment tab is shown
        vm.modelChanged = false; // Indicates whether data in the model was changed
        vm.activated = false;

        var orderBy = $filter('orderBy');
        vm.resultsCount = [10, 25, 50, 100];
        vm.tablePage = 0;
        vm.resultsPerPage = 10;

 

        function activate1() {
            return studentDataService.getStudentInfo($routeParams.studentId).then(function (response) {
                vm.studentInfo = response.data;
                return vm.studentInfo;
            });}
        function activate2() {
            return studentDataService.getStudentQuizzes($routeParams.studentId).then(function (response) {
                vm.studentQuizzes = response.data;
                return vm.studentQuizzes;
            });}
        function activate3() {
            return studentDataService.getStudentComments($routeParams.studentId).then(function (response) {
                vm.studentComments = response.data;
                return vm.studentComments;
            });}
            //while (vm.studentInfo == undefined || vm.studentQuizzes == undefined || vm.studentComments == undefined)
            //{
            //    ;;
            //}
            generatePredicate();

        vm.studentQuizzes = sortByDate(vm.studentQuizzes);
        vm.studentComments = sortByDate(vm.studentComments);
        vm.numberOfPages = function () {
            return Math.ceil(vm.studentQuizzes.length / vm.resultsPerPage);
        };

        vm.getNumber = function (num) {
            return new Array(num);
        };

        vm.goToPage = function (page) {
            vm.tablePage = page;
        };

        function generatePredicate() {
            vm.myPredicate = [null, null, null, null, null];
        };

        function clearPredicatesExcept(index) {
            var temp = vm.myPredicate[index];
            generatePredicate();
            vm.myPredicate[index] = temp;
        };

        vm.refreshPredicate = function (index) {
            if (vm.myPredicate[index] === null) {
                var item = null;
                switch (index) {
                    case 0:
                        item = '+name';
                        break;
                    case 1:
                        item = '+state';
                        break;
                    case 2:
                        item = '+questions';
                        break;
                    case 3:
                        item = '+verificationType';
                        break;
                    case 4:
                        item = '+otherDetails';
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
        };

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
        };

        vm.order = function (predicate, reverse) {
            vm.studentQuizzes = orderBy(vm.studentQuizzes, predicate, reverse);
            vm.predicate = predicate;
        };

        vm.getPassedQuizzes = function () {
            var passed = 0;
            var inVerification = 0;
            var notPassed = 0;

            vm.studentQuizzes.forEach(function (currVal, index) {
                if (currVal.state == 'Passed') {
                    passed++;
                }
                else if (currVal.state == 'In Verification') {
                    inVerification++;
                }
                else {
                    notPassed++;
                }
            });

            return passed + '/' + inVerification + '/' + notPassed;
        };

        vm.saveProfile = function () {
                studentDataService.saveProfileInfo(vm.studentInfo.id, vm.studentInfo.firstName, vm.studentInfo.lastName, vm.studentInfo.phone)
                //Here has popUp be called
            vm.modelChanged = false;
        };

        vm.cancelProfile = function () {
            activate();
            vm.modelChanged = false;
        }; // Cancel unsaved changes in the profile

        vm.toggleNewCommentFrame = function () {
            vm.newCommentFrame = !vm.newCommentFrame;
            vm.newComment = {}
        };

        vm.addComment = function () {
            vm.newComment.date = (new Date().toLocaleDateString());
            vm.studentComments.push(vm.newComment);
            vm.modelChanged = true;
            vm.toggleNewCommentFrame();
            vm.studentComments = sortByDate(vm.studentComments);
        };

        vm.validationCheck = function () {
            return $scope.profileInfo.firstName.$valid && $scope.profileInfo.lastName.$valid && $scope.profileInfo.phone.$valid && vm.modelChanged;
        }; // Checks

        vm.setQuizLink = function (quizId) {
            vm.link = '/Admin/Default/Index/Quiz/Review/' + quizId;
        };

        vm.setLink = function () {
            vm.link = '/Admin/Default/Index/Student/' + $routeParams.studentId + '/' + vm.currentTab;
        };

        function sortByDate(array) {
            return $filter('orderBy')(array, function (item) {
                var parts = item.date.split('.');
                return new Date(parseInt(parts[2]),
                        parseInt(parts[1]),
                        parseInt(parts[0]));
            }, true);
        }; // Sort specified array by date in descending order
    };
})(angular);