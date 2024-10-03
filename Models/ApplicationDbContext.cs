using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ChatMessage> ChatMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 添加初始數據
        modelBuilder.Entity<ChatMessage>().HasData(
            new ChatMessage { Id = 1, UserName = "Alice", Message = "Hello, world!", Timestamp = DateTime.Now },
            new ChatMessage { Id = 2, UserName = "Bob", Message = "Hi, Alice!", Timestamp = DateTime.Now.AddMinutes(-5) }
        );
    }
}