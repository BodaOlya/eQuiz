(function(angular) {
    angular.module('equizModule').filter('startFrom', function () {
        return function (input, start) {
            if (!input || !input.length) { return; }
            start = +start;
            return input.slice(start);
        }
    });
})(angular);