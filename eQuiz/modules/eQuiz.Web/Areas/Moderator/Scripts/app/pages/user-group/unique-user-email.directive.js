(function () {
    angular
    .module('equizModule')
    .directive('uniqueUserEmail', uniqueUserEmail);
    //todo
    uniqueUserEmail.$inject = ['userGroupService'];
    function uniqueUserEmail(userGroupService) {
        return {
            restrict: 'A',
            require: '^form',
            scope: {
                user: '=',
                users: '='
            },
            link: function (scope, element, attributes, formControl) {
                var inputElement = element[2].querySelector("[name]");
                var inputNgElement = angular.element(inputElement);
                var inputName = inputNgElement.attr('name');
                var messagesBlock = inputNgElement.next();
                console.log("IN DIRECTIVE ", scope.users);
                alert("gfghfh");
                function callback(data) {
                    formControl.name.$setValidity('repeatedMessage', data);
                    element.toggleClass('has-error', formControl[inputName].$invalid);
                    messagesBlock.toggleClass('hide', formControl[inputName].$valid);
                }

                inputNgElement.bind('blur', function () {
                    var isValid = true;
                    userGroupService.isUsersValid(user).then(function (data) {
                        isValid = data.data;
                    });
                    for (var i = 0; i < users.length; i++) {
                        if (users[i].Email === user.Email) {
                            isValid = false;
                        }
                        callback(isValid)
                    }

                })

            }
        }
    }
})