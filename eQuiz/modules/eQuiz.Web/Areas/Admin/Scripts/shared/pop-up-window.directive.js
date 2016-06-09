(function (angular) {
    angular.module('equizModule')
    .directive('popUpWindow', function () {
        return {
            restrict: 'EA',
            scope: false,
            template: '<div class="notifyPopUpMessage" ng-show="showNotification"><div class="closePopUp link" ng-click="closePopUp()">X</div><div class="NotifyPopUpText">{{messageText}}</div><div class="confirmPopUp"></div></div><div class="notifyPopUpMessage" ng-show="showWarning"><div class="closePopUp link" ng-click="closePopUp()">X</div><div class="ConfirmPopUpText">{{messageText}}</div><div class="confirmPopUp"><button class="buttonPopUp link" ng-click="closePopUp(); warningWindowOK()">OK</button><button class="buttonPopUp link" ng-click="closePopUp(); warningWindowCancel()">Cancel</button></div></div>',
            controller: function ($scope) {
                $scope.showNotification = false;
                $scope.showWarning = false;
                $scope.messageText = '';

                $scope.showNotifyPopUp = function(text) {
                    $scope.messageText = text;
                    $scope.showNotification = true;
                }

                $scope.showWarningPopUp = function(text) {
                    $scope.messageText = text;
                    $scope.showWarning = true;
                }

                $scope.closePopUp = function() {
                    $scope.showNotification = false;
                    $scope.showWarning = false;
                }
            }
        }
    });

})(angular);