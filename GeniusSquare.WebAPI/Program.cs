using GeniusSquare.Core.Game;
using GeniusSquare.WebAPI.Caching;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeniusSquare.WebAPI;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddServices();
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

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

        // TODO: Implement cache monitor to remove LRU items in the background
        services.AddSingleton<IAsyncCache<SolutionKey, IAsyncCachedEnumerable<Solution>>>(
            new AsyncCache<SolutionKey, IAsyncCachedEnumerable<Solution>>());
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
