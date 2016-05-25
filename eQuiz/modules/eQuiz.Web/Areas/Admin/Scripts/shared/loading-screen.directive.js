(function(angular) {
    angular.module('equizModule')
    .directive('loading', ['$http', '$timeout', function ($http, $timeout) {
        return {
            restrict: 'A',
            link: function (scope, elm, attrs) {
                var timer = false;

                scope.isLoading = function () {
                    return $http.pendingRequests.length > 0;
                };

                scope.$watch(scope.isLoading, function (v) {
                    if (timer) {
                        $timeout.cancel(timer)
                    }

                    timer = $timeout(function () {
                        if (v) {
                            elm.css('display', 'block');
                        } else {
                            elm.css('display', 'none');
                        }
                    }, 1000)
                });
            }
        };

    }]);
})(angular);