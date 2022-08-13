using Forum.BackendServices.Entities;

namespace Forum.Api.DTOs;

public class CommentAttachmentDto
{
	public Guid Id { get; set; }

	public string FileName { get; set; }
	
	public double FileSize { get; set; }

	public CommentAttachmentDto(CommentAttachment postAttachment)
	{
		Id = postAttachment.Id;
		FileName = postAttachment.FileName;
		FileSize = postAttachment.FileSize;
	}
}