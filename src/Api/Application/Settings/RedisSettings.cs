namespace Api.Application.Settings;

public class RedisSettings
{
    public string EndPoint { get; set; }
    public string Password { get; set; }
    public int DefaultDatabase { get; set; }
    public int SyncTimeout { get; set; }
    public int ConnectTimeout { get; set; }
    public string KeyPrefix { get; set; }
}