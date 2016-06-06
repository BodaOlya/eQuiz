(function (angular) {
    angular
        .module('equizModule')
        .filter('groupFilter', GroupFilter);

    function GroupFilter() {
        return function (data, selectedData, propertyToFilter) {
            if (!angular.isUndefined(data) && !angular.isUndefined(selectedData) && selectedData.length > 0) {
                var tempData = [];
                angular.forEach(selectedData, function (id) {
                    angular.forEach(data, function (item) {
                        if (item[propertyToFilter].indexOf(id) != -1) {
                            tempData.push(item);
                        }
                    });
                });
                return tempData;
            } else {
                return data;
            }
        };
    };
})(angular);