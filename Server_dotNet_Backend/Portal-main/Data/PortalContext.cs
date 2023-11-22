using Microsoft.EntityFrameworkCore;

namespace Portal.Data;

public class PortalContext : DbContext
{
    public PortalContext(DbContextOptions<PortalContext> options): base(options){}
    
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<Comment> Comments => Set<Comment>();
}