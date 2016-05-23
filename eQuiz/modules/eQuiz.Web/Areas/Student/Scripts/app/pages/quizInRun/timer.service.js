(function (angular) {
    var equizModule = angular.module("equizModule");

    equizModule.factory("timerService", ["$interval", function ($interval) {

        var stop = undefined;
        var minutes = 0;
        var seconds = 0;
        var color = 'black'

        var service = {
            data: getData(),
            startT: startTimer,
            stopT: stopTimer,
            resetT: resetTimer
        };

        return service;

        function getData() {
            var d = {
                sec: seconds,
                min: minutes,
                col: color
            };

            return d;
        };

        function startTimer(duration) {
            if (angular.isDefined(stop))
                return;
            minutes = duration;
            stop = $interval(function () {
                
                if (seconds > 0) {
                    seconds--;
                } else if (minutes > 0) {
                    minutes--;
                    seconds = 59;
                } else {
                    stopTimer();
                }
                if (minutes <= minutes / 10) {
                    color = 'red'
                }
                else {
                    color = 'black'
                }
                getData();
            }, 1000);
        };
            
        function stopTimer() {
            if (angular.isDefined(stop)) {
                $interval.cancel(stop);
                stop = undefined;
            }
        };

        function resetTimer() {
            stopTimer();
            if (seconds > 0 && service.seconds <= 60) {
                seconds = 0;
            }
            if (minutes > 0) {
                minutes = 0;
            }
            color = 'black';
        }

    }]);

})(angular);