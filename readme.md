
# DbNetSuiteCore

**DbNetSuiteCore** is a set of ASP.Net Core application UI development components for Razor pages, MVC and Blazor Server and are designed to enable the rapid development of data driven web applications. **DbNetSuiteCore** currently supports the following data sources MSSQL, MySQL, MariaDB, PostgreSQL, Oracle, MongoDB and SQLite databases along with JSON (files and API), CSV and Excel files and the file system itself.

Simply add DbNetSuiteCore to your pipeline as follows:
```c#
{
    using DbNetSuiteCore.Middleware;                    // <= Add this line

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddDbNetSuiteCore();               // <= Add this line

    builder.Services.AddRazorPages();

    var app = builder.Build();

    app.UseDbNetSuiteCore();                            // <= Add this line

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthorization();
    app.MapRazorPages();
    app.Run();
}
```
You can then add a component to your Razor page or MVC view  as follows:
```razor
@page
@using DbNetSuiteCore.Enums
@using DbNetSuiteCore.Models

<!DOCTYPE html>
<html lang="en">
<head>
    @DbNetSuiteCore.Resources.StyleSheet() @* Add the stylesheet *@
</head>
<body>
    <main>
@{
    GridModel customerGrid = new GridModel(DataSourceType.SQLite, "Northwind", "Customers");
    @(await DbNetSuiteCore.Control.Create(HttpContext).Render(customerGrid))
}
    </main>
    @DbNetSuiteCore.Resources.ClientScript() @* Add the client-side library *@
</body>
</html>
```

```razor

MVC view should use Context instead of HttpContext
...
@{
    GridModel customerGrid = new GridModel(DataSourceType.SQLite, "Northwind", "Customers");
    @(await DbNetSuiteCore.Control.Create(Context).Render(customerGrid))
}
...
```

For demos [click here](https://dbnetsuitecore.com/) and for the documentation [click here](https://github.com/dbnetlink/DbNetSuiteCore2/wiki) 