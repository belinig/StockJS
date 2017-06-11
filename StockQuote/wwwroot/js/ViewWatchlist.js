angular.module("Watch")
    .controller('viewCtrl', function ($scope, $interval, $timeout, $window, pollInterval, SRV_getWatchlistQuotes, SRV_getWatchlists, SRV_getDateTime, Watchlist) {
        $scope.model = {};
        $scope.watchlists = Watchlist.getWatchlists();
        var initWatchlist = viewModel.watch.name;
        Watchlist.setSelectedWatchlistName(initWatchlist);
        //$scope.watchlist = initWatchlist;
        $scope.notification = "Updating...";
        $scope.fontSize;
        $scope.pollInterval = pollInterval;
        $scope.first = true;


        $window.onload = function (e)
        {
            console.log("windows.onload");
            SRV_getWatchlists.getWs($scope);
            SRV_getWatchlistQuotes.getW($scope, Watchlist.getSelectedWatchlistName(), $scope.setWatchlist);
        }

        $scope.getWatchlist = function () {
            SRV_getWatchlistQuotes.getW($scope, Watchlist.getSelectedWatchlistName(), $scope.setWatchlist);
        };

        $scope.getWatchlists = function () {
            SRV_getWatchlists.getWs($scope)  // call service
        };

        $scope.getDateTime = function () {
            SRV_getDateTime.getDateTime($scope)
        };

        $scope.getDate = function () {
            var date = new Date();
            return date;
        };

        $scope.quoteDate = $scope.getDate;

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
            SRV_getWatchlistQuotes.getW($scope, Watchlist.setSelectedWatchlistName(watchlist.name), $scope.setWatchlist, $scope.setWatchlist);
        }

        $scope.setWatchlist = function (watchlist) {
            $scope.model.watchlist = watchlist;
            $scope.selectedWatchlist = $scope.model.watchlist;
            $scope.$apply();
        }

        $scope.order = function () {
            return $scope.order = 'name';
        };

        $scope.setSortOrder = function (field) {
            $scope.sortDir = !$scope.sortDir;
            return $scope.order = field;
        };

        SRV_getWatchlists.getWs($scope);
        SRV_getWatchlistQuotes.getW($scope, Watchlist.getSelectedWatchlistName(), $scope.setWatchlist);
    });

  