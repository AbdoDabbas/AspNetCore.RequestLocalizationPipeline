using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Localization.Extend
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
