using System.Text.RegularExpressions;
using Forum.Api.Exceptions;
using Forum.Api.Interfaces;
using Forum.BackendServices.Database;
using Forum.BackendServices.Entities;
using Forum.Contracts.Post;
using Microsoft.EntityFrameworkCore;

namespace Forum.Api.Services;

public class PostService : IPostService
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;
	
	public PostService(DatabaseContext context, IFileService fileService)
	{
		_context = context;
		_fileService = fileService;
	}

	public async Task<Post?> GetPostAsync(Guid id)
	{
		var post = await _context.Posts.Include(p => p.Owner).Include(p => p.Attachments).FirstOrDefaultAsync(u => u.Id == id);

		return post;
	}
	
	public async Task<List<Post>> GetPostsAsync(string? search, int page, int count)
	{
		var posts = search == null
			? await  _context.Posts
				.OrderBy(p => p.DateOfCreate)
				.Include(p => p.Owner)
				.Include(p => p.Attachments)
				.Skip((page - 1) * count)
				.Take(count)
				.ToListAsync()
			: await  _context.Posts
				.OrderBy(p => p.DateOfCreate)
				.Include(p => p.Owner)
				.Include(p => p.Attachments)
				.Where(p => Regex.IsMatch(p.Title, $"{search}"))
				.Skip((page - 1) * count)
				.Take(count)
				.ToListAsync();
		
		return posts;
	}

	public async Task<List<Post>> GetPostsByUserAsync(Guid id, int page, int count)
	{
		var posts = await  _context.Posts
				.OrderBy(p => p.DateOfCreate)
				.Include(p => p.Owner)
				.Include(p => p.Attachments)
				.Where(p => p.OwnerId == id)
				.Skip((page - 1) * count)
				.Take(count)
				.ToListAsync();
		
		return posts;
	}
	
	public async Task<Post> CreatePostAsync(PostRequest postRequest, Guid userId, IFormFileCollection? filesRequest)
	{
		var createPost = new Post
		{
			Id = Guid.NewGuid(),
			Title = postRequest.Title,
			Text = postRequest.Text,
			OwnerId = userId
		};
		_context.Posts.Add(createPost);
		
		if (filesRequest != null)
		{
			foreach (var file in filesRequest)
			{
				var attachment = new PostAttachment
				{
					Id = Guid.NewGuid(),
					FileName = file.FileName,
					FileSize = file.Length,
					PostId = createPost.Id,
					Post = createPost
				};
				await _fileService.CreateFileAsync(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", 
					"posts", createPost.Id.ToString(), $"{attachment.Id.ToString()}.jpeg"), file);
				_context.PostAttachments.Add(attachment);
			}
		}
		await _context.SaveChangesAsync();
		
		_context.Entry(createPost).Reference(p => p.Owner).Load();
		
		return createPost;
	}
	
	public async Task<Post> UpdatePostAsync(PostUpdate postUpdate, Guid userId , IFormFileCollection? filesRequest)
	{
		var validateOwnerPost = await ValidateOwnerPostAsync(userId, postUpdate.Id);
		if (validateOwnerPost == null) throw new SimpleForbiddenException("Нелья редактировать чужой пост");
		
		foreach (var attachment in validateOwnerPost.Attachments)
		{
			_fileService.RemoveFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", 
				"posts", validateOwnerPost.Id.ToString(), $"{attachment.Id.ToString()}.jpeg"));
		}

		var oldAttachments = _context.PostAttachments.Where(pa => pa.PostId == validateOwnerPost.Id).ToList();
		_context.PostAttachments.RemoveRange(oldAttachments);
		
		if (filesRequest != null)
		{
			foreach (var file in filesRequest)
			{
				var attachment = new PostAttachment
				{
					Id = Guid.NewGuid(),
					FileName = file.FileName,
					FileSize = file.Length,
					PostId = validateOwnerPost.Id,
				};
			
				await _fileService.CreateFileAsync(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", 
					"posts", validateOwnerPost.Id.ToString(), $"{attachment.Id.ToString()}.jpeg"), file);
			
				_context.PostAttachments.Add(attachment);
			}
		}
		validateOwnerPost.Title = postUpdate.Title;
		validateOwnerPost.Text = postUpdate.Text;
		
		_context.Posts.Update(validateOwnerPost);
		await _context.SaveChangesAsync();

		return validateOwnerPost;
	}
	
	public async Task<Post> DeletePostAsync(Post post)
	{
		if (post == null) throw new SimpleForbiddenException("Нельзя удалять чужой пост");

		_fileService.RemoveDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", 
			"posts", post.Id.ToString()));

		_context.Posts.Remove(post);
		await _context.SaveChangesAsync();
		
		return post;
	}

	public async Task<Post?> ValidateOwnerPostAsync(Guid userId, Guid postId)
	{
		var findPost = await _context.Posts
			.Include(p => p.Owner)
			.Include(p => p.Attachments)
			.FirstOrDefaultAsync(p => p.Id == postId 
			                          && p.OwnerId == userId);
		
		return findPost;
	}
}