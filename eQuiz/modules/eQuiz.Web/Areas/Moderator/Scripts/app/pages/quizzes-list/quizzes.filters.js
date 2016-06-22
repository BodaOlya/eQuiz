(function (angular) {
    angular
        .module('equizModule')
        .filter('ctime', ctime)
        .filter('startFrom', startFrom);

    function ctime() {
        return function (jsonDate) {
            if (jsonDate != null) {
                return new Date(parseInt(jsonDate.substr(6)));
            }
            return "No date";
        };
    };
    function startFrom() {
        return function (data, start) {
            if (data === undefined) {
                return [];
            }
            return data.slice(start);
        };
    };
})(angular);