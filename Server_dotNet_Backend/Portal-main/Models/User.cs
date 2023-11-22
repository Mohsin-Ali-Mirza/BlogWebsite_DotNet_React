namespace Portal.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }
    
    [Required]
    [StringLength(255)]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string Password { get; set; }
    
    [Required]
    [StringLength(7)]
    [RegularExpression("^(teacher|student)$", ErrorMessage = "Role must be either 'student' or 'teacher'.")]
    public required string Role { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default to current UTC time

    //User to Post 1:N
    public List<Post> Posts { get; set; }

    //User to Comment 1:N
    public List<Comment> Comments { get; set; }
}