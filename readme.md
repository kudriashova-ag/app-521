### Pagination

1. Helpers\Pagination\PagedResult.cs
2. Helpers\Pagination\PaginationHelper.cs
3. Helpers\Queryable\MovieQueryableExtension.cs
4. Services\MovieService.cs
5. Services\IMovieService.cs
6. Controllers\MovieController.cs


/* app.Use(async (context, next) =>
{
    // ?token=32435345   - next()
    // if (!context.Request.Query.ContainsKey("token"))
    // {
    //     context.Response.StatusCode = StatusCodes.Status403Forbidden;
    //     await context.Response.WriteAsync("Forbidden");
    //     return;
    // }
    Console.WriteLine("1 - вхід");
    await next(context);
    Console.WriteLine("1 - вихід");
});

app.Use(async (context, next) =>
{
    Console.WriteLine("2 - вхід");
    await next(context);
    Console.WriteLine("2 - вихід");
}); */

// окрема гілка за шляхом, назад не повертається
// app.Map("/api/v1/clients", clientsApp =>
// {
//     clientsApp.Run(async (context) =>
//     {
//         Console.WriteLine("clientsApp - стоп");
//     });
// });
// окрема гілка за умовою, назад не повертається
// app.MapWhen(context => context.Request.Query.ContainsKey("debug"),

//     clientsApp =>
//     {
//         clientsApp.Run(async (context) =>
//         {
//             Console.WriteLine("Режим налагодження");
//         });
//     }
// );


// умовне додавання middleware, назад  повертається в основний конвейер
// app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/v1/clients"),

//     clientsApp =>
//     {
//         clientsApp.Use(async (context, next) =>
//         {
//             Console.WriteLine("Режим налагодження");
//             await next();
//         });
//     }
// );

