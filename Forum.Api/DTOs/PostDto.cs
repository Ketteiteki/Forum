using Forum.BackendServices.Entities;

namespace Forum.Api.DTOs;

public class PostDto
{
	public Guid Id { get; set; }

	public string Title { get; set; }

	public string Text { get; set; }

	public int PostUpPoints { get; set; }
	
	public List<PostAttachmentDto> Attachments { get; set; }

	public DateTime DateOfCreate { get; set; }
	
	public Guid OwnerId { get; set; }

	public UserDto Owner { get; set; }

	public PostDto(Post post)
	{
		Id = post.Id;
		Title = post.Title;
		Text = post.Text;
		PostUpPoints = post.PostUpPoints;
		Attachments = post.Attachments.Select(a => new PostAttachmentDto(a)).ToList();
		DateOfCreate = post.DateOfCreate;
		OwnerId = post.OwnerId;
		Owner = new UserDto(post.Owner);
		DateOfCreate = post.DateOfCreate;
	}
}