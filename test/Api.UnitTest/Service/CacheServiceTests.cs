using System.Text.Json;
using Api.Application.Service;
using Moq;
using StackExchange.Redis;

namespace Api.UnitTest.Service;

public class CacheServiceTests
{
    private readonly Mock<IDatabase> _mockDatabase;
    private readonly ICacheService _cacheService;

    public CacheServiceTests()
    {
        _mockDatabase = new Mock<IDatabase>();
        var mockCacheDatabaseProvider = new Mock<ICacheDatabaseProvider>();
        mockCacheDatabaseProvider.Setup(x => x.GetDatabase()).Returns(_mockDatabase.Object);
        _cacheService = new CacheService(mockCacheDatabaseProvider.Object);
    }

    [Fact]
    public async Task GetAsync_ReturnsValue_WhenKeyExists()
    {
        const string key = "testKey";
        const string expectedValue = "testValue"; 
        var jsonValue = JsonSerializer.Serialize(expectedValue);
        _mockDatabase.Setup(x => x.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync(jsonValue);

        var result = await _cacheService.GetAsync<string>(key);

        Assert.Equal(expectedValue, result);
    }


    [Fact]
    public async Task GetAsync_ReturnsNull_WhenKeyDoesNotExist()
    {
        const string key = "nonExistentKey";
        _mockDatabase.Setup(x => x.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync(RedisValue.Null);

        var result = await _cacheService.GetAsync<string>(key);

        Assert.Null(result);
    }

    [Fact]
    public async Task SetAsync_ReturnsTrue_WhenValueIsSet()
    {
        const string key = "testKey";
        const string value = "testValue";
        var jsonValue = JsonSerializer.Serialize(value);

        _mockDatabase.Setup(x =>
                x.StringSetAsync(key, jsonValue, It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        var result = await _cacheService.SetAsync(key, value);

        Assert.True(result);
    }


    [Fact]
    public async Task SetAsync_ReturnsFalse_WhenValueIsNotSet()
    {
        const string key = "testKey";
        const string value = "testValue";
        _mockDatabase
            .Setup(x => x.StringSetAsync(key, value, It.IsAny<TimeSpan?>(), It.IsAny<When>(),
                It.IsAny<CommandFlags>())).ReturnsAsync(false);

        var result = await _cacheService.SetAsync(key, value);

        Assert.False(result);
    }
}