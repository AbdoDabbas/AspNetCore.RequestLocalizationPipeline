# AspNetCore.RequestLocalizationPipeline
A helper project to add route localization in .NET Core Web API projects.

You can use it by adding it as a separate project and reference it in the Web.API project you have, or as a nuget package:  
```
Install-Package AspNetCore.RequestLocalizationPipeline
```

# How to use it:  
There're couple of snippets you need to add to your `startup.cs` before you'll be able to use this package.  

This project depends on adding a route convention to your controllers, a convention is a prefix that will be prepended to all the routes in your project.  

1. define a route convention as a `readonly` field in startup:  
```
readonly string ROUTE_CONVENTION = "{" + CulturesHelper.Culture_Route_Key + "}/api";
```  
The class `CulturesHelper` contains the supported languages and the culture route key that'll be used to get the provided culture through 
the url, you can change it in the `Startup` constructor before you start 