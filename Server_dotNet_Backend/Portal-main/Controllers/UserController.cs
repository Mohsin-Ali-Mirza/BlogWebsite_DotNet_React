namespace Portal.Controllers;

// This controller will manage all user related endpoints and they will all begin with "/User"
[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly PortalContext db;
    private readonly IConfiguration configuration;
    
    // Constructor to initialize the controller with the database context and configuration file
    public UserController(PortalContext db, IConfiguration configuration)
    {
        this.db = db;
        this.configuration = configuration;
    }

    [HttpPost("Register")]
    public async Task<ActionResult<MsgResponse>> Register([FromBody] RegisterUserDTO NewUser)
    {
        bool emailExists = await db.Users.AnyAsync(u => u.Email == NewUser.Email);
        if (emailExists)
        {
            return Conflict(new MsgResponse{ Message = "Email already in use." });
        }
        
        string PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewUser.Password);
        
        User u1 = new User
        {
            Name = NewUser.Name,
            Email = NewUser.Email,
            Password = PasswordHash,
            Role = NewUser.Role
        };
        
        // Add the user to the database and save changes
        db.Users.Add(u1);
        await db.SaveChangesAsync();
        
        return Ok(new MsgResponse{ Message = "User Successfully Registered." });
    }

    [HttpPost("Login")]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginUserDTO LoginUser)
    {
        // Search for a user in DB that has the given email
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == LoginUser.Email);

        // If such a user exists and their stored password(hash) matches the given password then assign them a token 
        if (user != null && BCrypt.Net.BCrypt.Verify(LoginUser.Password, user.Password))
        {
            string tokenvalue = CreateToken(user);
            return Ok(new TokenResponse{ Token = tokenvalue });
        }
        else
        {
            return Unauthorized(new MsgResponse{ Message = "Incorrect Email or Password." });
        }
    }

    private string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            configuration.GetSection("Token").Value!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
    
    // Retrieve all Teachers with their IDs for Dropdown options in Filtering Posts
    [HttpGet("GetTeachers"), Authorize]
    public async Task<ActionResult<List<GetTeacherDTO>>> GetAllTeachers()
    {
        var teachers = await db.Users
            .Where(user => user.Role == "teacher")
            .Select(user => new GetTeacherDTO()
            {
                Id = user.Id,
                Name = user.Name
            })
            .ToListAsync();

        return Ok(teachers);
    }
    
    // Show user's profile page
    [HttpGet("GetProfile"), Authorize]
    public async Task<ActionResult<GetUserDTO>> GetProfile()
    {
        var userIdFromToken = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        
        var user = await db.Users
            .Where(user => user.Id == userIdFromToken)
            .Select(user => new GetUserDTO()
            {
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
            })
            .FirstOrDefaultAsync();
        
        if (user == null)
        {
            return NotFound(new MsgResponse { Message = "User not found." });
        }
        
        return Ok(user);
    }
    
    // Update user's profile page
    [HttpPut("UpdateProfile"), Authorize]
    public async Task<ActionResult<MsgResponse>> UpdateProfile([FromBody] UpdateUserDTO updatedUser)
    {
        var userIdFromToken = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userIdFromToken);
        if (user == null)
        {
            return NotFound(new MsgResponse { Message = "User not found." });
        }
        
        // Update user properties
        user.Name = updatedUser.Name;
        user.Email = updatedUser.Email;
        user.Password = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);

        // Save changes to the database
        await db.SaveChangesAsync();

        return Ok(new MsgResponse { Message = "Profile updated successfully." });
    }

    //Delete User's Profile
    [HttpDelete("DeleteProfile"), Authorize]
    public async Task<ActionResult<MsgResponse>> DeleteProfile()
    {
        try
        {
            var userIdFromToken = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userIdFromToken);
            if (user == null)
            {
                return NotFound(new MsgResponse { Message = "User not found." });
            }
            
            // Handle attachments removal
            var attachments = await db.Attachments
                .Where(a => db.Posts.Any(p => p.UserId == userIdFromToken && p.Id == a.PostId))
                .ToListAsync();
            foreach (var attachment in attachments)
            {
                // Remove the file from Azure Blob Storage
                AttachmentHelper.RemoveFileOnAzure(attachment.Path, configuration.GetConnectionString("AzureBlobStorage")!);

                // Remove the attachment record from the database
                db.Attachments.Remove(attachment);
            }
            
            // Remove the user
            db.Users.Remove(user);
            
            // Save changes to the database
            await db.SaveChangesAsync();

            // Make sure the frontend app signs out this user
            return Ok(new MsgResponse { Message = "Profile, Posts and Attachments deleted successfully." });
        }
        catch (ArgumentException)
        {
            // If the blob doesn't exist
            return NotFound(new MsgResponse
            {
                Message = "The attachment file does not exist in storage, but a record for it exists in the database."
            });
        }
        catch (Exception ex)
        {
            // Handle any exceptions that may occur during the deletion process (e.g., DB errors)
            return StatusCode(500, new MsgResponse { Message = ex.Message });
        }
    }
}