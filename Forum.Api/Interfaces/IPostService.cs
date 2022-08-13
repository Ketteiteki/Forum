using Forum.BackendServices.Entities;
using Forum.Contracts.Post;

namespace Forum.Api.Interfaces;

public interface IPostService
{
	Task<Post?> GetPostAsync(Guid id);

	Task<List<Post>> GetPostsAsync(string? search, int page, int count);
	
	Task<List<Post>> GetPostsByUserAsync(Guid id, int page, int count);
	
	Task<Post> CreatePostAsync(PostRequest postRequest, Guid userId, IFormFileCollection? filesRequest);

	Task<Post> UpdatePostAsync(PostUpdate postUpdate, Guid userId, IFormFileCollection? filesRequest);

	Task<Post> DeletePostAsync(Post post);
	
	Task<Post?> ValidateOwnerPostAsync(Guid userId, Guid postId);
}