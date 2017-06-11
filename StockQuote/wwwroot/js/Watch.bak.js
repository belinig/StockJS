angular.module("Watch", ['ngAnimate'])

    .constant ("pollInterval", 5000)
    .controller('myCtrl', function ($scope, $interval, $timeout, $window, pollInterval, SRV_getWatchlist, SRV_getDateTime) {
            $scope.model = {};
            $scope.watchlists = ["Oil", "Banks", "JIB Portfolio"];
            var initWatchlist = "@Model.Watch.Name";
            //$scope.watchlist = initWatchlist;
            $scope.notification = "Updating...";
            $scope.fontSize;
            $scope.pollInterval = pollInterval;
            $scope.first = true;


            $window.onload = function (e) { SRV_getWatchlist.getW($scope); }

            $scope.getWatchlist = function () {
                SRV_getWatchlist.getW($scope)  // call service
            };

            $scope.getDateTime = function () {
                SRV_getDateTime.getDateTime($scope)
            };

            $interval($scope.getDateTime, 1000);

            $timeout($scope.getWatchlist, 1000);

            $scope.getColor = function (value) {
                var color = { 'color': 'green', 'font-weight': 'bold' };
                if (value < 0)
                    color = { 'color': 'red', 'font-weight': 'bold' };
                return color;
            }


            $scope.getFontSize = function () {
                return $scope.fontSize;
            }


            $scope.changeWatchlist = function (watchlist) {
                SRV_getWatchlist.getW($scope)  // call service
            }

            $scope.order = function () {
                return $scope.order = 'name';
            };

            $scope.setSortOrder = function (field) {
                $scope.sortDir = !$scope.sortDir;
                return $scope.order = field;
            };
        })


    // Build URL (for quoter service) from the symbols array
    .factory('wf', function () {
        var buildWatchlistURLMethod = function (watchlist) {
            var url = '/api/QuoteApi/Watchlist/'
            console.log("buildWatchlistURL" + "watchlist=" + watchlist);
            return url + watchlist;
        }
        // parse results from watchlist stock quotes call
        var parseWatchlistDataMethod = function (wdata, watchlist) {
            console.log("parseWatchlistData start");
            var wquotes = [];
            for (i = 0; i < wdata.length; i++) {

                var quote = {}
                quote.symbol = wdata[i].symbol;
                quote.price = Number(wdata[i].lastTradePrice);
                quote.date = wdata[i].lastUpdate;
                wquotes.push(quote);
            }

            console.log("parseWatchlistData end, items parsed count=" + wquotes.length);
            wdata.wquotes = wquotes;
        }


        return {
            buildWatchlistURL: buildWatchlistURLMethod,
            parseWatchlistData: parseWatchlistDataMethod
        };
     })

    .service('SRV_getWatchlist', function($http, $q, $rootScope, wf) {
            this.getW = function ($scope) {
                console.log("SRV_getWatchlist.getW")

                $scope.notification = "Updating Watchlist...";

                first = false;
                var url = wf.buildWatchlistURL($scope.watchlist);

                // Get stock Quotes every {{pollInterval}} seconds
                $.ajax({
                    url: url,
                    type: "GET",
                    async: true,
                    dataType: 'json'
                }).then(function (wdata) {
                    $scope.notification = "";
                    wf.parseWatchlistData(wdata, $scope.watchlist);
                    $scope.model.wquotes = wdata.wquotes;
                    $scope.$apply();
                })
                    .fail(function () {
                        alert("Ajax failed getting watchlist quotes");
                    });

                }
     })
    .service('SRV_getDateTime', function(){
            this.getDateTime = function ($scope) {

                var date = new Date();
                var secs = date.getSeconds();
                var mins = date.getMinutes();
                if (secs < 10) { secs = "0" + secs };
                if (mins < 10) { mins = "0" + mins };
                var datestring = (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear() +
                    " - " + date.getHours() + ":" + mins + ":" + secs;
                $scope.curDateTime = datestring;

            }
        })

        .directive("myDateTime", function () {
            return {
                template: '<span>{{ curDateTime }}</span>',
                link: function (scope, element, attrs) { }
            }
        });
    

