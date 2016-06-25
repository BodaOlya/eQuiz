(function (angular) {
    angular
        .module('equizModule')
        .filter('timeFromMinutes', customTime);

    function customTime() {
        return function (inputMinutes) {
            if (inputMinutes != null || inputMinutes != undefined) {
                var hours = Math.trunc(inputMinutes / 60);
                var minutes = inputMinutes % 60;

                return ((hours > 0) ? hours + "h " : "") + ((minutes > 0) ? minutes + "min" : "");
            }
        };
    };
})(angular);