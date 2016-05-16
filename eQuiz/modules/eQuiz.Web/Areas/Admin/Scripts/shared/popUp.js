(function (angular) {
    angular.module('equizModule')

    .directive('popUpMsg', function () {
        return {
            restrict: 'E',
            scope: false,
            template: '<div id="popUpMsg-bg" ng-show="showPopUpMsg"><div id="popUpMsg"><div class="close" ng-click="closePopUp()">x</div><div class="content">{{popUpMsgContent}}</div><button ng-click="closePopUp()">Ok</button></div></div>',
            controller: function ($scope) {
                $scope.closePopUp = function () {
                    $scope.showPopUpMsg = false;
                }
            }
        }
    })

    .controller('popUp', function ($scope) {
        $scope.showPopUpMsg = false;
        $scope.openPopUp = function (text) {
            $scope.showPopUpMsg = true;
            $scope.popUpMsgContent = text;
        }
    })

})(angular);