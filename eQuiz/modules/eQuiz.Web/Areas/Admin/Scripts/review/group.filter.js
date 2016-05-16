(function (angular) {
    angular.module('equizModule').filter('groupFilter', GroupFilter);

    function GroupFilter() {
        return function (data, selectedData) {
            if (!angular.isUndefined(data) && !angular.isUndefined(selectedData) && selectedData.length > 0) {
                var tempData = [];
                angular.forEach(selectedData, function (id) {
                    angular.forEach(data, function (item) {
                        if (angular.equals(item.userGroup, id)) { //property user group needs to be changed manualy
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