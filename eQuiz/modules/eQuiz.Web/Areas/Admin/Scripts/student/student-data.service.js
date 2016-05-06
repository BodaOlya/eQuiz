﻿(function (angular) {

    angular
        .module("equizModule")
        .factory("studentDataService", studentDataService);

    studentDataService.$inject = ["$http"];

    function studentDataService($http) {

        var service = {
            getStudentInfo: getStudentInfo,
            getStudentQuizzes: getStudentQuizzes,
            getStudentComments: getStudentComments,
            saveProfileInfo: saveProfileInfo
        };

        return service;

        function getStudentInfo(studentId) {
            //var promise = $http.get("/Main/GetStudentInfo", studentId);
            //return promise;
            return {
                firstName: 'Eugene',
                lastName: 'Shtefaniuk',
                phone: '555-15-734',
                email: 'yevhen.sht@gmail.com',
                userGroup: 'Student'
            };
        }

        function getStudentQuizzes(studentId) {
            //var promise = $http.get("/Main/GetStudentQuizzes", studentId);
            //return promise;
            return [
                {
                    name: 'Quiz 1',
                    state: 'Passed',
                    questions: 20,
                    verificationType: 'Auto',
                    otherDetails: 'Details 1'
                },
                {
                    name: 'Quiz 2',
                    state: 'In Verification',
                    questions: 10,
                    verificationType: 'Manual',
                    otherDetails: 'Details 2'
                },
                {
                    name: 'Quiz 3',
                    state: 'Not passed',
                    questions: 20,
                    verificationType: 'Combined (A:12, M:8)',
                    otherDetails: 'Details 3'
                }
            ];
        }

        function getStudentComments(studentId) {
            //var promise = $http.get("/Main/GetStudentComments", studentId);
            //return promise;

            return [
                {
                    date: '15.04.2016',
                    author: 'Volodymyr',
                    text: 'Responsible, initiative student with excellent knowledge of necessary frameworks'
                },
                {
                    date: '03.05.2016',
                    author: 'Ivan',
                    text: 'Demonstrates deep theoretical knowledge'
                }];
        }

        function saveProfileInfo(studentInfo, studentComments) {
            //var promise = $http.post("/Main/SaveStudentInfo", studentInfo);
            //var promise = $http.post("/Main/SaveStudentComments", studentComments);
            //return promise;
        }
    }

})(angular);