using Api.Domain;
using Api.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Repository;

public interface IPostRepository
{
    Task<List<Post>?> GetAllPostsAsync();
    Task<Post?> GetPostByIdAsync(int id);
    Task AddPostsAsync(List<Post> posts);
    void AddPost(Post post);
    Task SaveChangesAsync();
}

public class PostRepository : IPostRepository
{
    private readonly AppDbContext _dbContext;

    public PostRepository(AppDbContext dbContext) => _dbContext = dbContext;
    public async Task<List<Post>?> GetAllPostsAsync() => await _dbContext.Posts.ToListAsync();
    public async Task<Post?> GetPostByIdAsync(int id) => await _dbContext.Posts.FindAsync(id);
    public async Task AddPostsAsync(List<Post> posts) => await _dbContext.Posts.AddRangeAsync(posts);
    public void AddPost(Post post) => _dbContext.Posts.Add(post);
    public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync();
}