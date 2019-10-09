using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.RequestLocalizationPipeline
{
    public static class CulturesHelper
    {
        private static string Arabic_Culture = "ar-SY";

        public static string Culture_Route_Key = "culture";
        public static string DefaultCultureString = Arabic_Culture;
        public static CultureInfo DefaultCulture = new CultureInfo(Arabic_Culture);


        public static List<CultureInfo> SupportedCultures
        {
            get
            {
                var arabicCulture = new CultureInfo(Arabic_Culture);
                arabicCulture.DateTimeFormat.Calendar = new GregorianCalendar();

                return new List<CultureInfo>
                {
                    arabicCulture,
                    new CultureInfo("en-US")
                };
            }
        }

        public static void AddLocalizationOptions(this IServiceCollection services)
        {
            var options = new RequestLocalizationOptions()
            {
                DefaultRequestCulture = new RequestCulture(CulturesHelper.DefaultCulture),
                SupportedCultures = CulturesHelper.SupportedCultures,
                SupportedUICultures = CulturesHelper.SupportedCultures
            };
            options.RequestCultureProviders.Insert(0,
                new RouteDataRequestCultureProvider()
                {
                    RouteDataStringKey = CulturesHelper.Culture_Route_Key,
                    UIRouteDataStringKey = CulturesHelper.Culture_Route_Key,
                    Options = options
                }
            );

            services.AddSingleton(options);
        }
    }
}
