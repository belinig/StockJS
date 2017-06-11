using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StockQuote.Components.Watchlist
{
    [ViewComponent(Name = "EditWatchlist")]
    public class EditViewComponent : ViewComponent
    {
        public class ViewModel
        {
            public WatchViewModel Watch { get; }

            public ViewModel(WatchViewModel watch)
            {
                Watch = watch;
            }
        }

        public class WatchViewModel
        {
            public string Name { get; }

            public WatchViewModel(string name)
            {
                Name = name;
            }
        }

        //public IViewComponentResult Invoke()
        //{
        //    return Content("Navigation");
        //}
        public static class RouteNames
        {
            public const string About = nameof(About);
            public const string Contact = nameof(Contact);
            public const string Home = nameof(Home);
        }

        private readonly IHostingEnvironment _environment;

        public EditViewComponent(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        //public IViewComponentResult Invoke()
        //{
        //    ItemViewModel[] navigationItems = new ItemViewModel[]{
        //        new ItemViewModel("Home", Url.RouteUrl(RouteNames.Home)),
        //        new ItemViewModel("Contact", Url.RouteUrl(RouteNames.Contact)),
        //        new ItemViewModel("About", Url.RouteUrl(RouteNames.About))
        //        };

        //    var viewModel = new ViewModel(navigationItems);

        //    return View(viewModel);
        //}

        public async Task<IViewComponentResult> InvokeAsync(string name)
        {
            return await Task.Run(() =>
            {
                var viewModel = new ViewModel(new WatchViewModel(name));

                return View(viewModel);
            });
        }
    }
}
