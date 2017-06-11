using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StockQuote.Components
{
    [ViewComponent(Name = "Navigation")]
    public class NavigationViewComponent : ViewComponent
    {
        public class ViewModel
        {
            public IList<ItemViewModel> NavigationItems { get; }

            public ViewModel(IList<ItemViewModel> navigationItems)
            {
                NavigationItems = navigationItems;
            }
        }

        public class ItemViewModel
        {
            public string Name { get; }
            public string TargetUrl { get; }

            public ItemViewModel(string name, string targetUrl)
            {
                Name = name;
                TargetUrl = targetUrl;
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

        public NavigationViewComponent(IHostingEnvironment environment)
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

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.Run(() =>
            {
                List<ItemViewModel> navigationItems = new List < ItemViewModel >(){
                new ItemViewModel("Home", Url.RouteUrl(RouteNames.Home)),
                new ItemViewModel("Contact", Url.RouteUrl(RouteNames.Contact)),
                new ItemViewModel("About", Url.RouteUrl(RouteNames.About))
                };

                if (_environment.IsDevelopment())
                {
                    var debugItem = new ItemViewModel("Debug", "/debug");
                    navigationItems.Add(debugItem);
                }

                var viewModel = new ViewModel(navigationItems);

                return View(viewModel);
            });
        }
    }
}
