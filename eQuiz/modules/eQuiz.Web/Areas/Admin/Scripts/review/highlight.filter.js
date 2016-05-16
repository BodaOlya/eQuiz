(function (angular) {
    angular.module("equizModule").filter('highlight', HighlightFilter);

    HighlightFilter.$inject = ['$sce'];

    function HighlightFilter ($sce) {
        return function (text, phrase) {
            if (phrase) {
                text = text.replace(new RegExp('(' + phrase + ')', 'gi'), '<span class="highlightedText">$1</span>');
            }

            return $sce.trustAsHtml(text);
        }
    };
})(angular);