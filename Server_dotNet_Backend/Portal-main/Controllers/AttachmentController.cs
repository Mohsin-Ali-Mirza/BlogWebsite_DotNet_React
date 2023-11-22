namespace Portal.Controllers;

// This controller will manage all attachment related endpoints and they will all begin with "/Attachment"
[Route("[controller]")]
[ApiController]
public class AttachmentController : ControllerBase
{
    private readonly PortalContext db;
    private readonly IConfiguration configuration;

    // Constructor to initialize the controller with the database context and configuration file
    public AttachmentController(PortalContext db, IConfiguration configuration)
    {
        this.db = db;
        this.configuration = configuration;
    }
    
    // Store an attachment for the given postId
    [HttpPost("Store/{postId}"), Authorize(Roles = "teacher")]
    public async Task<ActionResult<MsgResponse>> StoreAttachment(int postId, IFormFile file)
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

            // Store the file on the server and get the file path(URL)
            string filePath = AttachmentHelper.StoreFileOnAzure(file, configuration.GetConnectionString("AzureBlobStorage")!);

            // Create a new Attachment record linked to the given post
            var attachment = new Attachment
            {
                Type = file.ContentType,
                Path = filePath,
                PostId = postId
            };

            // Add the record to the database
            db.Attachments.Add(attachment);
            await db.SaveChangesAsync();

            return Ok(new MsgResponse { Message = "Attachment stored successfully." });
        }
        catch (ArgumentException)
        {
            // If the file was empty or null
            return BadRequest(new MsgResponse { Message = "File is empty or null." });
        }
        catch (Exception ex)
        {
            // Handle any exceptions that may occur (e.g., DB errors, File storage errors)
            return StatusCode(500, new MsgResponse { Message = ex.Message });
        }
    }
    
    // Delete an attachment using the given attachmentId
    [HttpDelete("Delete/{attachmentId}"), Authorize(Roles = "teacher")]
    public async Task<ActionResult<MsgResponse>> DeleteAttachment(int attachmentId)
    {
        try
        {
            // Check if the current user exists
            var userIdFromToken = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!await db.Users.AnyAsync(u => u.Id == userIdFromToken))
            {
                return NotFound(new MsgResponse { Message = "User not found." });
            }

            // Find the attachment by its ID
            var attachment = await db.Attachments
                .Include(a => a.Post.User) // Include navigation properties
                .FirstOrDefaultAsync(a => a.Id == attachmentId);

            // Check if the attachment exists
            if (attachment == null)
            {
                return NotFound(new MsgResponse { Message = "Attachment not found." });
            }

            // Check if the current user owns the post that the attachment belongs to
            if (attachment.Post.User.Id != userIdFromToken)
            {
                return Forbid();
            }
            
            // Remove the file from Azure Blob Storage
            AttachmentHelper.RemoveFileOnAzure(attachment.Path, configuration.GetConnectionString("AzureBlobStorage")!);

            // Remove the attachment record from the database
            db.Attachments.Remove(attachment);
            await db.SaveChangesAsync();

            return Ok(new MsgResponse { Message = "Attachment removed successfully." });
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
            // Handle any exceptions that may occur (e.g., DB errors, File removal errors)
            return StatusCode(500, new MsgResponse { Message = ex.Message });
        }
    }
}


public static class AttachmentHelper
{
    // Method to store the file on Azure Blob Storage and return the URL
    public static string StoreFileOnAzure(IFormFile file, string connString)
    {
        //We'll handle this exception in the API and send a bad request response
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is empty or null.");
        }
        
        // Use the predefined container name used to store all attachments
        string containerName = "attachments";
        
        // Generate a unique blob name to prevent conflicts
        string blobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        
        // Create all the required clients for communication with Azure Blob Storage
        BlobServiceClient serviceClient = new BlobServiceClient(connString);
        BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(containerName);
        containerClient.CreateIfNotExists();
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        // Upload the recieved file
        using (var stream = file.OpenReadStream())
        {
            blobClient.Upload(stream, true);
        }

        // Returnt the URL to the stored file
        return blobClient.Uri.ToString();
    }
    
    // Method to remove the file on Azure Blob Storage, throws an argument if blob doesn't exist
    public static void RemoveFileOnAzure(string blobPath, string connString)
    {
        // Use the predefined container name used to store all attachments
        string containerName = "attachments";
        
        // Extract the Blob Name from the Blob Path
        string blobName = new Uri(blobPath).Segments.Last();
        
        // Create all the required clients for communication with Azure Blob Storage
        BlobServiceClient serviceClient = new BlobServiceClient(connString);
        BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(containerName);
        containerClient.CreateIfNotExists();
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        if (blobClient.Exists())
        {
            // Delete the Blob
            blobClient.Delete(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);
        }
        else
        {
            throw new ArgumentException("Blob doesn't exist on Azure Blob Storage.");
        }
    }
}