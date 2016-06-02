(function (angular) {
    angular.module('equizModule')        
        .directive('importUsersFromFile', importUsers);

    function importUsers() {
        return {
            scope: {
                callback: '&'
            },
            link: function (scope, el, attrs) {
                el.bind('change', function (event) {
                    var files = event.target.files;
                    var file = files[0];

                    var data = new Array();

                    var reader = new FileReader();

                    reader.onload = function (event) {

                        var contents = event.target.result;

                        var allTextLines = contents.split(/\r\n|\n/);

                        for (var i = 0; i < allTextLines.length; i++) {

                            var line = allTextLines[i].split(',');

                            data.push({
                                FirstName: line[0],
                                LastName: line[1],
                                Email: line[2]
                            });
                        }
                        
                        scope.callback({data: data});                        
                    };

                    if (file) {
                         reader.readAsText(file);
                    }
                    
                    
                    scope.$apply();
                });
            }
        };
    };

})(angular)