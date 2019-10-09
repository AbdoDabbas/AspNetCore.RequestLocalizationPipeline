using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.RequestLocalizationPipeline
{
    public static class LocalizationOptions
    {
        public static List<CultureInfo> SupportedCultures => InitSupportedCultures();
        
        const string ArabicCulture = "ar-SY";

        internal static string DefaultCulture = ArabicCulture;
        internal static string CultureRouteKey = "";
        internal static int GetCultureRouteKeyIndex()
        {
            Regex langRegex = new Regex(@"\{" + LocalizationOptions.CultureRouteKey + @"( *:[\w= ]+)?\}");
            var routeConvSegs = LocalizationPipeline.RouteConvention.Split('/');
            return Array.FindIndex(routeConvSegs, langRegex.IsMatch);
        }

        private static List<CultureInfo> InitSupportedCultures()
        {
            var arabicCulture = new CultureInfo(ArabicCulture)
            {
                DateTimeFormat = {Calendar = new GregorianCalendar()}
            };

            return new List<CultureInfo>
            {
                arabicCulture,
                new CultureInfo("en-US")
            };
        }

        /// <summary>
        /// Adding the localization options as singleton object 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="defaultCulture">The default culture value as ISO three letters.</param>
        /// <param name="cultureRouteKey">The key that will be used in route convention to identify the culture value.</param>
        public static void AddLocalizationOptions(this IServiceCollection services, string cultureRouteKey, string defaultCulture = ArabicCulture)
        {
            CultureRouteKey = cultureRouteKey;

            var options = new RequestLocalizationOptions()
            {
                DefaultRequestCulture = new RequestCulture(defaultCulture),
                SupportedCultures = SupportedCultures,
                SupportedUICultures = SupportedCultures
            };
            options.RequestCultureProviders.Insert(0,
                new RouteDataRequestCultureProvider()
                {
                    RouteDataStringKey = cultureRouteKey,
                    UIRouteDataStringKey = cultureRouteKey,
                    Options = options
                }
            );

            services.AddSingleton(options);
        }
    }
}
