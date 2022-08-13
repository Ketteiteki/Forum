using Forum.Api.Exceptions;
using Forum.Api.Interfaces;
using Forum.BackendServices.Database;
using Forum.BackendServices.Entities;
using Forum.Contracts.Comment;
using Microsoft.EntityFrameworkCore;

namespace Forum.Api.Services;

public class CommentService : ICommentService
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;

	public CommentService(DatabaseContext context, IFileService fileService)
	{
		_context = context;
		_fileService = fileService;
	}

	public async Task<Comment?> GetCommentAsync(Guid commentId)
	{
		var comment = await _context.Comments
			.Include(c => c.Owner)
			.Include(c => c.Attachments)
			.FirstOrDefaultAsync(c => c.Id == commentId);

		return comment;
	}

	public async Task<List<Comment>> GetCommentsAsync(Guid postId)
	{
		var comments = await _context.Comments
			.Include(c => c.Owner)
			.Include(c => c.Attachments)
			.Where(c => c.PostId == postId)
			.ToListAsync();

		return comments;
	}

	public async Task<List<Comment>> GetCommentsByCommentAsync(Guid commentId)
	{
		var comments = await _context.Comments
			.Include(c => c.Owner)
			.Include(c => c.Attachments)
			.Where(c => c.ResponseToCommentId == commentId)
			.ToListAsync();

		return comments;
	}

	public async Task<Comment> CreateCommentAsync(CommentRequest commentRequest, Guid userId, IFormFileCollection? filesRequest)
	{
		var findPost = await _context.Posts.Include(p => p.Owner).FirstOrDefaultAsync(p => p.Id == commentRequest.PostId);
		if (findPost == null) throw new SimpleDbEntityNotFoundException("Пост не найден");
			
		var createComment = new Comment
		{
			Id = Guid.NewGuid(),
			Text = commentRequest.Text,
			IsOp = userId == findPost.Owner.Id,
			IsSage = commentRequest.IsSage,
			ResponseToCommentId = commentRequest.ResponseToCommentId,
			PostId = commentRequest.PostId,
			OwnerId = userId
		};
		
		if (filesRequest != null)
		{
			foreach (var file in filesRequest)
			{
				var attachment = new CommentAttachment
				{
					Id = Guid.NewGuid(),
					FileName = file.FileName,
					FileSize = file.Length,
					CommentId = createComment.Id,
					Comment = createComment
				};
			
				await _fileService.CreateFileAsync(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", 
					"comments", createComment.Id.ToString(), $"{attachment.Id.ToString()}.jpeg"), file);
			
				_context.CommentAttachaments.Add(attachment);
			}
		}

		if (createComment.IsSage == false)
			findPost.PostUpPoints++;

		_context.Comments.Add(createComment);
		await _context.SaveChangesAsync();

		return createComment;
	}

	public async Task<Comment> UpdateCommentAsync(CommentUpdate commentUpdate, Guid userId , IFormFileCollection? filesRequest)
	{
		var validateComment = await ValidateOwnerCommentAsync(userId, commentUpdate.Id);
		if (validateComment == null) throw new SimpleForbiddenException("");
		
		foreach (var attachment in validateComment.Attachments)
		{
			_fileService.RemoveFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", 
				"comments", validateComment.Id.ToString(), $"{attachment.Id.ToString()}.jpeg"));
		}

		var oldAttachments = _context.CommentAttachaments.Where(cp => cp.CommentId == validateComment.Id).ToList();
		_context.CommentAttachaments.RemoveRange(oldAttachments);
		
		if (filesRequest != null)
		{
			foreach (var file in filesRequest)
			{
				var attachment = new CommentAttachment
				{
					Id = Guid.NewGuid(),
					FileName = file.FileName,
					FileSize = file.Length,
					CommentId = validateComment.Id,
					Comment = validateComment
				};
			
				await _fileService.CreateFileAsync(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", 
					"comments", validateComment.Id.ToString(), $"{attachment.Id.ToString()}.jpeg"), file);
			
				_context.CommentAttachaments.Add(attachment);
			}
		}

		validateComment.Text = commentUpdate.Text;

		_context.Comments.Update(validateComment);
		await _context.SaveChangesAsync();
		
		_context.Entry(validateComment).Reference(c => c.Owner).Load();
		
		return validateComment;
	}

	public async Task<Comment> DeleteCommentAsync(Comment comment)
	{
		_fileService.RemoveDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", 
				"comments", comment.Id.ToString()));

		_context.Comments.Remove(comment);
		await _context.SaveChangesAsync();

		return comment;
	}

	public async Task<Comment?> ValidateOwnerCommentAsync(Guid userId, Guid commentId)
	{
		var findComment = await _context.Comments
			.Include(c => c.Owner)
			.Include(c => c.Attachments)
			.FirstOrDefaultAsync(c => c.Id == commentId && c.OwnerId == userId);
		return findComment;
	}
}