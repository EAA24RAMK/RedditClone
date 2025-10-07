using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Model;

namespace RedditAPP.Data;

public class ApiService
{
    private readonly HttpClient http;
    private readonly IConfiguration configuration;
    private readonly string baseAPI = "";

    public ApiService(HttpClient http, IConfiguration configuration)
    {
        this.http = http;
        this.configuration = configuration;
        this.baseAPI = configuration["base_api"];
    }

    // Posts
    public async Task<List<Post>> GetPosts()
    {
        return await http.GetFromJsonAsync<List<Post>>($"{baseAPI}posts") 
               ?? new List<Post>();
    }

    public async Task<Post?> GetPost(int id)
    {
        return await http.GetFromJsonAsync<Post>($"{baseAPI}posts/{id}");
    }

    public async Task<Post?> CreatePost(string title, string? text, string? url, string authorName)
    {
        var payload = new
        {
            title,
            text,
            url,
            authorName
        };
        
        var response = await http.PostAsJsonAsync($"{baseAPI}posts", payload);

        if (!response.IsSuccessStatusCode) return null;
        
        return await response.Content.ReadFromJsonAsync<Post>();
    }
    
    // Comments
    public async Task<Comment?> CreateComment(int postId, string text, string authorName)
    {
        var payload = new
        {
            text,
            authorName
        };
        
        var response = await http.PostAsJsonAsync($"{baseAPI}posts/{postId}/comments", payload);
        
        if (!response.IsSuccessStatusCode) return null;
        
        return await response.Content.ReadFromJsonAsync<Comment>();
    }

    // Votes
    public async Task<bool> UpvotePost(int postId)
    {
        var response = await http.PutAsync($"{baseAPI}posts/{postId}/upvote/", null);
        return response.IsSuccessStatusCode;
    }
    
    public async Task<bool> DownvotePost(int postId)
    {
        var response = await http.PutAsync($"{baseAPI}posts/{postId}/downvote/", null);
        return response.IsSuccessStatusCode;   
    }
    
    public async Task<bool> UpvoteComment(int postId, int commentId)
    {
        var response = await http.PutAsync($"{baseAPI}posts/{postId}/comments/{commentId}/upvote/", null);
        return response.IsSuccessStatusCode;
    }
    
    public async Task<bool> DownvoteComment(int postId, int commentId)
    {
        var response = await http.PutAsync($"{baseAPI}posts/{postId}/comments/{commentId}/downvote/", null);
        return response.IsSuccessStatusCode;  
    }
}
