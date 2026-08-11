using Microsoft.AspNetCore.Mvc;
using RushTodo.Api.Models;
using RushTodo.Api.Services;

namespace RushTodo.Api.Controllers;

[ApiController]
[Route("gardener")]
public class GardenerController : ControllerBase
{
    private readonly IGardenerService _gardenerService;

    public GardenerController(IGardenerService gardenerService)
    {
        _gardenerService = gardenerService;
    }

    [HttpGet]
    [ProducesResponseType<LookupItem[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<LookupItem[]>> Get()
    {
        return await _gardenerService.GetLookup();
    }
}
