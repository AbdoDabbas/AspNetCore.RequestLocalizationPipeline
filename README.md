# AspNetCore.RequestLocalizationPipeline
A helper project to add route localization in .NET Core Web API projects.

You can use it by adding it as a separate project and reference it in the Web.API project you have, or as a nuget package:  
```
Install-Package AspNetCore.RequestLocalizationPipeline
```

# How to use it:  
There're couple of snippets you need to add to your `startup.cs` before you'll be able to use this package.  

1. Specify the route key you want to use for culture:  
```csharp
const string langKey = "lang";
```

2. Define a route convention as a `readonly` field in startup:  
```csharp
readonly string ROUTE_CONVENTION = "{" + langKey + "}/api";
```  
Adding a route convention to your controllers, where a convention is a prefix that will be prepended to all the routes in your project.  

3. In method `ConfigureServices` we have to prepare the localization options and add it as a singleton to be used later:  
```csharp
public void ConfigureServices(IServiceCollection services)
{
	// An extension method from Microsoft.Extensions.DependencyInjection to add required services for localization.
	services.AddLocalization(options => options.ResourcesPath = "Resources");

	// An extension method from the package to prepare the options and add the object as Singleton in DI.
	// First parameter is the same route key you defined in step 1.
	// Second parameter is the default culture you want your api to use in case no culture passed through URL.
	services.AddLocalizationOptions(langKey);
}
```  
Note: `AddLocalizationOptions` will add the `RouteDataRequestCultureProvider` in index 0 of the list `RequestCultureProviders`.  

4. Now we will add the localization pipeline as a global action filter in your project:  
```csharp
services.AddMvc(opts =>
    {
		...
		// Make sure it's set before any other filters ,if you have, if you want to use the culture in them.
        opts.AddLocalizationPipeLine(ROUTE_CONVENTION);
		...
    })
```

5. In `Configure` method, we'll add a rewriter to make sure we accept requests that don't have any culutre in the position of the specified culture key:  
```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
	app.UseDefaultCultureRewrite(ROUTE_CONVENTION);
}
```

6. In `Configure` method, we'll add a rewriter to make sure we accept cultures in two letters (not only ISO 3 letters):  
```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
	app.UseDefaultCultureRewrite(ROUTE_CONVENTION)
		.UseTwoLettersCultureRewrite(ROUTE_CONVENTION);
}
```

#### Note:  
If you want to make sure if it's working, comment `app.UseMvc()` in method `Configure` and add this temporarly:  
```csharp
app.Run(context => context.Response.WriteAsync(
            $"Rewritten or Redirected Url: " +
            $"{context.Request.Path + context.Request.QueryString}"));
```

At the end, this is my first project on github, I may did something wrong in it, or it's not clean code, please advice and feel free to contribute.