namespace Portal.Models;

public class Post
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string Title { get; set; }
    
    [Required]
    public required string Content { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default to current UTC time

    //User to Post 1:N
    [Required]
    public int UserId { get; set; }
    public User User { get; set; }

    //Course to Post 1:N
    [Required]
    public int CourseId { get; set; }
    public Course Course { get; set; }

    //Post to Comment 1:N
    public List<Comment> Comments { get; set; }

    //Post to Attachments 1:N
    public List<Attachment> Attachments { get; set; }
}