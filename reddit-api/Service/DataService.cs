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
                Title = "Velkommen til vores Reddit!",
                Text = "Det her er den første post",
                AuthorName = "Admin",
                CreatedAt = DateTime.UtcNow,
                Votes = 3
            };

            var post2 = new Post
            {
                Title = "Tjek denne side ud!!!!",
                Url = "https://dotnet.microsoft.com",
                AuthorName = "Rasmus",
                CreatedAt = DateTime.UtcNow,
                Votes = 5
            };
            
            db.Posts.AddRange(post1, post2);
            db.SaveChanges();
            
            // Tilføjer nogle kommentarer
            db.Comments.Add(new Comment
            {
                PostId = post1.PostId,
                Text = "Godt gået!",
                AuthorName = "Kasper",
                CreatedAt = DateTime.UtcNow,
                Votes = 2
            });

            db.Comments.Add(new Comment
            {
                PostId = post1.PostId,
                Text = "Ser spændende ud",
                AuthorName = "Alfred",
                CreatedAt = DateTime.UtcNow,
                Votes = 1
            });

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