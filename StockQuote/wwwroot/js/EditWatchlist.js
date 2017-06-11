angular.module("Watch")
    .controller('editCtrl', function ($scope, $interval, $timeout, $window, $filter, $uibModal, pollInterval, SRV_getWatchlistQuotes, SRV_saveWatchlist, SRV_getDateTime, SRV_findASXStocks, Watchlist) {
        $scope.model = {};
        $scope.notification = "Loading...";
        $scope.fontSize;
        $scope.pollInterval = pollInterval;
        $scope.first = true;
        $scope.modified = false;
        $scope.stocks = {};

        //$('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
        //    var target = $(e.target).attr("href") // activated tab
        //    var x = $(e.target);         // active tab
        //    var y = $(e.relatedTarget);  // previous tab
        //    alert("editCtrl=" + target);
        //});

        $('a[data-toggle="tab"]').on('show.bs.tab', function (e) {
            var next = $(e.target).attr("href") // activated tab
            var previous = $(e.relatedTarget).attr("href");         // active tab
            console.log("about to show " + next + " and hide " + previous);
            if (next === "#edit") {
                $scope.watchlists = Watchlist.getWatchlists();
                $scope.model.watchlist = Watchlist.getWatchlist(Watchlist.getSelectedWatchlistName());
                $scope.selectedWatchlist = $scope.model.watchlist;
                $scope.modified = false;
            }
        });

        $('a[data-toggle="tab"]').on('hide.bs.tab', function (e) {
            var previous = $(e.target).attr("href") // activated tab
            var next = $(e.relatedTarget).attr("href");         // active tab
            console.log("about to hide " + previous + " and show " + next);
            if ($scope.modified) {
                $scope.notification = "Watchlist changes has not been saved. Please save or cancel before exiting edit mode";
                e.preventDefault();
            }
        });

        var colors = ["red", "blue", "green", "yellow", "brown", "black"];


        var $stockAddEdit = $('#stockAddEdit');

 
        $stockAddEdit.typeahead({
            hint: true,
            highlight: true,
            minLength: 1
        },
            {
                name: 'asxstocks',
                limit: 10,
                source: function (query, sync, async) {
                return $scope.findASXStocks(query, async);
                },
                templates: {
                    suggestion:
                    Handlebars.compile("<div><strong>{{code}}</strong><br/><div class=\"subject\"><i>{{name}}</i></div></div>")
                },
                display: function (sugestion) {
                    return sugestion.code;
                }
            });

        $scope.stock2AddElem = document.querySelector('#stockAddEdit.tt-input');
        $scope.$watch("stock2Add",
            function (newValue, oldValue) {
                if ($scope.stock2AddElem.value !== newValue)
                {
                    $scope.stock2AddElem.value = newValue;
                }
            }
        )


        if ($scope.selectedWatchlist != null && $scope.selectedWatchlist.id == null)
        {
            $scope.getWatchlist();
        }

        $scope.getWatchlist = function () {
            SRV_getWatchlistQuotes.getW($scope, Watchlist.getSelectedWatchlistName(), $scope.setWatchlist);
        };


        $scope.getDateTime = function () {
            SRV_getDateTime.getDateTime($scope)
        };

        $scope.findASXStocks = function (match, process) {
            SRV_findASXStocks.get($scope, match, process)
        };

        $scope.setFoundASXStocks = function (stocks) {
            $scope.stocks = stocks;
        };

        $interval($scope.getDateTime, 1000);


        $scope.getFontSize = function () {
            return this.fontSize;
        };


        $scope.changeWatchlist = function (watchlist) {
            if (watchlist !== null) {
                SRV_getWatchlistQuotes.getW($scope, Watchlist.setSelectedWatchlistName(watchlist.name), $scope.setWatchlist, $scope.setWatchlist);
            }
        }

        $scope.setWatchlist = function (watchlist) {
            $scope.model.watchlist = watchlist;
            $scope.selectedWatchlist = $scope.model.watchlist;
            $scope.$apply();
        }

        $scope.order = function () {
            return this.order = 'name';
        };

        $scope.setSortOrder = function (field) {
            this.sortDir = !$scope.sortDir;
            return $scope.order = field;
        };

        $scope.changeWatchlistItem = function (watchlist, itemId, newValue) {
            console.log("watchlist:" + watchlist + ",itemid:" + itemId + ",newValue:" + newValue);
            //$scope.model.wquotes[itemId] = newValue
        };

        $scope.newWatchlistEdit = function () {
            console.log("new watchlist");
            this.watchlist = "NewWachtlist";
            console.log($scope.watchlist);
            this.model.wquotes = [];
        };

        $scope.saveWatchlistEdit = function () {
            SRV_saveWatchlist.postW($scope, $scope.onWatchlistSaved)
            $scope.getWatchlist();
        };

        $scope.onWatchlistSaved = function (watchlist) {
            if (watchlist.Name === Watchlist.getSelectedWatchlistName()) {
                $scope.notification = "Reloading watchlist:" + Watchlist.getSelectedWatchlistName();
                watchlist.modified = false;
                Watchlist.removeWatchlist(Watchlist.getSelectedWatchlistName());
                $scope.getWatchlist();
            }
        }



        $scope.cancelWatchlistEdit = function () {
            $scope.modified = false;
        };

        $scope.removeStockFromWatchlistEdit = function (item) {
            item.removed = true;
            Watchlist.getSelectedWatchlist().modified = true;
            //$scope.$apply();
        };


        $scope.importWatchlistEdit = function () {
            console.log("importWatchlistEdit");
            $scope.modified = true;
            var file = $scope.files[0];
            var reader = new FileReader();
            reader.onload = function (loadEvent) {
                text = reader.result;
                $scope.csvImport = $filter("csvToObj")(text);
                $scope.notification = file.name + " red successfully; processing";
                if ($scope.csvImport != null && $scope.csvImport.length > 0)
                {
                    $scope.ShowImport($scope.csvImport[0]["Name"]);
                }
            }
            reader.onerror = function (err) {
                console.log(err, err.loaded
                    , err.loaded === 0
                    , file.Name);
                $scope.notification = "Failed to import file:" + file.Name + ":" + err;
            }

            $scope.notification = "Processing File";
            reader.readAsText(file);
        };


        $scope.addStock2WatchlistEdit = function (symbol) {
            console.log("addStock2WatchlistEdit to watchlist=" + this.selectedWatchlist.name + " stock=" + symbol);
            Watchlist.addStocktoWatchlist(this.selectedWatchlist.name, symbol);
            $scope.modified = true;
        };

        $scope.ShowImport = function (watchlist) {
            console.log("ShowImport watchlist:" + watchlist);
            console.log("ShowImport start");
            var wquotes = [];
            if ($scope.csvImport == null) {
                console.log("csvImport empty");
                return;
            }
            for (i = 0; i < $scope.csvImport.length; i++) {

                if (watchlist != $scope.csvImport[i]["Name"])
                {
                    continue;
                }
                var quote = {}
                quote.id = i;
                quote.removed = false;
                quote.symbol = $scope.csvImport[i]["Code"];
                quote.symbol += ".AX"
                wquotes.push(quote);
            }

            console.log("ShowImport end, items parsed count=" + wquotes.length);
            $scope.model.wquotes = wquotes;
            $scope.watchlist = watchlist;
            $scope.$apply()
        };


        $scope.searchStockCode = function (stock2Add) {
            $scope.stock2Add = $scope.stock2Add;
            var modalInstance = $uibModal.open({
                templateUrl: 'ModalStockCodeSearch',
                controller: 'findStockCtrl',
                windowClass: 'app-modal-window',
                resolve: {
                    syncData: () => stock2Add
                }
            });

            modalInstance.result.then(function (selectedItem) {
                $scope.stock2Add = selectedItem;
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };

        $scope.notification = "";
    })
    .filter("filterRemoved", function () {

        return function (items, $scope) {
            var resultArr = [];

            angular.forEach(items, function (item, idx) {

                if (item.removed !== true) {
                    resultArr.push(item);
                }

            });

            return resultArr;
        }

    })
    .service('SRV_saveWatchlist', function ($http, $q, $rootScope, wf, Watchlist) {
        this.postW = function ($scope, onWatchlistSaved) {
            console.log("SRV_saveWatchlist.postW")

            $scope.notification = "Updating Watchlist...";

            first = false;
            var url = wf.buildSaveWatchlistURL($scope.watchlist);

            var watchlistselected = Watchlist.getWatchlist(Watchlist.getSelectedWatchlistName());
            var watchlist = {};
            watchlist.Id = watchlistselected.id;
            watchlist.Name = watchlistselected.name;
            watchlist.Symbols = [];
            for (let stock of watchlistselected.stocks) {
                if (!stock.removed) {
                    watchlist.Symbols.push(stock.symbol);
                }
            };

            // Get stock Quotes every {{pollInterval}} seconds
            $.ajax({
                url: url,
                type: "POST",
                async: true,
                contentType: 'application/json',
                dataType: 'json',
                data: JSON.stringify(watchlist)
            }).then(function (wdata) {
                $scope.notification = wdata ? "watchlist saved:" : "watchlist save failed" + watchlist.Name;
                onWatchlistSaved(watchlist);
            })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    alert("Ajax failed saving watchlist:" + watchlist.Name);
                    $scope.notification = textStatus + ":" + jqXHR["responseText"];
                });

        }
    })
    .service('SRV_findASXStocks', function ($http, $q, $rootScope, wf, Watchlist) {
        this.get = function ($scope, match, onFound) {
            console.log("SRV_findASXStocks.get")
            return this.getN($scope, match, onFound, 4)
        }

        this.getN = function ($scope, match, onFound, top) {
            console.log("SRV_findASXStocks.getN")

            $scope.notification = "ASX stocks lookup ...";

            first = false;
            var url = "/api/QuoteApi/ASXStocks/Find/" + match + "/" + top;

            // Get stock Quotes every {{pollInterval}} seconds
            $.ajax({
                url: url,
                type: "POST",
                async: true,
                contentType: 'application/json',
                dataType: 'json',
            }).then(function (wdata) {
                $scope.notification = wdata ? "stocks loaded" : "stock load failed";
                onFound(wdata);
            })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    alert("Ajax failed loading ASX stocks:" + match);
                    $scope.notification = textStatus + ":" + jqXHR["responseText"];
                });

        }
    })
    .directive("fileread", [function () {
        return {
            scope: {
                fileread: "="
            },
            link: function (scope, element, attributes) {
                element.bind("change", function (changeEvent) {
                    var reader = new FileReader();
                    reader.onload = function (loadEvent) {
                        text = reader.result;
                        scope.$apply(function () {
                            scope.fileread = loadEvent.target.result;
                        });
                        scope.notification = file.Name +" red successfully; processing";
                    }
                    console.log("fileread");

                    reader.onerror = function (err) {
                        console.log(err, err.loaded
                            , err.loaded == 0
                            , file);
                        button.removeAttribute("disabled");
                        
                    }

                    scope.notification = "Processing File";
                    reader.readAsText(event.target.files[0]);

                });
            }
        }
    }])
    .directive('fileInput', ['$parse', function ($parse) {
        return {
            restrict: 'A',
            link: function (scope, elm, attrs) {
                elm.bind('change', function (){
                    $parse(attrs.fileInput)
                    .assign(scope, elm[0].files)
                    scope.$apply()
                })
            }
        }
    }])
    .filter('csvToObj', function (normalizeImportHeaderFilter) {
        return function (input) {
            var rows = input.split('\n');
            var obj = [];
            var header = [];
            var counter = 0;
            angular.forEach(rows, function (val) {
                var o = val.split(',');
                counter++;
                if (counter === 1)
                {
                    for (i = 0; i < o.length; i++) {
                        header[i] = normalizeImportHeaderFilter(o[i]);
                    }
                }
                else
                {
                    
                    var row = {};
                    for (i = 0; i < o.length; i++)
                    {
                        row[header[i]] = o[i];
                    }
                    obj.push(row);
                }
               
            });
            return obj;
        };
    })
    .filter('normalizeImportHeader', function () {
        return function (input) {
            var normalizeImportHeader = String(input);
            normalizeImportHeader = normalizeImportHeader.replace("($)", "Dollar").replace("(%)", "Percentage");
            normalizeImportHeader = normalizeImportHeader.replace(/\s/g, '');
            return normalizeImportHeader;
        };
    });

        
      

