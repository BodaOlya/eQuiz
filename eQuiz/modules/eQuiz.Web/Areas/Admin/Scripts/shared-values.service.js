(function (angular) {
    angular.module('equizModule').service('sharedProperties', function () {

        var hashtable = {};

        return {
            setValue: function (key, value) {
                hashtable[key] = value;
            },
            getValue: function (key) {
                return hashtable[key];
            }
        }
    });
})(angular);