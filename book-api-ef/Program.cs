using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Json;

using Data;
using Service;

var builder = WebApplication.CreateBuilder(args);

// Sætter CORS så API'en kan bruges fra andre domæner
var AllowSomeStuff = "_AllowSomeStuff";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowSomeStuff, builder => {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Tilføj DbContext factory som service.
builder.Services.AddDbContext<RedditContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ContextSQLite")));

// Tilføj DataService så den kan bruges i endpoints
builder.Services.AddScoped<DataService>();

// Dette kode kan bruges til at fjerne "cykler" i JSON objekterne.
/*
builder.Services.Configure<JsonOptions>(options =>
{
    // Her kan man fjerne fejl der opstår, når man returnerer JSON med objekter,
    // der refererer til hinanden i en cykel.
    // (altså dobbelrettede associeringer)
    options.SerializerOptions.ReferenceHandler = 
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});
*/

var app = builder.Build();

// Seed data hvis nødvendigt.
using (var scope = app.Services.CreateScope())
{
    var dataService = scope.ServiceProvider.GetRequiredService<DataService>();
    dataService.SeedData(); // Fylder data på, hvis databasen er tom. Ellers ikke.
}

app.UseHttpsRedirection();
app.UseCors(AllowSomeStuff);

// Middlware der kører før hver request. Sætter ContentType for alle responses til "JSON".
app.Use(async (context, next) =>
{
    context.Response.ContentType = "application/json; charset=utf-8";
    await next(context);
});


// DataService fås via "Dependency Injection" (DI)
// GET endpoints
app.MapGet("/", (DataService service) =>
{
    return new { message = "Hello World!" };
});

app.MapGet("/api/posts", (DataService service) =>
{
    var posts = service.GetPosts()
        .Select(p => new 
        { 
            p.PostId, 
            p.Title, 
            p.Url, 
            p.Text, 
            p.AuthorName, 
            p.CreatedAt, 
            p.Votes,
            commentsCount = p.Comments.Count
        });

    return Results.Ok(posts);
});

app.MapGet("/api/posts/{id}", (DataService service, int id) =>
{
    var post = service.GetPost(id);
    if (post == null) return Results.NotFound(new { message = "Posten findes ikke." });

    return Results.Ok(new
    {
        post.PostId,
        post.Title,
        post.Url,
        post.Text,
        post.AuthorName,
        post.CreatedAt,
        post.Votes,
        commentsCount = post.Comments.Select(c => new
        {
            c.CommentId,
            c.Text,
            c.AuthorName,
            c.CreatedAt,
            c.Votes
        })
    });
});

// POST endpoints
app.MapPost("/api/posts", (DataService service, NewPostData data) =>
{
    var post = service.CreatePost(data.Title, data.Text, data.Url, data.AuthorName);
    return Results.Created($"/api/posts/{post.PostId}", post);
});

app.MapPost("/api/posts/{id}/comments", (DataService service, int id, NewCommentData data) =>
{
    var comment = service.CreateComment(id, data.Text, data.AuthorName);
    if (comment == null) return Results.NotFound(new { message = "Posten findes ikke." });
    
    return Results.Created($"/api/posts/{id}/comments/{comment.CommentId}", comment);
});

// PUT endpoints
app.MapPut("/api/posts/{id}/upvote", (DataService service, int id) =>
{
    return service.UpvotePost(id)
        ? Results.Ok()
        : Results.NotFound(new { message = "Posten findes ikke." });
});

app.MapPut("/api/posts/{id}/downvote", (DataService service, int id) =>
{
    return service.DownvotePost(id)
        ? Results.Ok()
        : Results.NotFound(new { message = "Posten findes ikke." });
});

app.MapPut("/api/posts/{postid}/comments/{commentid}/upvote", (DataService service, int postid, int commentid) =>
{
    return service.UpvoteComment(postid, commentid)
        ? Results.Ok()
        : Results.NotFound(new { message = "Posten findes ikke." });
});

app.MapPut("/api/posts/{postid}/comments/{commentid}/downvote", (DataService service, int postid, int commentid) =>
{
    return service.DownvoteComment(postid, commentid)
        ? Results.Ok()
        : Results.NotFound(new { message = "Posten findes ikke." });   
});

app.Run();

// Record datamodeller til post metoder
record NewPostData(string Title, string? Text, string? Url, string AuthorName);
record NewCommentData(string Text, string AuthorName);