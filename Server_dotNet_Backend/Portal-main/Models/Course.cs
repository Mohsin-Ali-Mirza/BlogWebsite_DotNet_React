namespace Portal.Models;

public class Course
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }

    //Course to Post 1:N
    public List<Post> Posts { get; set; }
}
