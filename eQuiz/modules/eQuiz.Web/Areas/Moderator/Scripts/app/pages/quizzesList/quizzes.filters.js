(function (angular) {
    var quizzesFilters = angular
        .module('quizzesFilters', [])
        .filter('ctime', function () {
            return function (jsonDate) {
                if (jsonDate != null) {
                    return new Date(parseInt(jsonDate.substr(6)));
                }
                return "";
            };
        });
})(angular);