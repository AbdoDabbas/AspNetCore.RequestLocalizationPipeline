using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;

namespace AspNetCore.RequestLocalizationPipeline
{
    public static class LocalizationRewritesSupport
    {
        public static IApplicationBuilder UseTwoLettersCultureRewrite(this IApplicationBuilder app, string routeConvention)
        {
            LocalizationPipeline.RouteConvention = routeConvention;
            int cultureRouteKeyIndex = LocalizationOptions.GetCultureRouteKeyIndex();

            var rewOpts = new RewriteOptions();
            rewOpts
               .Add(context =>
               {
                   if (cultureRouteKeyIndex <= -1)
                       return;

                   var requestSegments = context.HttpContext.Request.Path.Value.Split('/');
                   if (string.IsNullOrEmpty(requestSegments[0]))
                       requestSegments = requestSegments.Skip(1).ToArray();

                   // if lang exists just check if it's valid one
                   if (cultureRouteKeyIndex <= requestSegments.Length - 1)
                   {
                       var reqLang = requestSegments[cultureRouteKeyIndex];
                       // Is supported language
                       var lang = LocalizationOptions.SupportedCultures.FirstOrDefault(
                           a => a.TwoLetterISOLanguageName.Equals(
                               reqLang, StringComparison.InvariantCultureIgnoreCase));
                       if (lang != null)
                       {
                           requestSegments[cultureRouteKeyIndex] = lang.Name;
                           context.HttpContext.Request.Path = "/" + string.Join("/", requestSegments);
                       }
                   }
               });

            app.UseRewriter(rewOpts);

            return app;
        }

        public static IApplicationBuilder UseDefaultCultureRewrite(this IApplicationBuilder app, string routeConvention)
        {
            LocalizationPipeline.RouteConvention = routeConvention;
            int cultureRouteKeyIndex = LocalizationOptions.GetCultureRouteKeyIndex();
            var routeConvSegs = LocalizationPipeline.RouteConvention.Split('/');

            var rewrOpts = new RewriteOptions();
            rewrOpts
               .Add(context =>
               {
                   if (cultureRouteKeyIndex <= -1)
                       return;

                   string noLangRequestPathPattern = "";
                   // Generate the empty language request

                   var lst = routeConvSegs.ToList();
                   lst.RemoveAt(cultureRouteKeyIndex);
                   noLangRequestPathPattern = "^(/" + string.Join('/', lst) + "/.*)$";

                   string requestStr = context.HttpContext.Request.Path.Value;
                   // If it's request without lang
                   if (Regex.IsMatch(requestStr, noLangRequestPathPattern))
                   {
                       // Generate one with the default lang
                       var reqSegs = requestStr.Split('/').ToList();
                       if (string.IsNullOrEmpty(reqSegs[0]))
                           reqSegs = reqSegs.Skip(1).ToList();

                       reqSegs.Insert(cultureRouteKeyIndex, LocalizationOptions.DefaultCulture);

                       context.HttpContext.Request.Path = "/" + string.Join("/", reqSegs);
                   }
               });

            app.UseRewriter(rewrOpts);

            return app;
        }
    }
}