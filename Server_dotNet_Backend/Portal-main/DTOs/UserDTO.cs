namespace Portal.DTOs;

public class RegisterUserDTO
{
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
}

public class LoginUserDTO
{
    [Required]
    [StringLength(255)]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string Password { get; set; }
}

public class TokenResponse
{
    public required string Token { get; set; }
    public string TokenType { get; set; } = "bearer";
}

//A Standard Message Response to be used when any entity data doesn't need to be returned
public class MsgResponse
{
    public required string Message { get; set; }
}

public class GetTeacherDTO
{
    [Required]
    public int Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }
}

public class GetUserDTO
{
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }

    [Required]
    [StringLength(255)]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [StringLength(7)]
    [RegularExpression("^(teacher|student)$", ErrorMessage = "Role must be either 'student' or 'teacher'.")]
    public required string Role { get; set; }
}

public class UpdateUserDTO
{
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
}