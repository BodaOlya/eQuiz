(function (angular) {
    angular
        .module('equizModule')
        .filter('questionStatusFilter', QuestionStatusFilter);

    function QuestionStatusFilter() {
        return function (questions, selectedStatuses) {
            if (!angular.isUndefined(questions) && !angular.isUndefined(selectedStatuses) && selectedStatuses.length > 0) {
                var tempQuestions = [];
                angular.forEach(selectedStatuses, function (id) {
                    angular.forEach(questions, function (question) {
                        if (question.Status === "Not Passed" && id === 2) {
                            tempQuestions.push(question);
                        } else if (question.Status === "In Verification" && id === 0) {
                            tempQuestions.push(question);
                        } else if (question.Status === "Passed" && id === 1) {
                            tempQuestions.push(question);
                        }
                    });
                });
                return tempQuestions;
            } else {
                return questions;
            }
        };
    };
})(angular);