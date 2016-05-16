(function (angular) {

    angular
        .module("equizModule")
        .factory("reviewDataService", reviewDataService);

    reviewDataService.$inject = ["$http"];

    function reviewDataService($http) {

        var service = {
            getStudents: getStudentsAjax,
        };

        return service;

        function getStudentsAjax() {
            var promise = [
      {
          id: 1,
          student: 'Eugene Shtefaniuk',
          userGroup: 'User',
          quizzes: 'dotNet, HTML'
      }, {
          id: 2,
          student: 'Dmytro Maherovskyi',
          userGroup: 'Admin',
          quizzes: 'dotNet, Angular'
      }, {
          id: 3,
          student: 'Halyna Posivnych',
          userGroup: 'Admin',
          quizzes: 'dotNet'
      }, {
          id: 4,
          student: 'Lev-Ivan Bulyk',
          userGroup: 'User',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 5,
          student: 'Ostap Herelei',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 6,
          student: 'Pavlo Demskyi',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      },
      {
          id: 7,
          student: 'Eugene Shtefaniuk',
          userGroup: 'User',
          quizzes: 'dotNet, HTML'
      }, {
          id: 8,
          student: 'Dmytro Maherovskyi',
          userGroup: 'Admin',
          quizzes: 'dotNet, Angular'
      }, {
          id: 9,
          student: 'Halyna Posivnych',
          userGroup: 'Admin',
          quizzes: 'dotNet'
      }, {
          id: 10,
          student: 'Lev-Ivan Bulyk',
          userGroup: 'User',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 11,
          student: 'Ostap Herelei',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 12,
          student: 'Pavlo Demskyi',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      },
      {
          id: 13,
          student: 'Eugene Shtefaniuk',
          userGroup: 'User',
          quizzes: 'dotNet, HTML'
      }, {
          id: 14,
          student: 'Dmytro Maherovskyi',
          userGroup: 'Admin',
          quizzes: 'dotNet, Angular'
      }, {
          id: 15,
          student: 'Halyna Posivnych',
          userGroup: 'Admin',
          quizzes: 'dotNet'
      }, {
          id: 16,
          student: 'Lev-Ivan Bulyk',
          userGroup: 'User',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 17,
          student: 'Ostap Herelei',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 18,
          student: 'Pavlo Demskyi',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      },
      {
          id: 19,
          student: 'Eugene Shtefaniuk',
          userGroup: 'User',
          quizzes: 'dotNet, HTML'
      }, {
          id: 20,
          student: 'Dmytro Maherovskyi',
          userGroup: 'Admin',
          quizzes: 'dotNet, Angular'
      }, {
          id: 21,
          student: 'Halyna Posivnych',
          userGroup: 'Admin',
          quizzes: 'dotNet'
      }, {
          id: 22,
          student: 'Lev-Ivan Bulyk',
          userGroup: 'User',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 23,
          student: 'Ostap Herelei',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 24,
          student: 'Pavlo Demskyi',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      },
      {
          id: 25,
          student: 'Eugene Shtefaniuk',
          userGroup: 'User',
          quizzes: 'dotNet, HTML'
      }, {
          id: 26,
          student: 'Dmytro Maherovskyi',
          userGroup: 'Admin',
          quizzes: 'dotNet, Angular'
      }, {
          id: 27,
          student: 'Halyna Posivnych',
          userGroup: 'Admin',
          quizzes: 'dotNet'
      }, {
          id: 28,
          student: 'Lev-Ivan Bulyk',
          userGroup: 'User',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 29,
          student: 'Ostap Herelei',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 30,
          student: 'Pavlo Demskyi',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      },
      {
          id: 31,
          student: 'Eugene Shtefaniuk',
          userGroup: 'User',
          quizzes: 'dotNet, HTML'
      }, {
          id: 32,
          student: 'Dmytro Maherovskyi',
          userGroup: 'Admin',
          quizzes: 'dotNet, Angular'
      }, {
          id: 33,
          student: 'Halyna Posivnych',
          userGroup: 'Admin',
          quizzes: 'dotNet'
      }, {
          id: 34,
          student: 'Lev-Ivan Bulyk',
          userGroup: 'User',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 35,
          student: 'Ostap Herelei',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 36,
          student: 'Pavlo Demskyi',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      },
      {
          id: 37,
          student: 'Eugene Shtefaniuk',
          userGroup: 'User',
          quizzes: 'dotNet, HTML'
      }, {
          id: 38,
          student: 'Dmytro Maherovskyi',
          userGroup: 'Admin',
          quizzes: 'dotNet, Angular'
      }, {
          id: 39,
          student: 'Halyna Posivnych',
          userGroup: 'Admin',
          quizzes: 'dotNet'
      }, {
          id: 40,
          student: 'Lev-Ivan Bulyk',
          userGroup: 'User',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 41,
          student: 'Ostap Herelei',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 42,
          student: 'Pavlo Demskyi',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      },
      {
          id: 43,
          student: 'Eugene Shtefaniuk',
          userGroup: 'User',
          quizzes: 'dotNet, HTML'
      }, {
          id: 44,
          student: 'Dmytro Maherovskyi',
          userGroup: 'Admin',
          quizzes: 'dotNet, Angular'
      }, {
          id: 45,
          student: 'Halyna Posivnych',
          userGroup: 'Admin',
          quizzes: 'dotNet'
      }, {
          id: 46,
          student: 'Lev-Ivan Bulyk',
          userGroup: 'User',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 47,
          student: 'Ostap Herelei',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 48,
          student: 'Pavlo Demskyi',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      },
      {
          id: 49,
          student: 'Eugene Shtefaniuk',
          userGroup: 'User',
          quizzes: 'dotNet, HTML'
      }, {
          id: 50,
          student: 'Dmytro Maherovskyi',
          userGroup: 'Admin',
          quizzes: 'dotNet, Angular'
      }, {
          id: 51,
          student: 'Halyna Posivnych',
          userGroup: 'Admin',
          quizzes: 'dotNet'
      }, {
          id: 52,
          student: 'Lev-Ivan Bulyk',
          userGroup: 'User',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 53,
          student: 'Ostap Herelei',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 54,
          student: 'Pavlo Demskyi',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      },
      {
          id: 55,
          student: 'Eugene Shtefaniuk',
          userGroup: 'User',
          quizzes: 'dotNet, HTML'
      }, {
          id: 56,
          student: 'Dmytro Maherovskyi',
          userGroup: 'Admin',
          quizzes: 'dotNet, Angular'
      }, {
          id: 57,
          student: 'Halyna Posivnych',
          userGroup: 'Admin',
          quizzes: 'dotNet'
      }, {
          id: 58,
          student: 'Lev-Ivan Bulyk',
          userGroup: 'User',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 59,
          student: 'Ostap Herelei',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 60,
          student: 'Pavlo Demskyi',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      },
      {
          id: 61,
          student: 'Eugene Shtefaniuk',
          userGroup: 'User',
          quizzes: 'dotNet, HTML'
      }, {
          id: 62,
          student: 'Dmytro Maherovskyi',
          userGroup: 'Admin',
          quizzes: 'dotNet, Angular'
      }, {
          id: 63,
          student: 'Halyna Posivnych',
          userGroup: 'Admin',
          quizzes: 'dotNet'
      }, {
          id: 64,
          student: 'Lev-Ivan Bulyk',
          userGroup: 'User',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 65,
          student: 'Ostap Herelei',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      }, {
          id: 66,
          student: 'Pavlo Demskyi',
          userGroup: 'Moderator',
          quizzes: 'ASP.NET MVC'
      }
            ];
            return promise;
        }
    }

})(angular);