namespace Portal.DTOs;

//Used for create and update requests
public class SetPostDTO
{
    [Required]
    [StringLength(255)]
    public required string Title { get; set; }
    
    [Required]
    public required string Content { get; set; }
    
    [Required]
    public int CourseId { get; set; }
}

public class GetPostDTO
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string Title { get; set; }
    
    [Required]
    public required string Content { get; set; }
    
    //Join to get Teacher and Course Names
    [Required]
    public required string TeacherName { get; set; }
    
    [Required]
    public required string CourseName { get; set; }
    
    [Required]
    public required List<AttachmentDTO> Attachments { get; set; }
}

// Only to be used within GetPostDTO
public class AttachmentDTO
{
    public int Id { get; set; }
    public required string Type { get; set; }
    public required string Path { get; set; }
}

public class GetCourseDTO
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }
}

// Returned on post creation to be used for calling create attachment API
public class PostIDResponse
{
    public int PostId { get; set; }
}