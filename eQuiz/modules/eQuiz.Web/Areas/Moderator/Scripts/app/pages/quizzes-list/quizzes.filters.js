(function (angular) {
    angular
        .module('equizModule')
        .filter('ctime', ctime);

    function ctime() {
        return function (jsonDate) {
            if (jsonDate != null) {
                return new Date(parseInt(jsonDate.substr(6)));
            }
            return "No date";
        };
    };
})(angular);