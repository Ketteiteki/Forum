using Forum.BackendServices.Entities;
using Forum.Contracts.Comment;

namespace Forum.Api.Interfaces;

public interface ICommentService
{
	Task<Comment?> GetCommentAsync(Guid commentId);

	Task<List<Comment>> GetCommentsAsync(Guid postId);
	
	Task<Comment> CreateCommentAsync(CommentRequest commentRequest, Guid userId,
		IFormFileCollection? filesRequest);

	Task<List<Comment>> GetCommentsByCommentAsync(Guid commentId);

	Task<Comment> UpdateCommentAsync(CommentUpdate commentUpdate, Guid userId, IFormFileCollection? filesRequest);

	Task<Comment> DeleteCommentAsync(Comment comment);
	
	Task<Comment?> ValidateOwnerCommentAsync(Guid userId, Guid commentId);
}