using GeniusSquare.WebAPI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace GeniusSquare.WebAPI.Controllers;

[ApiController]
[Route("config")]
public class ConfigController : ControllerBase
{
    private readonly IDictionary<string, Model.Config> _configs;
    private readonly ILogger<ConfigController> _logger;

    public ConfigController(
        IDictionary<string, Model.Config> configs,
        ILogger<ConfigController> logger)
    {
        _configs = configs;
        _logger = logger;
    }

    [HttpGet("{configId}")]
    public ActionResult<Model.Config> Get(string configId)
    {
        configId = configId.NormaliseId();

        if (!_configs.TryGetValue(configId, out Model.Config? config))
        {
            return NotFound($"Unknown config: '{configId}'");
        }

        return config;
    }
}
