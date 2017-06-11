using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Jarloo.CardStock.Models;
using System.Collections.ObjectModel;
using Jarloo.CardStock.Helpers;
using MongoDB.Bson;
using StockQuote.Models;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace StockQuote.Controllers
{
    [Authorize]
    public class WatchlistController : Controller
    {
        [HttpGet]
        public IActionResult Watch()
        {
            return View("QuoteTabs");
        }

        [HttpGet]
        public IActionResult ModalStockCodeSearch()
        {
            return new LocalRedirectResult("~/html/ModalStockCodeSearch.html");
        }
    }
}
