
using GeniusSquare.WebAPI.Model;

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
            LoadConfigs()
            .ToDictionary(config => config.Id));
    }

    private static IEnumerable<Config> LoadConfigs()
    {
        // TODO: load default config from file
        yield return new Config
        {
            Id = Config.DefaultId,
            BoardSize = new(6, 6),
            Transformation = Transformation.RotateReflect,
            Pieces = { "O1", "I2", "I3", "J3", "I4", "J4", "O4", "T4", "S4" }
        };
    }
}
