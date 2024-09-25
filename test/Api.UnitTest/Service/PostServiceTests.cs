using Api.Application.Service;
using Api.Domain;
using Api.Infrastructure.Repository;
using Api.Integration;
using Moq;

namespace Api.UnitTest.Service;

public class PostServiceTests
{
    private readonly Mock<IPostRepository> _mockPostRepository;
    private readonly Mock<IJsonPlaceholderApi> _mockJsonPlaceholderApi;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly PostService _postService;

    public PostServiceTests()
    {
        _mockPostRepository = new Mock<IPostRepository>();
        _mockJsonPlaceholderApi = new Mock<IJsonPlaceholderApi>();
        _mockCacheService = new Mock<ICacheService>();
        _postService = new PostService(_mockPostRepository.Object, _mockJsonPlaceholderApi.Object,
            _mockCacheService.Object);
    }

    [Fact]
    public async Task GetPostsAsync_ReturnsCachedPosts_WhenCacheIsNotEmpty()
    {
        var cachedPosts = new List<Post> { new Post { Id = 1, Title = "Test Post" } };
        _mockCacheService.Setup(x => x.GetAsync<List<Post>>(It.IsAny<string>())).ReturnsAsync(cachedPosts);

        var result = await _postService.GetPostsAsync();

        Assert.Equal(cachedPosts, result);
    }

    [Fact]
    public async Task GetPostsAsync_ReturnsPostsFromRepository_WhenCacheIsEmpty()
    {
        var posts = new List<Post> { new Post { Id = 1, Title = "Test Post" } };
        _mockCacheService.Setup(x => x.GetAsync<List<Post>>(It.IsAny<string>())).ReturnsAsync((List<Post>?)null);
        _mockPostRepository.Setup(x => x.GetAllPostsAsync()).ReturnsAsync(posts);

        var result = await _postService.GetPostsAsync();

        Assert.Equal(posts, result);
    }

    [Fact]
    public async Task GetPostsAsync_FetchesAndStoresPosts_WhenRepositoryIsEmpty()
    {
        var posts = new List<Post> { new Post { Id = 1, Title = "Test Post" } };
        _mockCacheService.Setup(x => x.GetAsync<List<Post>>(It.IsAny<string>())).ReturnsAsync((List<Post>?)null);
        _mockPostRepository.Setup(x => x.GetAllPostsAsync()).ReturnsAsync(new List<Post>());
        _mockJsonPlaceholderApi.Setup(x => x.GetPosts()).ReturnsAsync(posts);

        var result = await _postService.GetPostsAsync();

        Assert.Equal(posts, result);
        _mockPostRepository.Verify(x => x.AddPostsAsync(posts), Times.Once);
        _mockPostRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPostByIdAsync_ReturnsCachedPost_WhenCacheIsNotEmpty()
    {
        var cachedPost = new Post { Id = 1, Title = "Test Post" };
        _mockCacheService.Setup(x => x.GetAsync<Post>(It.IsAny<string>())).ReturnsAsync(cachedPost);

        var result = await _postService.GetPostByIdAsync(1);

        Assert.Equal(cachedPost, result);
    }

    [Fact]
    public async Task GetPostByIdAsync_ReturnsPostFromRepository_WhenCacheIsEmpty()
    {
        var post = new Post { Id = 1, Title = "Test Post" };
        _mockCacheService.Setup(x => x.GetAsync<Post>(It.IsAny<string>())).ReturnsAsync((Post?)null);
        _mockPostRepository.Setup(x => x.GetPostByIdAsync(It.IsAny<int>())).ReturnsAsync(post);

        var result = await _postService.GetPostByIdAsync(1);

        Assert.Equal(post, result);
    }

    [Fact]
    public async Task GetPostByIdAsync_FetchesAndStoresPost_WhenRepositoryIsEmpty()
    {
        var post = new Post { Id = 1, Title = "Test Post" };
        _mockCacheService.Setup(x => x.GetAsync<Post>(It.IsAny<string>())).ReturnsAsync((Post?)null);
        _mockPostRepository.Setup(x => x.GetPostByIdAsync(It.IsAny<int>())).ReturnsAsync((Post?)null);
        _mockJsonPlaceholderApi.Setup(x => x.GetPostById(It.IsAny<int>())).ReturnsAsync(post);

        var result = await _postService.GetPostByIdAsync(1);

        Assert.Equal(post, result);
        _mockPostRepository.Verify(x => x.AddPost(post), Times.Once);
        _mockPostRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}