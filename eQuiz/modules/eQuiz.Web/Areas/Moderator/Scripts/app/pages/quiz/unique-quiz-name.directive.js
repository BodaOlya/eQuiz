(function () {
    angular.module('equizModule').
    directive('uniqueQuizName', uniqueQuizName);

    uniqueQuizName.$inject = ['quizService'];

    function uniqueQuizName(quizService) {
    	return {
    		restrict: 'A',
    		require: '^form',
    		link: function (scope, element, attributes, formControl) {
    		    var inputElement = element[0].querySelector("[name]");
    		    var inputNgElement = angular.element(inputElement);
    		    var inputIdElement = inputNgElement.next();
    			var inputName = inputNgElement.attr('name');
    			var messagesBlock = inputIdElement.next();

    			function callback(data) {
    			    formControl[inputName].$setValidity('nonUniqueName', data.data);
    			    element.toggleClass('has-error', formControl[inputName].$invalid);
    			    messagesBlock.toggleClass('hide', formControl[inputName].$valid);
    			}

    			inputNgElement.bind('blur', function () {
    			    var s = inputIdElement.val();
    			    if (inputIdElement.val()) {
    			        quizService.isNameUnique(inputElement.value, inputIdElement.val()).then(function (data) {
    			            callback(data);
    			        });
    			    }
    			    else {
    			        quizService.isNameUnique(inputElement.value).then(function (data) {
    			            callback(data);
    			        });
    			    }
    			})
    		}
    	}
    }
})();