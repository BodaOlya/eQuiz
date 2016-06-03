(function (angular) {
    angular
        .module('equizModule')
        .filter('trustAsHtml', trustAsHtml);

    trustAsHtml.$inject = ['$sce'];

    function trustAsHtml ($sce) {
        return function (input) {
            return $sce.trustAsHtml(input);
        }
    }

})(angular);