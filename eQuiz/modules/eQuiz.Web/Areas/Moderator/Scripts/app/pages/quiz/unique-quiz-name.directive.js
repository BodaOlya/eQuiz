(function () {
    angular.module('quizModule').
    directive('uniqueQuizName', uniqueQuizName);

    uniqueQuizName.$inject = ['quizService'];

    function uniqueQuizName(quizService) {
    	return {
    		restrict: 'A',
    		require: '^form',
    		link: function (scope, element, attributes, formControl) {
    		    var inputElement = element[0].querySelector("[name]");
    			var inputNgElement = angular.element(inputElement);
    			var inputName = inputNgElement.attr('name');
    			var messagesBlock = inputNgElement.next();

    			inputNgElement.bind('blur', function () {
    			    quizService.isNameUnique(inputElement.value).then(function (data) {
    			            formControl.name.$setValidity('nonUniqueName', data.data);
    			            element.toggleClass('has-error', formControl[inputName].$invalid);
    			            messagesBlock.toggleClass('hide', formControl[inputName].$valid);
    			    })
    			})
    		}
    	}
    }
})();