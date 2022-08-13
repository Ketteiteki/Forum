using Forum.BackendServices.Entities;

namespace Forum.Api.DTOs;

public class PostAttachmentDto
{
	public Guid Id { get; set; }

	public string FileName { get; set; }
	
	public double FileSize { get; set; }

	public PostAttachmentDto(PostAttachment postAttachment)
	{
		Id = postAttachment.Id;
		FileName = postAttachment.FileName;
		FileSize = postAttachment.FileSize;
	}
}