namespace Portal.Models;

public class Attachment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string Type { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string Path { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default to current UTC time

    //Post to Attachments 1:N
    [Required]
    public int PostId { get; set; }
    public Post Post { get; set; }
}