using GeniusSquare.WebAPI.Model;
using Microsoft.AspNetCore.Mvc;

namespace GeniusSquare.WebAPI.Controllers;

[ApiController]
[Route("config")]
public class ConfigController : ControllerBase
{
    private readonly IDictionary<string, Config> _configs;
    private readonly ILogger<ConfigController> _logger;

    public ConfigController(IDictionary<string, Config> configs, ILogger<ConfigController> logger)
    {
        _configs = configs;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public ActionResult<Config> Get(string id)
    {
        if (!_configs.TryGetValue(id, out Config? config))
        {
            return NotFound($"Unknown config: '{id}'");
        }

        return config;
    }
}
