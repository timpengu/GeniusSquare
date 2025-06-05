
using GeniusSquare.WebAPI.Model;
using System.Text.Json.Serialization;
using System.Text.Json;

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
        services.AddSingleton<IDictionary<string, Config>>(
            LoadConfigs().ToDictionary(config => config.Id));

        services.AddSingleton<IDictionary<string, Piece>>(
            LoadPieces().ToDictionary(piece => piece.Id));
    }

    private static IEnumerable<Config> LoadConfigs() => LoadJson<Config[]>("Configuration/Configs.json");
    private static IEnumerable<Piece> LoadPieces() => LoadJson<Piece[]>("Configuration/Pieces.json");

    // TODO: Share LoadJson() implementation with GeniusSquare.Configuration.Config.Load()
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
