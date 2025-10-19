using Microsoft.EntityFrameworkCore;
using System.Text.Json;

using Data;
using Model;

namespace Service;

public class DataService
{
    private RedditContext db { get; }

    public DataService(RedditContext db) 
    {
        this.db = db;
    }
    /// <summary>
    /// Seeder noget nyt data i databasen hvis det er nødvendigt.
    /// </summary>
    public void SeedData() 
    {
        // Tilføjer nogle posts
        if (!db.Posts.Any())
        {
            var post1 = new Post
            {
                Title = "Velkommen til det nye Reddit!",
                Text = "Det her er det første post",
                AuthorName = "Rasmus",
                CreatedAt = DateTime.UtcNow,
                Votes = 5
            };

            var post2 = new Post
            {
                Title = "Tjek denne side ud!!!!",
                Url = "https://dotnet.microsoft.com",
                AuthorName = "Alfred",
                CreatedAt = DateTime.UtcNow.AddMinutes(-5),
                Votes = 7
            };
            
            var post3 = new Post
            {
                Title = "Tjek Blazor ud!!!!",
                Text = "Blazor er bare fedt!",
                AuthorName = "Kasper",
                CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                Votes = 3
            };
            
            var post4 = new Post
            {
                Title = "Har lige lært EF / Enity Framework, wow en oplevelse",
                Text = "En meget nem måde at arbejde med databaser i .NET",
                AuthorName = "Elias",
                CreatedAt = DateTime.UtcNow.AddMinutes(-20),
                Votes = 6
            };
            
            var post5 = new Post
            {
                Title = "Det her Reddit er meget bedre end Facebook",
                Url = "https://facebook.com",
                AuthorName = "Lars",
                CreatedAt = DateTime.UtcNow.AddMinutes(-30),
                Votes = 10
            };
            
            var post6 = new Post
            {
                Title = "Har i set det rigtige Reddit? Synes de er lidt foran på design delen",
                Url = "https://reddit.com",
                AuthorName = "Rasmus",
                CreatedAt = DateTime.UtcNow.AddMinutes(-45),
                Votes = 7
            };
            
            var post7 = new Post
            {
                Title = "C# er top lækkert",
                Text = "Mit ynglingssprog!",
                AuthorName = "Dorthe",
                CreatedAt = DateTime.UtcNow.AddMinutes(-60),
                Votes = 7
            };
            
            var post8 = new Post
            {
                Title = "Har i prøvet at oprette et post her på?",
                Text = "Jeg synes det er super nemt",
                AuthorName = "John",
                CreatedAt = DateTime.UtcNow.AddMinutes(-75),
                Votes = 5
            };
            
            var post9 = new Post
            {
                Title = "Der er kommet en ny opdatering til Visual Studio",
                Text = "Synes det er dårligt",
                AuthorName = "Preben",
                CreatedAt = DateTime.UtcNow.AddMinutes(-90),
                Votes = 1
            };
            
            var post10 = new Post
            {
                Title = "Tak for modtagelsen af vores Reddit",
                Text = "Vi takker endnu en gang",
                AuthorName = "Rasmus",
                CreatedAt = DateTime.UtcNow.AddMinutes(-120),
                Votes = 5
            };
            
            db.Posts.AddRange(post1, post2, post3, post4, post5, post6, post7, post8, post9, post10);
            db.SaveChanges();
            
            // Tilføjer to kommentarer til hvert post
            void AddComments(Post post, string author1, string text1, string author2, string text2)
            {
                db.Comments.Add(new Comment
                {
                    PostId = post.PostId,
                    Text = text1,
                    AuthorName = author1,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-2),
                    Votes = 2
                });

                db.Comments.Add(new Comment
                {
                    PostId = post.PostId,
                    Text = text2,
                    AuthorName = author2,
                    CreatedAt = DateTime.UtcNow.AddMinutes(-1),
                    Votes = 1
                });
            }
            
            AddComments(post1, "Kasper", "Godt gået!", "John", "Ser spændende ud");
            AddComments(post2, "Alfred", "Tak for linket", "John", "En god side fra Microsoft");
            AddComments(post3, "Kim", "Har også brugt meget Blazor", "Jimmy", "Anbefaler ikke Blazor");
            AddComments(post4, "Alfred", "Et super værktøj", "John", "Vil jeg også anbefale");
            AddComments(post5, "John", "Er også godt træt af Facebook", "Iben", "Denne side er meget bedre.");
            AddComments(post6, "Albert", "Nej, synes jeres UI er bedre", "Lars", "Det her er nemmere at forholde sig til");
            AddComments(post7, "Karsten", "Ja det er super", "Jim", "Super lækkert programmeringssprog");
            AddComments(post8, "Anders", "Giver dig ret, det fungerer bare", "Kim", "Er ikke så teknisk");
            AddComments(post9, "John", "Har ikke opdateret endnu", "Vincent", "Jeg er fan");
            AddComments(post10, "John", "Selv tak", "Kasper", "Det er os der takker");

            db.SaveChanges();
        }
    }

    // Hent alle post og enkelte posts med kommentarer
    public List<Post> GetPosts() 
    {
        return db.Posts
            .Include(p => p.Comments) // Henter kommentarerne fra post
            .OrderByDescending(p => p.CreatedAt)
            .Take(50) // Tager 50 til forsiden, som opgaven beskriver
            .ToList();
    }

    public Post? GetPost(int id) {
        return db.Posts
            .Include(p => p.Comments) // Henter post pr id med kommentarer
            .FirstOrDefault(p => p.PostId == id);
    }

    // Opret post og kommentarer
    public Post CreatePost(string title, string? text, string? url, string authorName)
    {
        var post = new Post
        {
            Title = title,
            Text = text,
            Url = url,
            AuthorName = authorName,
            CreatedAt = DateTime.UtcNow
        };

        db.Posts.Add(post);
        db.SaveChanges();
        return post;
    }

    public Comment? CreateComment(int postId, string text, string authorName)
    {
        var post = db.Posts.FirstOrDefault(p => p.PostId == postId);
        if (post == null) return null;
        var comment = new Comment
        {
            PostId = postId,
            Text = text,
            AuthorName = authorName,
            CreatedAt = DateTime.UtcNow
        };
        
        db.Comments.Add(comment);
        db.SaveChanges();
        return comment;
    }
    
    // Giv votes
    public bool UpvotePost(int id)
    {
        var post = db.Posts.FirstOrDefault(p => p.PostId == id);
        if (post == null) return false; // Posten findes ikke (ikke muligt at upvote)
        post.Votes++; // Plusser 1 vote til nuværende antal votes
        db.SaveChanges();
        return true;
    }

    // Samme metode, men minuser votes
    public bool DownvotePost(int id)
    {
        var post = db.Posts.FirstOrDefault(p => p.PostId == id);
        if (post == null) return false;
        post.Votes--;
        db.SaveChanges();
        return true;
    }
    
    public bool UpvoteComment(int postId, int commentId)
    {
        var comment = db.Comments.FirstOrDefault(c => c.PostId == postId && c.CommentId == commentId); // Finder post og kommentar pr id
        if (comment == null) return false;
        comment.Votes++;
        db.SaveChanges();
        return true;
    }
    
    // Samme metode, men minuser votes på kommentar
    public bool DownvoteComment(int postId, int commentId)
    {
        var comment = db.Comments.FirstOrDefault(c => c.PostId == postId && c.CommentId == commentId);
        if (comment == null) return false;
        comment.Votes--;
        db.SaveChanges();
        return true;
    }

}