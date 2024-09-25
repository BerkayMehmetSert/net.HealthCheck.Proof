using Api.Domain;
using Refit;

namespace Api.Integration;

public interface IJsonPlaceholderApi
{
    [Get("/posts")]
    Task<List<Post>> GetPosts();

    [Get("/posts/{id}")]
    Task<Post> GetPostById(int id);
}