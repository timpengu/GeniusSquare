using GeniusSquare.Core.Game;
using GeniusSquare.WebAPI.Caching;
using Nito.Disposables;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeniusSquare.WebAPI;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddServices();
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Start/stop cache monitors
        await using CollectionAsyncDisposable cacheMonitors = new(app.Services.GetServices<IAsyncCacheMonitor>());

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton(LoadConfigs());
        services.AddSingleton(LoadPieces());

        services.AddCache<SolutionKey, IAsyncCachedEnumerable<Solution>>(
            $"AsyncCache<{nameof(Solution)}>",
            TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(1), 3, 5); // TODO: Read cache parameters from config
    }

    private static void AddCache<TKey, TValue>(
        this IServiceCollection services,
        string cacheName, TimeSpan cacheTimeout, TimeSpan monitorInterval, int cacheLowWatermark, int cacheHighWatermark)
        where TKey : notnull
    {
        services.AddSingleton<IAsyncCache<TKey, TValue>>(serviceProvider =>
            new AsyncCache<TKey, TValue>());

        services.AddSingleton<IAsyncCacheMonitor>(serviceProvider =>
            new AsyncCacheMonitor<TKey, TValue>(
                serviceProvider.GetService<IAsyncCache<TKey,TValue>>()!,
                serviceProvider.GetService<ILogger<AsyncCacheMonitor<TKey, TValue>>>()!,
                cacheName, cacheTimeout, monitorInterval, cacheLowWatermark, cacheHighWatermark
            ));
    }

    // TODO: Add a persistent store for configs, pieces, etc
    private static IEnumerable<Model.Config> LoadConfigs() => LoadJsonArray<Model.Config>("Configuration/Configs.json");
    private static IEnumerable<Model.Piece> LoadPieces() => LoadJsonArray<Model.Piece>("Configuration/Pieces.json");

    // TODO: Share LoadJson() implementation with GeniusSquare.Configuration.Config.Load()
    private static IEnumerable<T> LoadJsonArray<T>(string path) => LoadJson<IEnumerable<T>>(path);
    private static T LoadJson<T>(string path)
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions)
            ?? throw new Exception($"Failed to deserialize {typeof(T).Name}");
    }

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        ReadCommentHandling = JsonCommentHandling.Skip,
    };
}
