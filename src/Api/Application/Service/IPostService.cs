using Api.Domain;

namespace Api.Application.Service;

public interface IPostService
{
    Task<List<Post>> GetPostsAsync();
    Task<Post> GetPostByIdAsync(int postId);
}