(function () {
    angular.module('quizModule').
    directive('showTimeSpanError', function () {
        return {
            restrict: 'A',
            require: '^form',
            link: function (scope, element, attributes, formControl) {
                var hours = angular.element(element[0].querySelector("[name]"));
                var minutes = hours.next().next();
                var hoursBlock = minutes.next().next();
                var minutesBlock = hoursBlock.next();

                hours.bind('blur', function () {
                    element.toggleClass('has-error', formControl['durationHours'].$invalid || formControl['durationMinutes'].$invalid);
                    hoursBlock.toggleClass('hide', formControl['durationHours'].$valid);
                });

                minutes.bind('blur', function () {
                    element.toggleClass('has-error', formControl['durationHours'].$invalid || formControl['durationMinutes'].$invalid);
                    minutesBlock.toggleClass('hide', formControl['durationMinutes'].$valid);
                });
            }
        }
    });
})();