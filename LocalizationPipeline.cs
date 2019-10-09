using AspNetCore.RequestLocalizationPipeline;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.RequestLocalizationPipeline
{
    public static class LocalizationPipeline
    {
        public static void AddCulturePipeLine(this MvcOptions opts, string routeConvention)
        {
            // Add an api convention for all controllers
            opts.Conventions.Insert(0, new RouteConvention(new RouteAttribute(routeConvention)));
            // Add
            opts.Filters.Add(new MiddlewareFilterAttribute(typeof(LocalizationFilter)));
        }
    }
}
