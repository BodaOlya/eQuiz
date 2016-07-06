(function (angular) {
    angular.module('equizModule')
    .directive('popUpWindow', function () {
        return {
            restrict: 'EA',
            scope: false,
            templateUrl: '/Areas/Admin/Scripts/shared/pop-up.html',
            controller: function ($scope) {
                $scope.showNotification = false;
                $scope.showWarning = false;
                $scope.showErrorNotification = false;
                $scope.messageText = '';

                $scope.showNotifyPopUp = function (text) {
                    $scope.messageText = text;
                    $scope.closePopUp();
                    $scope.showNotification = true;
                }

                $scope.showErrorPopUp = function (text) {
                    $scope.messageText = text;
                    $scope.closePopUp();
                    $scope.showErrorNotification = true;
                }

                $scope.showWarningPopUp = function (text, ifOk, ifCancel) {
                    $scope.messageText = text;
                    $scope.closePopUp();
                    $scope.showWarning = true;
                    $scope.warningWindowOK = ifOk;
                    $scope.warningWindowCancel = ifCancel;

                }

                $scope.closePopUp = function () {
                    $scope.showErrorNotification = false;
                    $scope.showNotification = false;
                    $scope.showWarning = false;
                }
            }
        }
    });

})(angular);