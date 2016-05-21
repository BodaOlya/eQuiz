(function () {
    angular.module('equizModule').
    directive('uniqueQuizName', uniqueQuizName);

    uniqueQuizName.$inject = ['quizService', '$location'];

    function uniqueQuizName(quizService, $location) {
    	return {
    		restrict: 'A',
    		require: '^form',
    		link: function (scope, element, attributes, formControl) {
    		    var inputElement = element[0].querySelector("[name]");
    			var inputNgElement = angular.element(inputElement);
    			var inputName = inputNgElement.attr('name');
    			var messagesBlock = inputNgElement.next();

    			function callback(data) {
    			    formControl.name.$setValidity('nonUniqueName', data.data);
    			    element.toggleClass('has-error', formControl[inputName].$invalid);
    			    messagesBlock.toggleClass('hide', formControl[inputName].$valid);
    			}

    			inputNgElement.bind('blur', function () {
    			    if ($location.search().id) {
    			        quizService.isNameUnique(inputElement.value, $location.search().id).then(function (data) {
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