(function (angular) {
    angular
    .module('equizModule')
    .directive('uniqueUserEmail', uniqueUserEmail);
    //todo
    uniqueUserEmail.$inject = ['userGroupService'];
    function uniqueUserEmail(userGroupService) {
        return {
            scope: {
                user: '=',
                users: '='
            },
            restrict: 'A',
            require: '^form',

            link: function (scope, element, attributes, formControl) {
                var inputElement = element[0].querySelector('[name ="Email"]');
                var inputNgElement = angular.element(inputElement);
                var inputName = inputNgElement.attr('name');
                var messagesBlock = inputNgElement.next();

                function callback(data) {
                    formControl.Email.$setValidity('nonUniqueEmail', data);
                    element.toggleClass('has-error', formControl[inputName].$invalid);
                    messagesBlock.toggleClass('hide', formControl[inputName].$valid);
                }

                function checkValidAfterAddedUser() {
                    var amountSame = 0;
                    for (var i = 0; i < scope.users.length; i++) {
                        if (scope.users[i].Email === scope.user.Email) {
                            amountSame++;
                        }
                    }

                    var isValid = true;
                    userGroupService.isUserValid(scope.user.FirstName, scope.user.LastName, scope.user.Email).then(function (data) {
                        isValid = data.data;

                        if (amountSame > 1 || !isValid) {
                            callback(false);
                        } else {

                            callback(true);
                        }
                    });
                }
                //  checkValidAfterAddedUser();

                inputNgElement.bind('blur', function () {
                    checkValidAfterAddedUser();
                });
            }
        }
    }
})(angular);