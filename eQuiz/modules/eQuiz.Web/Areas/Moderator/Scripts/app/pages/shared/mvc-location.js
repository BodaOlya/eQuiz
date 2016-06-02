(function () {
    angular.module('equizModule')
           .factory('mvcLocation', function () {
               return {
                   search: search
               };

               function search(name) {
                   var url = window.location.href;
                   name = name.replace(/[\[\]]/g, "\\$&");
                   var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
                       results = regex.exec(url);
                   if (!results) return null;
                   if (!results[2]) return '';
                   return decodeURIComponent(results[2].replace(/\+/g, " ").replace('/', ''));
               }
           });
})();