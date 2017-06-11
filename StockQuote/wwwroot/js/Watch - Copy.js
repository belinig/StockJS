(function () {
    //'use strict';
// works best in chrome and firefox - developed in chrome browser version 54

    var app = angular.module("Watch", ['ngAnimate']);
  pollInterval = 5000;   // interval of stock quoter service call in milliseconds
  var model = {user: "Andy"};
  var first = true;


      // These symbols will be used in the service call to the stock quotation service
     //var symbols = [".IXIC", ".DJI", ".INX", "MU", "AAPL", "GOOG", "ORCL"];

     // for display of company name associated with ticker
     // AJAX call will populate this object
     var companyName = {};


 app.service('SRV_getWatchlist', function($http, $q, $rootScope)
 {
        this.getW = function ($scope) {
            console.log("SRV_getWatchlist.getW")

            $scope.notification = "Updating Watchlist...";

            first = false;
            var url = buildWatchlistURL($scope.watchlist);

            // Get stock Quotes every {{pollInterval}} seconds
            $.ajax({
                url: url,
                type: "GET",
                async: true,
                dataType: 'json'
            }).then(function (wdata) {
                $scope.notification = "";
                parseWatchlistData(wdata, $scope.watchlist);
                model.wquotes = wdata.wquotes;
                $scope.model = model;
                $scope.$apply();
            })
                .fail(function () {
                    alert("Ajax failed getting watchlist quotes");
                });

        }
    }
    );


	// Build URL (for quoter service) from the symbols array
	function buildWatchlistURL($watchlist) {
	    var url = '/api/QuoteApi/Watchlist/'
	    console.log("buildWatchlistURL" + "watchlist=" + $watchlist);
	    return url + $watchlist;
	}

	// parse results from watchlist stock quotes call
	function parseWatchlistData(wdata, watchlist) {
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


  app.service('SRV_getDateTime', function(){
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
    });


     app.directive("myDateTime", function() {

          return {
        template : '<span>{{ curDateTime }}</span>',
	    	link: function (scope, element, attrs) {
    }
    };
});



/*
	app.filter("calcSum", function () {
        return function (data, key, key2, scope) {

	    if (angular.isUndefined(data) && angular.isUndefined(key))
            return 0;

          var sum = 0;

          angular.forEach(data,function(v,k){
		    if (isNaN(v[key])) {}
    else {

            var shares = v[key2];
            if  (!angular.isUndefined(shares)) {
              var value = (v[key]);
              shares = parseInt(shares);
              if (key == "dayGainLoss")
                 shares = 1;
         	  sum = sum + (value * shares);
 			}
        }});


        if (key == "dayGainLoss")
          scope.totalGainLossDay = sum;
        else if (key == "cost")
          scope.totalCost = sum;
        else if (key == "l")
           scope.totalValue = sum;

         if (scope.mode == "Extended Trading")
             if (key != "dayGainLoss")
                 sum = 0;
        return sum;
    }
})
*/

app.controller('myCtrl', function($scope, $interval, $timeout, $window, SRV_getWatchlist, SRV_getDateTime) {
        $scope.model = model;
    $scope.watchlists = ["Oil", "Banks", "JIB Portfolio"];
    var initWatchlist = "@Model.Watch.Name";
    //$scope.watchlist = initWatchlist;
	$scope.notification = "Updating...";
    $scope.fontSize;
    $scope.pollInterval = pollInterval;


    $window.onload = function (e) {SRV_getWatchlist.getW($scope);}

    $scope.getWatchlist = function () {
        SRV_getWatchlist.getW($scope)  // call service
    };

     $scope.getDateTime = function() {
        SRV_getDateTime.getDateTime($scope)
    };

     $interval($scope.getDateTime, 1000);

     $timeout($scope.getWatchlist, 1000);

	    $scope.getColor = function(value){
		   var color = {'color':'green', 'font-weight':'bold'};
		   if (value < 0)
	           color = {'color':'red', 'font-weight':'bold'};
		   return color;
        }

    /*
		 $scope.getColor_totalgainloss = function(){
		   var color = {'color':'green', 'font-weight':'bold'};
           var total = $scope.totalValue - $scope.totalCost - $scope.cash;
           if (total < 0)
	           color = {'color':'red', 'font-weight':'bold'};
           if ($scope.mode == "Extended Trading")
               var color = {'color':'green', 'font-weight':'bold'};
		   return color;
        }


		 $scope.getColor_totalgainlossDay = function(){

		   var color = {'color':'green', 'font-weight':'bold'};
	       if ($scope.totalGainLossDay < 0)
	           color = {'color':'red', 'font-weight':'bold'};
		   return color;
        }

    */

        $scope.getFontSize = function(){
             return $scope.fontSize;
        }


		$scope.changeWatchlist = function(watchlist){
        SRV_getWatchlist.getW($scope)  // call service
    }


    $scope.order = function() {
	           return $scope.order = 'name';
         };

	    $scope.setSortOrder = function(field) {
        $scope.sortDir = !$scope.sortDir;
    return $scope.order = field;
        };

});
})();