using Microsoft.AspNetCore.Mvc;
using RushTodo.Api.Models;
using RushTodo.Api.Services;

namespace RushTodo.Api.Controllers;

[ApiController]
[Route("work-item")]
public class WorkItemController : ControllerBase
{
    private readonly IWorkItemService _workItemService;

    public WorkItemController(IWorkItemService workItemService)
    {
        _workItemService = workItemService;
    }

    [HttpGet]
    [ProducesResponseType<WorkItemModel[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<WorkItemModel[]>> Search(
        [FromQuery] WorkItemSearchParameters parameters,
        CancellationToken cancellationToken)
    {
        return await _workItemService.Search(parameters, cancellationToken);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<WorkItemModel>(StatusCodes.Status200OK)]
    public async Task<ActionResult<WorkItemModel>> Get(int id)
    {
        return await _workItemService.Get(id);
    }

    [HttpPost]
    [ProducesResponseType<WorkItemModel>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkItemModel>> Create(WorkItemModel model)
    {
        if (model.WorkItemId is not null) return BadRequest("WorkItemId must be omitted when creating a WorkItem.");

        var result = await _workItemService.Save(model);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<WorkItemModel>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkItemModel>> Update(int id, WorkItemModel model)
    {
        if (model.WorkItemId is not null && model.WorkItemId != id)
            return BadRequest("The route ID does not match WorkItemId.");

        model.WorkItemId = id;
        return await _workItemService.Save(model);
    }
}
