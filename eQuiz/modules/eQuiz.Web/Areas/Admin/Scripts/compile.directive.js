(function (angular) {
    angular.module('equizModule').directive('compile', Compile);

    Compile.$inject = ['$compile'];

    function Compile($compile) {
        return function ($scope, element, attrs) {
            $scope.$watch(
                function ($scope) {
                    return $scope.$eval(attrs.compile);
                },
                function (value) {
                    element.html(value);
                    $compile(element.contents())($scope);
                }
            );
        };
    };
})(angular);