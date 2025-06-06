using GeniusSquare.WebAPI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace GeniusSquare.WebAPI.Controllers;

[ApiController]
[Route("config")]
public class ConfigController : ControllerBase
{
    private readonly IDictionary<string, Model.Config> _configs;
    private readonly ILogger<ConfigController> _logger;

    public ConfigController(IEnumerable<Model.Config> configs, ILogger<ConfigController> logger)
    {
        _configs = configs.ToDictionary(c => c.Id.NormaliseId());
        _logger = logger;
    }

    [HttpGet("{id}")]
    public ActionResult<Model.Config> Get(string id)
    {
        id = id.NormaliseId();

        if (!_configs.TryGetValue(id, out Model.Config? config))
        {
            return NotFound($"Unknown config: '{id}'");
        }

        return config;
    }
}
