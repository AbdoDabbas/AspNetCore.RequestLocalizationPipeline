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
    public static class CultureSupportRewrites
    {
        public static IApplicationBuilder UseTwoLettersCultureRewrite(this IApplicationBuilder app, string routeConvention,
                                                                 string langKey, CultureInfo defaultCulture)
        {
            Regex langRegex = new Regex(@"\{" + langKey + @"( *:[\w= ]+)?\}");
            var routeConvSegs = routeConvention.Split('/');
            var langKeyIndex = Array.FindIndex(routeConvSegs, langRegex.IsMatch);

            // .AddRewrite(@"^(api/.*)", $"{supportedCulture.TwoLetterISOLanguageName}/$1",
            var rewrOpts = new RewriteOptions();
            rewrOpts
               .Add(context =>
               {

                   var requestSegments = context.HttpContext.Request.Path.Value.Split('/');
                   if (string.IsNullOrEmpty(requestSegments[0]))
                       requestSegments = requestSegments.Skip(1).ToArray();

                   // if lang exists just check if it's valid one
                   if (langKeyIndex > -1 && langKeyIndex <= requestSegments.Length - 1)
                   {
                       var reqLang = requestSegments[langKeyIndex];
                       // Is supported language
                       var lang = CulturesHelper.SupportedCultures.FirstOrDefault(
                           a => a.TwoLetterISOLanguageName.Equals(
                               reqLang, StringComparison.InvariantCultureIgnoreCase));
                       if (lang != null)
                       {
                           requestSegments[langKeyIndex] = lang.Name;
                           context.HttpContext.Request.Path = "/" + string.Join("/", requestSegments);
                       }
                   }
               });

            app.UseRewriter(rewrOpts);

            return app;
        }

        public static IApplicationBuilder UseDefaultCultureRewrite(this IApplicationBuilder app, string routeConvention, string langKey, CultureInfo defaultCulture)
        {
            Regex langRegex = new Regex(@"\{" + langKey + @"( *:[\w= ]+)?\}");
            var routeConvSegs = routeConvention.Split('/');
            var langKeyIndex = Array.FindIndex(routeConvSegs, langRegex.IsMatch);

            // .AddRewrite(@"^(api/.*)", $"{supportedCulture.TwoLetterISOLanguageName}/$1",
            var rewrOpts = new RewriteOptions();
            rewrOpts
               .Add(context =>
               {
                   string noLangRequestPathPattern = "";
                   // Generate the empty language request
                   if (langKeyIndex > -1)
                   {
                       var lst = routeConvSegs.ToList();
                       lst.RemoveAt(langKeyIndex);
                       noLangRequestPathPattern = "^(/" + string.Join('/', lst) + "/.*)$";
                   }

                   string requestStr = context.HttpContext.Request.Path.Value;
                   // If it's request without lang
                   if (Regex.IsMatch(requestStr, noLangRequestPathPattern))
                   {
                       // Generate one with the default lang
                       var reqSegs = requestStr.Split('/').ToList();
                       if (string.IsNullOrEmpty(reqSegs[0]))
                           reqSegs = reqSegs.Skip(1).ToList();

                       reqSegs.Insert(langKeyIndex, defaultCulture.Name);

                       context.HttpContext.Request.Path = "/" + string.Join("/", reqSegs);
                   }
               });

            app.UseRewriter(rewrOpts);

            return app;
        }
    }
}
