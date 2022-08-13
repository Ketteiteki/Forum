using Forum.BackendServices.Entities;
using Microsoft.EntityFrameworkCore;

namespace Forum.BackendServices.Database;

public class DatabaseContext : DbContext
{
	public DbSet<User> Users { get; set; } = null!;
	public DbSet<Comment> Comments { get; set; } = null!;
	public DbSet<Post> Posts { get; set; } = null!;
	public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
	public DbSet<PostAttachment> PostAttachments { get; set; } = null!;
	public DbSet<CommentAttachment> CommentAttachaments { get; set; } = null!;

	public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
	{
		base.Database.EnsureCreated();
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder
			.Entity<User>()
			.HasMany(u => u.Comments)
			.WithOne(c => c.Owner)
			.HasForeignKey(c => c.OwnerId)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder
			.Entity<User>()
			.HasMany(u => u.Posts)
			.WithOne(c => c.Owner)
			.HasForeignKey(c => c.OwnerId)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder
			.Entity<User>()
			.HasOne(u => u.RefreshToken)
			.WithOne(rt => rt.User)
			.HasForeignKey<RefreshToken>(rt => rt.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder
			.Entity<Post>()
			.HasMany(t => t.Comments)
			.WithOne(c => c.Post)
			.HasForeignKey(c => c.PostId)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder
			.Entity<Post>()
			.HasMany(p => p.Attachments)
			.WithOne(a => a.Post)
			.HasForeignKey(a => a.PostId)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder
			.Entity<Comment>()
			.HasMany(p => p.Attachments)
			.WithOne(a => a.Comment)
			.HasForeignKey(a => a.CommentId)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder
			.Entity<Comment>()
			.HasMany(c => c.Comments)
			.WithOne(c => c.ResponseToComment)
			.HasForeignKey(c => c.ResponseToCommentId)
			.OnDelete(DeleteBehavior.SetNull);
	}
}