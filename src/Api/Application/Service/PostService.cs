using Api.Domain;
using Api.Infrastructure.Repository;
using Api.Integration;

namespace Api.Application.Service;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IJsonPlaceholderApi _jsonPlaceholderApi;
    private readonly ICacheService _cache;
    private const string PostsCacheKey = "posts_cache";
    private static readonly TimeSpan CacheExpiryTime = TimeSpan.FromHours(1);

    public PostService(IPostRepository postRepository, IJsonPlaceholderApi jsonPlaceholderApi, ICacheService cache)
    {
        _postRepository = postRepository;
        _jsonPlaceholderApi = jsonPlaceholderApi;
        _cache = cache;
    }

    public async Task<List<Post>> GetPostsAsync()
    {
        var cachedPosts = await _cache.GetAsync<List<Post>>(PostsCacheKey);
        if (cachedPosts is not null)
        {
            return cachedPosts;
        }

        var posts = await _postRepository.GetAllPostsAsync();
        if (posts is null || posts.Count == 0)
        {
            posts = await FetchAndStorePostsAsync();
        }

        await _cache.SetAsync(PostsCacheKey, posts, CacheExpiryTime);
        return posts;
    }

    public async Task<Post> GetPostByIdAsync(int postId)
    {
        var cacheKey = GetPostCacheKey(postId);

        var cachedPost = await _cache.GetAsync<Post>(cacheKey);
        if (cachedPost is not null)
        {
            return cachedPost;
        }

        var post = await _postRepository.GetPostByIdAsync(postId) ?? await FetchAndStorePostByIdAsync(postId);

        await _cache.SetAsync(cacheKey, post, CacheExpiryTime);
        return post;
    }

    private async Task<List<Post>> FetchAndStorePostsAsync()
    {
        var posts = await _jsonPlaceholderApi.GetPosts();
        await _postRepository.AddPostsAsync(posts);
        await _postRepository.SaveChangesAsync();
        return posts;
    }

    private async Task<Post> FetchAndStorePostByIdAsync(int postId)
    {
        var post = await _jsonPlaceholderApi.GetPostById(postId);
        _postRepository.AddPost(post);
        await _postRepository.SaveChangesAsync();
        return post;
    }

    private static string GetPostCacheKey(int postId) => $"post_{postId}";
}