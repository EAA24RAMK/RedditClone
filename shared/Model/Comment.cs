namespace Model
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int PostId { get; set; } // Foreign key
        public Post Post { get; set; } = null!;
        public string Text { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int Votes { get; set; } = 0;
    }
}