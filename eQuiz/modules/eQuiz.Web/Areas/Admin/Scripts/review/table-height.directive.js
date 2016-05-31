(function (angular) {
    angular.module('equizModule').directive('autoHeight', function () {
        return {
            restrict: 'A',
            link: function (scope, elem, attrs) {
                elem.css('min-height', (attrs.autoHeight * 10) + 200 + 'px');
            }
        };
    });
})(angular);