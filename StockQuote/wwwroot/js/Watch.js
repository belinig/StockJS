angular.module("Watch", ['ngAnimate', 'ui.bootstrap'])

    .constant("pollInterval", 5000)
    .factory('Watchlist', function () {
        class Watchlist {
            constructor(name) {
                this.name = name;
                this.modified = false;
                this.id = null;
            }

        } 

        class WatchlistQuote {
            constructor(quotedatetime) {
                this.quotedatetime = quotedatetime;
            }
        } 

        class Stock {
            constructor(symbol) {
                this.symbol = symbol;
                this.removed = false;
            }
        }

        class StockQuote extends Stock {
            constructor(symbol, price, lastUpdate) {
                super(symbol)
                this.price = price;
                this.lastUpdate = lastUpdate;
            }
        }

        class StocksContext {
            constructor() {
                this.selectWatchlistName = null;
                this.watchlists = {};
            }
        }

        let sContext = new StocksContext();

        // parse results from watchlist stock quotes call
        var addWatchlistMethod = function (id, name, stocks) {
            console.log("addWatchlistMethod name=" + name);
            let watchlist = new Watchlist(name);
            watchlist.id = id;
            watchlist.stocks = stocks;
            sContext.watchlists[name] = watchlist;
            return watchlist;
        }

        var createStockMethod = function (name) {
            let stock = new Stock(name);
            return stock;
        }

        var createStockQuoteMethod = function (name, price, lastUpdate) {
            let stock = new StockQuote(name, price, lastUpdate);
            return stock;
        }

        var getWatchlistsMethod = function () {
            return sContext.watchlists;
        }

        var getWatchlistMethod = function (watchlistname) {
            if (sContext.watchlists.hasOwnProperty(watchlistname)) {
                return sContext.watchlists[watchlistname];
            }
            else 
            {
                return null;
            }
        }

        var removeWatchlistMethod = function (watchlistname) {
            if (sContext.watchlists.hasOwnProperty(watchlistname)) {
                delete sContext.watchlists[watchlistname];
            }
        }

        var getSelectedWatchlistNameMethod = function () {
            return sContext.selectWatchlistName;
        }

        var setSelectedWatchlistNameMethod = function (name) {
            sContext.selectWatchlistName = name;
            return sContext.selectWatchlistName;
        }

        var getSelectedWatchlistMethod = function () {
            return getWatchlistMethod(getSelectedWatchlistNameMethod());
        }

        var addStocktoWatchlistMethod = function (watchlistname, stockname) {
            var stock = createStockMethod(stockname);
            var watchlist = getWatchlistMethod(watchlistname);
            if (watchlist !== null)
            {
                if (watchlist.stocks === null)
                {
                    watchlist.stocks = [];
                }
                watchlist.stocks.push(stock);
            }
        }

        return {
            addWatchlist: addWatchlistMethod,
            getWatchlists: getWatchlistsMethod,
            createStockQuote: createStockQuoteMethod,
            createStock: createStockMethod,
            getSelectedWatchlistName: getSelectedWatchlistNameMethod,
            setSelectedWatchlistName: setSelectedWatchlistNameMethod,
            getWatchlist: getWatchlistMethod,
            addStocktoWatchlist: addStocktoWatchlistMethod,
            getSelectedWatchlist: getSelectedWatchlistMethod,
            removeWatchlist: removeWatchlistMethod

        };
    })
    // Build URL (for quoter service) from the symbols array
    .factory('wf', ['Watchlist', function (Watchlist) {
        var buildWatchlistURLMethod = function (watchlist) {
            var url = '/api/QuoteApi/Watchlist/Current/'
            console.log("buildWatchlistURL" + "watchlist=" + watchlist);
            return url + watchlist;
        }
        var buildSaveWatchlistURLMethod = function (watchlist) {
            var url = '/api/QuoteApi/Watchlist/Save'
            console.log("buildSaveWatchlistURL" + "watchlist=" + watchlist);
            return url;
        }
        var buildGetWatchlistsURLMethod = function () {
            var url = '/api/QuoteApi/Watchlists/Get'
            console.log("buildGetWatchlistsURL:" + url);
            return url;
        }
        // parse results from watchlist stock quotes call
        var parseWatchlistDataMethod = function (wdata, watchlistname) {
            console.log("parseWatchlistData start");
            var wquotes = [];
            if (wdata == null) {
                console.log("wdata empty");
                return;
            }
            var watchlist = Watchlist.addWatchlist(wdata.watchlist.id, watchlistname, wquotes);
            for (i = 0; i < wdata.quotes.length; i++) {
                var stockQuote = Watchlist.createStockQuote(wdata.quotes[i].symbol, Number(wdata.quotes[i].lastTradePrice), wdata.quotes[i].lastUpdate);
                wquotes.push(stockQuote);
            }
        
            console.log("parseWatchlistData end, items parsed count=" + wquotes.length);
            return watchlist;
        }


        return {
            buildWatchlistURL: buildWatchlistURLMethod,
            parseWatchlistData: parseWatchlistDataMethod,
            buildSaveWatchlistURL: buildSaveWatchlistURLMethod,
            buildGetWatchlistsURL   : buildGetWatchlistsURLMethod
        };
     }])
    .service('SRV_getWatchlistQuotes', function($http, $q, $rootScope, wf) {
        this.getW = function ($scope, watchlistname, setWatchlist) {
                console.log("SRV_getWatchlistQuotes.getW")

                $scope.notification = "Loading watchlist:" + watchlistname + " ...";

                if (watchlistname == null)
                {
                    console.log("SRV_getWatchlist.getW no watchlistname selected")
                    return;
                }

                first = false;
                var watchlist = null;
                var url = wf.buildWatchlistURL(watchlistname);

                // Get stock Quotes every {{pollInterval}} seconds
                $.ajax({
                    url: url,
                    type: "GET",
                    async: true,
                    dataType: 'json'
                }).then(function (wdata) {
                    $scope.notification = "";
                    watchlist = wf.parseWatchlistData(wdata, watchlistname);
                    setWatchlist(watchlist);
                 })
                    .fail(function () {
                        alert("Ajax failed getting watchlist quotes");
                    });
                }
    })
    .service('SRV_getWatchlists', function ($http, $q, $rootScope, wf, Watchlist) {
        this.getWs = function ($scope) {
            console.log("SRV_getWatchlists.getWs")

            $scope.notification = "Retriving Watchlists...";

            first = false;
            var url = wf.buildGetWatchlistsURL();

            // Get stock Quotes every {{pollInterval}} seconds
            $.ajax({
                url: url,
                type: "GET",
                async: true,
                dataType: 'json'
            }).then(function (wdata) {
                $scope.notification = "";

                if (wdata != null)
                {
                    for (i = 0; i < wdata.length; i++) {
                        Watchlist.addWatchlist(null, wdata[i]);
                    }
                }
               
            })
                .fail(function () {
                    alert("Ajax failed getting watchlists names");
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
    .service('SRV_getDate', function () {
        this.getDate = function ($scope) {
            var date = new Date();
            var datestring = (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear();
            $scope.curDate = datestring;
        }
    })
        .directive("myDateTime", function () {
            return {
                template: '<span>{{ curDateTime }}</span>',
                link: function (scope, element, attrs) { }
            }
    })
    .controller('tabsCtrl', function ($scope, $interval, $timeout, $window, pollInterval, SRV_getWatchlistQuotes, SRV_getWatchlists, SRV_getDateTime, Watchlist) {
        $scope.onWatchSelected = function (watchlist) {
            //$windows.alert("onWatchSelected");
        }

        $scope.onWatchSelected = function (watchlist) {
           //$windows.alert("onEditSelected");
        }

        $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            var target = $(e.target).attr("href") // activated tab
            console.log(target + "activated")
            //alert(target);
        });
    });
    

