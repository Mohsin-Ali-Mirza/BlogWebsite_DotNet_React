namespace Portal.Models;

public class Comment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    //User to Comment 1:N
    [Required]
    public int UserId { get; set; }
    public User User { get; set; }

    //Post to Comment 1:N
    [Required]
    public int PostId { get; set; }
    public Post Post { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string Content { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default to current UTC time
}