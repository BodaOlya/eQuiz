(function(angular) {
angular.module('equizModule').controller('ReviewController', ReviewController);

    ReviewController.$inject = ['$scope', '$filter', 'reviewDataService', 'sharedProperties']; 

    function ReviewController($scope, $filter, reviewDataService, sharedProperties) {
        var vm = this;

        var orderBy = $filter('orderBy');

        vm.search = ''; // Represents search field on the form
        vm.myPredicate = null;
        vm.tablePage = 0; // Current table page
        vm.resultsPerPage = 10;
        vm.resultsCount = [10, 25, 50, 100]; // Possible numbers of results per page
        vm.selectedGroup = []; 

        vm.headers = [
    {
        name:'Student',
        field: 'student',
        predicateIndex: 1
    },{
        name: 'User group', 
        field: 'userGroup',
        predicateIndex: 0
    },{
        name:'Quizzes', 
        field: 'quizzes',
        predicateIndex: 2
    }
  ]; 
        vm.students = [];

        function activate() {
        //var studentsPromise = reviewDataService.getStudents();
        //studentsPromise.then(function (response) {
        //    $scope.$applyAsync(function () {
        //        $scope.students.push.apply($scope.students, response.data);
        //    });
        //});
        vm.students = reviewDataService.getStudents() // Get the mock students data
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
                  item = '+userGroup';
                  break;
              case 1:
                  item = '+student';
                  break;
              case 2:
                  item = '+quizzes';
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

    vm.numberOfPages = function () {
        return Math.ceil(vm.searchFiltered.length / vm.resultsPerPage);
    };

    vm.getNumber = function (num) {
      return new Array(num);
  };

    vm.goToPage = function (page) {
        vm.tablePage = page;
    };

    vm.groupList = GetUniquePropertyValues(vm.students, 'userGroup'); // Property user group needs to be changed manualy    

    $scope.setSelectedGroup = function () { // DONT PUT THIS FUNCTION INTO VM! let it be in scope (because of 'this' in function)
        var id = this.group;
        if (vm.selectedGroup.toString().indexOf(id.toString()) > -1) {
            for (var i = 0; i < vm.selectedGroup.length; i++) {
                if (vm.selectedGroup[i] === id) {
                    vm.selectedGroup.splice(i, 1);
                }
            }
        } else {
            vm.selectedGroup.push(id);
        }
        return false;
    };

    vm.isChecked = function (group) {
        if (vm.selectedGroup.toString().indexOf(group.toString()) > -1) {
            return 'icon-ok pull-right';
        }
        return false;
    };

    vm.checkAll = function () {
        vm.selectedGroup = vm.groupList;
    };

    vm.unCheckAll = function () {
        vm.selectedGroup = [];
    };

    function GetUniquePropertyValues(arrayToCheck, propertyName) {
        var flags = [];
        var output = [];
        for (var i = 0; i < arrayToCheck.length; i++) {
            if (flags[arrayToCheck[i][propertyName]]) {
                continue;
            }

            flags[arrayToCheck[i][propertyName]] = true;
            output.push(arrayToCheck[i][propertyName]);
        }

        return output;
    }

    vm.setStudent = function (student) {
        sharedProperties.selectedStudent = student;
    }; 
};

})(angular);