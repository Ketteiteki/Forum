using Forum.BackendServices.Entities;

namespace Forum.Api.DTOs;

public class CommentDto
{
	public Guid Id { get; set; }

	public string Text { get; set; }
	
	public bool IsOp { get; set; }

	public bool IsSage { get; set; }
	
	public Guid? ResponseToCommentId { get; set; }
	
	public List<CommentAttachmentDto> Attachments { get; set; }

	public DateTime DateOfCreate { get; set; }

	public Guid OwnerId { get; set; }

	public UserDto Owner { get; set; }

	public Guid PostId { get; set; }

	public CommentDto(Comment comment)
	{
		Id = comment.Id;
		Text = comment.Text;
		IsOp = comment.IsOp;
		IsSage = comment.IsSage;
		ResponseToCommentId = comment.ResponseToCommentId;
		Attachments = comment.Attachments.Select(a => new CommentAttachmentDto(a)).ToList();
		DateOfCreate = comment.DateOfCreate;
		OwnerId = comment.OwnerId;
		Owner = new UserDto(comment.Owner);
		PostId = comment.PostId;
	}
}