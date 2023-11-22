namespace Portal.Controllers;

// This controller will manage all post related endpoints and they will all begin with "/Post"
[Route("[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly PortalContext db;
    private readonly IConfiguration configuration;

    // Constructor to initialize the controller with the database context and configuration file
    public PostController(PortalContext db, IConfiguration configuration)
    {
        this.db = db;
        this.configuration = configuration;
    }
    
    // Retrieve all Courses with their IDs for Dropdown options in Creating Post or Filtering Posts
    [HttpGet("GetCourses"), Authorize]
    public ActionResult<List<GetCourseDTO>> GetCourses()
    {
        var courses = db.Courses
            .Select(course => new GetCourseDTO
            {
                Id = course.Id,
                Name = course.Name
            })
            .ToList();

        return Ok(courses);
    }

    // Retrieve all Posts with filtering using Course Id or Teacher Name
    [HttpGet(""), Authorize]
    public async Task<ActionResult<List<GetPostDTO>>> AllPosts([FromQuery] int? courseId, [FromQuery] string? teacher)
    {
        // Check if the current user exists
        var userIdFromToken = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (!await db.Users.AnyAsync(u => u.Id == userIdFromToken))
        {
            return NotFound(new MsgResponse { Message = "User not found." });
        }

        var postQuery = db.Posts
            .Include(post => post.User)   // Include the User (teacher) related to the post
            .Include(post => post.Course) // Include the Course related to the post
            .Include(post => post.Attachments) // Include the Attachments related to the post
            .OrderByDescending(post => post.CreatedAt)
            .AsQueryable(); 

        if (courseId.HasValue)
        {
            if (courseId.HasValue && db.Courses.Find(courseId) == null)
            {
                return NotFound(new MsgResponse { Message = "Course not found." });
            }

            postQuery = postQuery.Where(post => post.CourseId == courseId);
        }

        if (teacher != null)
        {
            if (!await db.Users.AnyAsync(u => u.Role == "teacher" && u.Name.ToLower().Contains(teacher)))
            {
                return NotFound(new MsgResponse { Message = "Teacher not found." });
            }

            postQuery = postQuery.Where(p => p.User.Name.ToLower().Contains(teacher));
        }

        var posts = await postQuery
            .Select(post => new GetPostDTO
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                TeacherName = post.User.Name,
                CourseName = post.Course.Name,
                Attachments = post.Attachments.Select(attachment => new AttachmentDTO
                {
                    Id = attachment.Id,
                    Type = attachment.Type,
                    Path = attachment.Path
                }).ToList()
            })
            .ToListAsync();

        return Ok(posts);
    }

    [HttpGet("{id}"), Authorize]
    public async Task<ActionResult<List<GetPostDTO>>> GetPost([FromRoute] int id)
    {
        // Check if the current user exists
        var userIdFromToken = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (!await db.Users.AnyAsync(u => u.Id == userIdFromToken))
        {
            return NotFound(new MsgResponse { Message = "User not found." });
        }

        var postQuery = db.Posts
            .Include(post => post.User)   // Include the User (teacher) related to the post
            .Include(post => post.Course) // Include the Course related to the post
            .Include(post => post.Attachments) // Include the Attachments related to the post
            .FirstOrDefault(x => x.Id == id);

        if (postQuery == null)
        {
            return NotFound(new MsgResponse { Message = "Post not found." });
        }

        var post = new GetPostDTO
        {
            Id = postQuery.Id,
            Title = postQuery.Title,
            Content = postQuery.Content,
            TeacherName = postQuery.User.Name,
            CourseName = postQuery.Course.Name,
            Attachments = postQuery.Attachments.Select(attachment => new AttachmentDTO
            {
                Id = attachment.Id,
                Type = attachment.Type,
                Path = attachment.Path
            }).ToList()
        };

        return Ok(post);
    }

    // Retrieve current user's posts and allows filtering by course
    [HttpGet("MyPosts"), Authorize(Roles = "teacher")]
    public async Task<ActionResult<List<GetPostDTO>>> MyPosts([FromQuery] int? courseId)
    {
        // Check if the current user exists
        var userIdFromToken = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (!await db.Users.AnyAsync(u => u.Id == userIdFromToken))
        {
            return NotFound(new MsgResponse { Message = "User not found." });
        }

        var postsQuery = db.Posts
            .Include(post => post.User)   // Include the User (teacher) related to the post
            .Include(post => post.Course) // Include the Course related to the post
            .Include(post => post.Attachments) // Include the Attachments related to the post
            .Where(post => post.User.Id == userIdFromToken);

        // Optionally, filter posts by courseId if provided
        if (courseId.HasValue)
        {
            if (!await db.Courses.AnyAsync(c => c.Id == courseId))
            {
                return NotFound(new MsgResponse { Message = "Course not found." });
            }
            postsQuery = postsQuery.Where(post => post.CourseId == courseId);
        }

        var posts = await postsQuery
            .Select(post => new GetPostDTO
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                TeacherName = post.User.Name,
                CourseName = post.Course.Name,
                Attachments = post.Attachments.Select(attachment => new AttachmentDTO
                {
                    Id = attachment.Id,
                    Type = attachment.Type,
                    Path = attachment.Path
                }).ToList()
            })
            .ToListAsync();

        return Ok(posts);
    }
    
    // Create a new post
    [HttpPost("CreatePost"), Authorize(Roles = "teacher")]
    public async Task<ActionResult<PostIDResponse>> CreatePost([FromBody] SetPostDTO newPost)
    {
        // Check if the current user exists
        var userIdFromToken = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (!await db.Users.AnyAsync(u => u.Id == userIdFromToken))
        {
            return NotFound(new MsgResponse { Message = "User not found." });
        }
        
        //Check if course exists
        if (!await db.Courses.AnyAsync(c => c.Id == newPost.CourseId))
        {
            return NotFound(new MsgResponse { Message = "Course not found." });
        }
        
        // Create a new Post entity based on the data provided in the NewPost DTO
        Post p1 = new Post
        {
            Title = newPost.Title,
            Content = newPost.Content,
            CourseId = newPost.CourseId,
            UserId = userIdFromToken
        };

        // Add the new post to the database
        db.Posts.Add(p1);

        // Save changes to the database
        await db.SaveChangesAsync();
        
        return Ok(new PostIDResponse{PostId = p1.Id});
    }
    
    // Edit existing post
    [HttpPut("UpdatePost/{postId}"), Authorize(Roles = "teacher")]
    public async Task<ActionResult<MsgResponse>> UpdatePost(int postId, [FromBody] SetPostDTO updatedPost)
    {
        // Check if the current user exists
        var userIdFromToken = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (!await db.Users.AnyAsync(u => u.Id == userIdFromToken))
        {
            return NotFound(new MsgResponse { Message = "User not found." });
        }
        
        //Check if course exists
        if (!await db.Courses.AnyAsync(c => c.Id == updatedPost.CourseId))
        {
            return NotFound(new MsgResponse { Message = "Course not found." });
        }
        
        var post = await db.Posts
            .Include(p => p.User) // Include the User navigation property
            .FirstOrDefaultAsync(p => p.Id == postId);

        // Check if the post exists
        if (post == null)
        {
            return NotFound(new MsgResponse { Message = "Post not found." });
        }
            
        // Check if the current user owns the post
        if (post.User.Id != userIdFromToken)
        {
            return Forbid();
        }
        
        // Update post properties
        post.Title = updatedPost.Title;
        post.Content = updatedPost.Content;
        post.CourseId = updatedPost.CourseId;

        // Save changes to the database
        await db.SaveChangesAsync();

        return Ok(new MsgResponse { Message = "Post updated successfully." });
    }
    
    // Delete a post
    [HttpDelete("DeletePost/{postId}"), Authorize(Roles = "teacher")]
    public async Task<ActionResult<MsgResponse>> DeletePost(int postId)
    {
        try
        {
            // Check if the current user exists
            var userIdFromToken = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!await db.Users.AnyAsync(u => u.Id == userIdFromToken))
            {
                return NotFound(new MsgResponse { Message = "User not found." });
            }

            var post = await db.Posts
                .Include(p => p.User) // Include the User navigation property
                .FirstOrDefaultAsync(p => p.Id == postId);

            // Check if the post exists
            if (post == null)
            {
                return NotFound(new MsgResponse { Message = "Post not found." });
            }

            // Check if the current user owns the post
            if (post.User.Id != userIdFromToken)
            {
                return Forbid();
            }

            // Handle attachments removal
            var attachments = await db.Attachments.Where(a => a.PostId == postId).ToListAsync();
            foreach (var attachment in attachments)
            {
                // Remove the file from Azure Blob Storage
                AttachmentHelper.RemoveFileOnAzure(attachment.Path, configuration.GetConnectionString("AzureBlobStorage")!);

                // Remove the attachment record from the database
                db.Attachments.Remove(attachment);
            }
            
            // Remove the post
            db.Posts.Remove(post);

            // Save changes to the database
            await db.SaveChangesAsync();

            return Ok(new MsgResponse { Message = "Post and attachments removed successfully." });
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
            // Handle any exceptions that may occur
            return StatusCode(500, new MsgResponse { Message = ex.Message });
        }
    }
}