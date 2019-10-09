using AspNetCore.RequestLocalizationPipeline;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.RequestLocalizationPipeline
{
    public static class LocalizationPipeline
    {
        private static string _routeConvention;
        internal static string RouteConvention
        {
            get => _routeConvention;
            set
            {
                if (string.IsNullOrEmpty(_routeConvention))
                    _routeConvention = value;
            }
        }

        /// <summary>
        /// Add the localization filter to all actions in the api along with the route convention containing
        /// the culture route key.
        /// </summary>
        /// <param name="opts"></param>
        /// <param name="routeConvention">The route</param>
        public static void AddLocalizationPipeLine(this MvcOptions opts, string routeConvention)
        {
            RouteConvention = routeConvention;

            // Add an api convention for all controllers
            opts.Conventions.Insert(0, new RouteConvention(new RouteAttribute(routeConvention)));
            // Add filter to all actions
            opts.Filters.Add(new MiddlewareFilterAttribute(typeof(LocalizationFilter)));
        }
    }
}