(function (angular) {

    angular
        .module("equizModule")
        .factory("quizzesDataService", quizzesDataService);

    function quizzesDataService() {

        var service = {            
            getQuizzes: getQuizzesAjax,
        };

        return service;

        function getQuizzesAjax(quizeId) {
            var promise = [{
                    quiz_name: 'Theory',
                    group_name: '.Net 2015',
                    questions_amount: 17,
                    students_amount: 15,
                    verification_type: 'Combined(A:13 M:4)'
                }, {
                    quiz_name: 'Logic',
                    group_name: 'Java',
                    questions_amount: 14,
                    students_amount: 7,
                    verification_type: 'Auto'
                }, {
                    quiz_name: 'SQL',
                    group_name: '.Net 2015',
                    questions_amount: 10,
                    students_amount: 10,
                    verification_type: 'Manual'
                }, {
                    quiz_name: 'Theory',
                    group_name: '.Net 2015',
                    questions_amount: 17,
                    students_amount: 15,
                    verification_type: 'Combined(A:13 M:4)'
                }, {
                    quiz_name: 'Logic',
                    group_name: 'Java',
                    questions_amount: 14,
                    students_amount: 7,
                    verification_type: 'Auto'
                }, {
                    quiz_name: 'SQL',
                    group_name: '.Net 2015',
                    questions_amount: 10,
                    students_amount: 10,
                    verification_type: 'Manual'
                }, {
                    quiz_name: 'Theory',
                    group_name: '.Net 2015',
                    questions_amount: 17,
                    students_amount: 15,
                    verification_type: 'Combined(A:13 M:4)'
                }, {
                    quiz_name: 'Logic',
                    group_name: 'Java',
                    questions_amount: 14,
                    students_amount: 7,
                    verification_type: 'Auto'
                }, {
                    quiz_name: 'SQL',
                    group_name: '.Net 2015',
                    questions_amount: 10,
                    students_amount: 10,
                    verification_type: 'Manual'
                }, {
                    quiz_name: 'Theory',
                    group_name: '.Net 2015',
                    questions_amount: 17,
                    students_amount: 15,
                    verification_type: 'Combined(A:13 M:4)'
                }, {
                    quiz_name: 'Logic',
                    group_name: 'Java',
                    questions_amount: 14,
                    students_amount: 7,
                    verification_type: 'Auto'
                }, {
                    quiz_name: 'SQL',
                    group_name: '.Net 2015',
                    questions_amount: 10,
                    students_amount: 10,
                    verification_type: 'Manual'
                }, {
                    quiz_name: 'Theory',
                    group_name: '.Net 2015',
                    questions_amount: 17,
                    students_amount: 15,
                    verification_type: 'Combined(A:13 M:4)'
                }, {
                    quiz_name: 'Logic',
                    group_name: 'Java',
                    questions_amount: 14,
                    students_amount: 7,
                    verification_type: 'Auto'
                }, {
                    quiz_name: 'SQL',
                    group_name: '.Net 2015',
                    questions_amount: 10,
                    students_amount: 10,
                    verification_type: 'Manual'
                }, {
                    quiz_name: 'Theory',
                    group_name: '.Net 2015',
                    questions_amount: 17,
                    students_amount: 15,
                    verification_type: 'Combined(A:13 M:4)'
                }, {
                    quiz_name: 'Logic',
                    group_name: 'Java',
                    questions_amount: 14,
                    students_amount: 7,
                    verification_type: 'Auto'
                }, {
                    quiz_name: 'SQL',
                    group_name: '.Net 2015',
                    questions_amount: 10,
                    students_amount: 10,
                    verification_type: 'Manual'
                }, {
                    quiz_name: 'Theory',
                    group_name: '.Net 2015',
                    questions_amount: 17,
                    students_amount: 15,
                    verification_type: 'Combined(A:13 M:4)'
                }, {
                    quiz_name: 'Logic',
                    group_name: 'Java',
                    questions_amount: 14,
                    students_amount: 7,
                    verification_type: 'Auto'
                }, {
                    quiz_name: 'SQL',
                    group_name: '.Net 2015',
                    questions_amount: 10,
                    students_amount: 10,
                    verification_type: 'Manual'
                }
            ];
            return promise;
        }
    }

})(angular);