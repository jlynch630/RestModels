---
uid: getting-started
---

# Getting Started with RestModels and Entity Framework Core

In this getting started example, we'll be building a simple API for a blog server. When you're finished, you'll be able to create, read, update, and delete blogs and blog posts with simple key-based authentication.

## Terminology
Some of the terms used in this article can mean a lot of different things in a lot of different places, so here's a quick breakdown of what they mean in this article:

* **API:** Every call to some form of `app.UseRestModels` is an "API," a set of routes bundled together for working with a specific model.
* **Model:** The data class that the API operates on. In Entity Framework, this is also referred to as an Entity.
* **Route:** A bundle of all the options for a request with a single associated route pattern {todo fix - im having a hard time explaining this}
* **Route Pattern:** The string pattern that determines whether an HTTP request path matches a route

## Prerequisites
* [Visual Studio with .NET Core](https://visualstudio.microsoft.com/) (Windows) or [Visual Studio Code](https://code.visualstudio.com/) and the [.NET Core SDK](https://www.microsoft.com/net/download/core) (Windows, Mac, Linux)
* A good knowledge of ASP.NET Core and Entity Framework is recommended, but not required.

## Creating a new Project
Step one is creating a new ASP.NET Core project in Visual Studio, or on the command line.

### [Visual Studio 2019](#tab/vs)
* From the Start Page, click on **Create a new project**
* Click on or search for **ASP.NET Core Web Application**
* Enter a suitable project name, like **RMGettingStarted**
* Click **Create**

### [Command Line](#tab/cli)
Create a new folder for your project, and a new web application with `dotnet`:
```bash
~$ mkdir BlogServer && cd BlogServer
~/BlogServer$ dotnet new web
```
***

## Installing Dependencies
Next, install the `RestModels.EntityFrameworkCore` nuget package, as well as the Entity Framework package for your database. In this example, we'll be using `Sqlite`.

### [Visual Studio 2019](#tab/vs)
* From the toolbar, select **Tools** > **NuGet Package Manager** > **Package Manager Console**
* Enter the following commands in the Package Manager Console:
```powershell
Install-Package Microsoft.EntityFrameworkCore.Sqlite
Install-Package RestModels.EntityFrameworkCore
```
### [Command Line](#tab/cli)
```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package RestModels.EntityFrameworkCore
```
***

Once you've installed the package, go ahead and build the project (`Shift+F6` or `dotnet build`) to make sure everything's set up properly.

## Creating A Model
Next, we'll create the models for our blog server, based on the classes in EF Core's [getting started guide](https://docs.microsoft.com/en-us/ef/core/get-started/).

### [Blog.cs](#tab/cs)
```csharp
using System.Collections.Generic;
namespace BlogServer {
    public class Blog {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; } = new List<Post>();
    }

    public class Post {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
```
***

## Setting up EF Core
Set up Entity Framework with a `BlogServerContext` as you normally would. Here's the context this guide will use:

### [BlogServerContext.cs](#tab/cs)
```csharp
using Microsoft.EntityFrameworkCore;
namespace BlogServer {
    public class BlogServerContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=blogging.db");
    }
}
```
***

Register the DbContext Service in the `ConfigureServices` method of `Startup.cs`:
```csharp
services.AddDbContext<BlogServerContext>(); // { todo -- better error when missing }
```

Then, create a migration and the database:

### [Visual Studio 2019](#tab/vs)
In the Package Manager Console, run the following:
```powershell
Install-Package Microsoft.EntityFrameworkCore.Tools
Add-Migration InitialCreate
Update-Database
```

For more details on what these commands are doing, refer to EF Core's [getting started guide](https://docs.microsoft.com/en-us/ef/core/get-started/?tabs=visual-studio#tabpanel_CeZOj-G++Q-4_visual-studio)
### [Command Line](#tab/cli)
```bash
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet ef migrations add InitialCreate
dotnet ef database update
```

For more details on what these commands are doing, refer to EF Core's [getting started guide](https://docs.microsoft.com/en-us/ef/core/get-started/?tabs=netcore-cli#tabpanel_CeZOj-G++Q-4_netcore-cli)
***

## Building the `Blog` API
Now that we have our model and EF Core set up, we can start building our Blog API.

In  `Startup.cs`, add the following code to the end of the `Configure` method:
```csharp
string AdminKey = "mysecretkey";
app.UseEntityFrameworkRestModels<Blog, BlogServerContext>("blog", options => {
    options
        .ParseJson() // parse JSON for request bodies
        .WriteJson() // output JSON responses
        .Omit(m => m.Posts) // don't include posts in the response JSON
        .CatchExceptions() // show error messages 
        .GetByPrimaryKey() // GET /blog/{id}
        .AuthQuery("key", AdminKey) // require authentication for all future routes
        .PostCreate() // POST /blog
        .PutUpdateByPrimaryKey() // PUT /blog/{id}
        .DeleteByPrimaryKey(); // DELETE /blog/{id}
});
```

> [!IMPORTANT]
> Make sure you add `using RestModels.Extensions;` and `using RestModels.EntityFrameworkCore.Extensions;` to the top of the file, otherwise the build will fail.

This defines an API on the `/blog` route, which can create, read, update, and delete Blogs. Notice that child routes inherit the properties of their parent routes by default. For example, all of the `GET`/`POST`/`PUT`/`DELETE` requests will omit the `Posts` property from their response because the `Omit` method was called on the base route. 
{{Sometimes, however, a method like `GetByPrimaryKey` will clear all body parsers for that route, because it doesn't make sense to parse a body for a `GET` request. -- todo: article w/ more in-depth exploration of this topic, maybe delete this confusing sentence entirely}}

> [!Note]
> Order is important when creating APIs. In the API above, the `AuthQuery` method only applies to the create, update, and delete requests following it. The `GET` request before it has no authentication.

> [!IMPORTANT]
> **Never** hardcode an API key in code. `AdminKey` is only used in this guide for simplicity.

## Running the Project
At this point, run the project to better understand what you've built so far. To run the app, hit the play button in Visual Studio, or run `dotnet run` in the command line. While you can try to explore the `GET` request in the browser, you'll need a tool like [Insomnia](https://insomnia.rest/) or [Postman](https://www.postman.com/) to test the `POST`, `PUT`, and `DELETE` requests.

> [!NOTE]
> You'll likely immediately notice that visting just `/blog` in your web browser returns a `405 Method Not Allowed` response. That's because no routes matching it have a `GET` request method associated with them. You can add a request method manually with <xref:RestModels.Options.Builder.RestModelOptionsBuilder`2.AddRequestMethod(System.String)>, but it's generally recommended to use the `options.SetupMETHOD` and `options.METHOD` methods instead like we did here. For more information on routing, see <xref:routing>.

### Experimenting with Blogs
#### Create A Blog
Lets try to create a new Blog. Use the port given in the browser or console output. Make sure to use authentication!

**Request**
```rest
POST https://localhost:5001/blog?key=mysecretkey
```

```json
{
    "Url": "http://github.blog/"
}
```

**Response**
```json
{
    "PostId": 1,
    "Url": "http://github.blog/"
}
```

> [!TIP]
> Since we set the API to `WriteJson()`, every request will return a JSON representation of the affected models. Try replacing that line with `WriteXml()` or `WriteNumberAffected()` instead.

#### Reading a Blog
Great! We've just created a new Blog. Let's retrieve it using the `PostId` returned earlier. We don't need any auth here.

**Request**
```rest
GET https://localhost:5001/blog/1
```

**Response**
```json
{
    "PostId": 1,
    "Url": "http://github.blog/"
}
```

#### Updating a Blog
Uh-oh! We accidentally created the Blog with an insecure, `http` URL! Let's fix that:

**Request**
```rest
PUT https://localhost:5001/blog/1?key=mysecretkey
```

```json
{
    "Url": "https://github.blog/"
}
```

**Response**
```json
{
    "PostId": 1,
    "Url": "https://github.blog/"
}
```

While we're at it, we should probably fix the API to disallow insecure blog urls. Lets do that now by adding options to the `PUT` request:
```csharp
.PostCreate(options => {
    options.RequireAllInput(m => m.Url.StartsWith("https://")); //todo: custom error messages
})
```

Run the program again and see what happens if you try to create an insecure Blog.

> [!NOTE]
> Although it is generally recommended that `PUT` requests be idempotent, RestModels update operations by default allow partial updates. That is, you don't need to provide all the properties of a model in the request body for an update to be successful. If you would like to instead require all properties be provided, try replacing the update route above with the following: 
> ```csharp
> .PutUpdateByPrimaryKey(update => {
>   update
>       .RequireAllProperties() // require every settable property
>       .IgnorePrimaryKey(); // except for the primary key, provided in path
> })

#### Deleting a blog
Finally, lets get rid of the Blog we created:

**Request**
```rest
DELETE https://localhost:5001/blog/1?key=mysecretkey
```

**Response**
```json
{
    "PostId": 1,
    "Url": "https://github.blog/"
}
```

By default, the deleted model is returned.

## Checkpoint
You've successfully built your first API with RestModels! If you think you've got the hang of things, and just want to explore, look at <xref:request-flow>. Otherwise, if you want a little more guidance, we still have to build an API for `Posts`. Let's do that now.

## Building the `Post` API
Let's take this next one step by step.

First, we'll define an API in with it's root at `/blog/{id}/posts`, and we'll use JSON again to parse and return results.

### [Startup.cs](#tab/cs)
```csharp
app.UseEntityFrameworkRestModels<Post, BlogServerContext>("blog/{BlogId:int}/posts/", options => {
    options
        .ParseJson()
        .WriteJson()
});
```
***

RestModels uses the routing engine from ASP.NET Core, so you can use the same [templates](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-3.1#route-template-reference).

Next, we'll set up the same CRUD operations from the `Blog` API. Since we're still developing, we'll leave out `CatchExceptions` so we can use the default ASP.NET Core exception handler.

```csharp
app.UseEntityFrameworkRestModels<Post, BlogServerContext>("blog/{BlogId:int}/posts/", options => {
    options
        .GetByPrimaryKey()
        .AuthQuery("key", AdminKey)
        .PostCreate()
        .PutUpdateByPrimaryKey()
        .DeleteByPrimaryKey();
});
```
> [!NOTE]
> While slashes in the middle of a route pattern are respected, slashes at the beginning and end of route patterns are ignored. That is, `blog`, `/blog`, and `/blog/` are all treated the same way, so feel free to use whatever preference you prefer.

Since we only want these routes to modify posts from the Blog of the given BlogId route value, we'll add a [Filter](xref:filtering).
```csharp
app.UseEntityFrameworkRestModels<Post, BlogServerContext>("blog/{BlogId:int}/posts/", options => {
    options
        .FilterByRouteEqual(p => p.BlogId, "BlogId")
        ...
});
```

> [!NOTE]
> While filters do operate over the entire dataset provided by EntityFramework, they still use LINQ-to-SQL to keep the API performant.

It doesn't make sense to filter posts when we're creating a new `Post`, so lets clear that filter in `PostCreate`:
```csharp
app.UseEntityFrameworkRestModels<Post, BlogServerContext>("blog/{BlogId:int}/posts/", options => {
    options
        ...
        .PostCreate(create => create.ClearFilters())
        ...
});
```

> [!TIP]
> Alternatively, you could move the call to `PostCreate` above the call to `FilterByRouteEqual` and the filter wouldn't be applied.

If you try to run it now, `POST` requests will fail because Entity Framework complains that a foreign key constraint isn't met. Well, we can add that foreign key constraint easily with `SetValueRoute`, which will set a value for the parsed model before it's created based on that `BlogId` route value.
```csharp
app.UseEntityFrameworkRestModels<Post, BlogServerContext>("blog/{BlogId:int}/posts/", options => {
    options
        ...
        .PostCreate(create => {
            create
                .ClearFilters()
                .SetValueRoute(p => p.BlogId, "BlogId");
        })
        ...
});
```

> [!TIP]
> RestModels can infer the name of the route value from the name of the property you want to set. Try removing `"BlogId"` from the call to `SetValueRoute`.


We'll also want to add a way to `GET` all of the posts on a Blog. We can add that functionality on the base route with a simple call.
```csharp
app.UseEntityFrameworkRestModels<Post, BlogServerContext>("blog/{BlogId:int}/posts/", options => {
    options
        ...
        .Get();
});
```

If you're unauthenticated, however, let's make the API return a random blog post. Each of the methods that creates a new route accepts both a route pattern to append to the existing one and an options handler to add additional options. We'll use those here to create a route at `blog/{id}/posts/random`
```csharp
app.UseEntityFrameworkRestModels<Post, BlogServerContext>("blog/{BlogId:int}/posts/", options => {
    options
        ...
        .SetupAnonymousGet("random", random => {
            random.Filter(set => set.Skip(new Random().Next(0, set.Count())))
                .LimitOne();
        });
});
```

> [!TIP]
> The `Blog` property will always be null on returned posts because of how Entity Framework works. Use <xref:RestModels.Options.Builder.RestModelOptionsBuilder`2.Omit(System.Linq.Expressions.Expression{System.Func{`0,System.Object}})> to hide it.

## Wrapping Up
Run the project again and play around with creating blogs, creating, updating, deleting posts in a blog, getting all posts, and getting a random post.

Congratulations! You've built two APIs with RestModels in less than 30 lines of code! These getting started guides provide a small taste of what RestModels can do. 

If you're still eager to learn more, check out the following resources:

* [The Request Flow](xref:request-flow). A closer look at the architecture of RestModels.
* [More Examples](xref:api-examples). RestModels hides a lot of power in a small library. The best way to make full use of it is by exploring examples.