(function () {
    angular.module('quizModule').
    directive('showError', function () {
      return {
          restrict: 'A',
          require: '^form',
          link: function (scope, el, attrs, formCtrl) {
              var inputElement = el[0].querySelector("[name]");
              var inputNgElement = angular.element(inputElement);
              var inputName = inputNgElement.attr('name');
              var messagesBlock = inputNgElement.next();

              inputNgElement.bind('blur', function () {
                  el.toggleClass('has-error', formCtrl[inputName].$invalid);
                  messagesBlock.toggleClass('hide', formCtrl[inputName].$valid);
              })
          }
      }
  });
})();